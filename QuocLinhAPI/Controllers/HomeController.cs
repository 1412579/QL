using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using BUS;
using Newtonsoft.Json;
namespace QuocLinhAPI.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            //var json = Helpers.Instance.GetJSON("https://api.adincube.com/api/1.3/public/reporting/overview?start=2019-05-05&end=2019-05-15&auth_token=e59658c1967040aa82333e5e25533caf");
            var user = Session["user"] as User;
            if (user == null)
                return Redirect("~/Login");
            var model = new List<InfoApi>();
            if (user.Role == (int)Role.User)
                model = BUS_API.Instance.GetAllByUserId(user.UserId);
            else model = BUS_API.Instance.GetAllDistinct();
            return View(model);
        }

        public ActionResult CTRNumber()
        {
            if (!IsAdminLogged())
                return Redirect("~/");
            var model = BUS_CTR.Instance.Get();
            if (model != null)
                return View(model);
            return View();
        }

        [HttpPost]
        public ActionResult CTRNumber(TrickAvarage model)
        {
            if (!IsAdminLogged())
                return Redirect("~/");
            if (model.TrickAvarageId > 0)
            {
                BUS_CTR.Instance.Update(model);
            }
            else
            {
                model.TrickAvarageId = 9999;
                BUS_CTR.Instance.Insert(model);
            }
            return RedirectToAction("CTRNumber");
        }

        [HttpPost]
        public ActionResult GetData(string ListSelected, string start, string end)
        {

            var user = Session["user"] as User;
            if (user == null)
                return Json(new
                {
                    Status = -1,
                    Msg = "Không có quyền truy cập."
                });

            var ListData = new List<ListDataApi>();
            var IsAdmin = (Role)user.Role == Role.Admin;
            var model = new List<InfoApi>();
            if (!IsAdmin)
                model = BUS_API.Instance.GetAllByUserId(user.UserId);
            else
            {
                model = BUS_API.Instance.GetAllDistinct();
                if (!string.IsNullOrEmpty(ListSelected) && ListSelected != "null")
                    model = model.Where(x => ListSelected.IndexOf(x.InfoApiId.ToString()) >= 0).ToList();


            }
            var ListCalled = new List<CalledToAppodeal>();
            var NoData = new NoData();
            var TimeOut = new List<int>();
            var NoResult = new List<int>();
            var CTRValue = 1.0;
            if (!IsAdmin)
            {
                CTRValue = user.RevVal.Value * 1.0 / 100; 
            }
            foreach (var item in model)
            {
                if ((API)item.TypeAPI == API.Appodeal)
                {
                    if(ListCalled != null && ListCalled.Any(x=>x.InfoApiId == item.InfoApiId))
                    {
                        var RowData = new List<DataApi>();
                        var Saved = ListCalled.FirstOrDefault(x => x.InfoApiId == item.InfoApiId);
                        foreach (var data in Saved.Data)
                        {
                            if (item.Container != data["app_key"])
                                continue;
                            var unit = new DataApi();
                            unit.Name = data["app_name"];
                            unit.CountryCode = data["country_code"];
                            unit.Platform = data["platform"];
                            unit.Requests = Convert.ToInt32(data["requests"]);
                            unit.Fills = Convert.ToInt32(data["fills"]);
                            unit.Impressions = Convert.ToInt32(data["impressions"]);
                            unit.Fillrate = Convert.ToDouble(data["fillrate"]);
                            unit.Clicks = Convert.ToInt32(data["clicks"]);
                            unit.CTR = Convert.ToDouble(data["ctr"]);
                            unit.Views = Convert.ToInt32(data["views"]);
                            unit.Revenue = Convert.ToInt32(data["revenue"]) * CTRValue;
                            unit.Ecpm = Convert.ToDouble(data["ecpm"]);
                            RowData.Add(unit);
                        }
                        if(RowData != null && RowData.Any())
                            ListData.Add(new ListDataApi()
                            {
                                Html = BuildStringData(RowData, item, IsAdmin),
                                TotalImpressions = item.Impression == 1 || IsAdmin ? RowData.Sum(x => x.Impressions) : 0,
                                TotalClicks = item.Click == 1 || IsAdmin ? RowData.Sum(x => x.Clicks) : 0,
                                TotalRevenue = item.Revenues == 1 || IsAdmin ? RowData.Sum(x => x.Revenue) : 0,
                                //TotalFillrate = RowData.Average(x => x.Fillrate),
                                InfoApiId = item.InfoApiId
                            });
                    }
                    var url = Helpers.Instance.GetApiLink(item.TypeAPI.Value, item.Token, item.Container, start, end, item.UserIdApi.Value);
                    var objTask = Helpers.Instance.GetJSON(url);
                    if (objTask != null)
                    {
                        var TaskId = 0;
                        if (objTask["task_id"] != null)
                            TaskId = Convert.ToInt32(objTask["task_id"]);
                        if (TaskId > 0)
                        {
                                bool IsNoResult = true;
                            dynamic rsl = null;
                            url = Helpers.Instance.GetApiLink(item.TypeAPI.Value, item.Token, item.Container, start, end, item.UserIdApi.Value, TypeAppodeal.Output, TaskId);
                            var RowData = new List<DataApi>();
                            Thread.Sleep(2000);
                            for (var i = 0; i < 5; i++)
                            {
                                rsl = Helpers.Instance.GetJSON(url);
                                try
                                {
                                    if (rsl != null && rsl["message"] == "success")
                                    {
                                        rsl = rsl["data"];
                                        ListCalled.Add(new CalledToAppodeal() { InfoApiId = item.InfoApiId, Data = rsl });
                                        //parse dữ liệu này
                                        foreach (var data in rsl)
                                        {
                                            if (item.Container != data["app_key"])
                                                continue;
                                            var unit = new DataApi();
                                            unit.Name = data["app_name"];
                                            unit.CountryCode = data["country_code"];
                                            unit.Platform = data["platform"];
                                            unit.Requests = Convert.ToInt32(data["requests"]);
                                            unit.Fills = Convert.ToInt32(data["fills"]);
                                            unit.Impressions = Convert.ToInt32(data["impressions"]);
                                            unit.Fillrate = Convert.ToDouble(data["fillrate"]);
                                            unit.Clicks = Convert.ToInt32(data["clicks"]);
                                            unit.CTR = Convert.ToDouble(data["ctr"]);
                                            unit.Views = Convert.ToInt32(data["views"]);
                                            unit.Revenue = Convert.ToInt32(data["revenue"]) * CTRValue;
                                            unit.Ecpm = Convert.ToDouble(data["ecpm"]);
                                            RowData.Add(unit);
                                        }
                                        if (RowData != null && RowData.Any())
                                        {
                                            ListData.Add(new ListDataApi()
                                            {
                                                Html = BuildStringData(RowData, item, IsAdmin),
                                                TotalImpressions = item.Impression == 1 || IsAdmin ? RowData.Sum(x => x.Impressions) : 0,
                                                TotalClicks = item.Click == 1 || IsAdmin ? RowData.Sum(x => x.Clicks) : 0,
                                                TotalRevenue = item.Revenues == 1 || IsAdmin ? RowData.Sum(x => x.Revenue) : 0,
                                                //TotalFillrate = RowData.Average(x => x.Fillrate),
                                                InfoApiId = item.InfoApiId
                                            });
                                            IsNoResult = false;
                                        }
                                        else
                                        {
                                            NoResult.Add(item.InfoApiId);
                                        }   
                                        break;
                                    }
                                }
                                catch {
                                    Thread.Sleep(2000);
                                }
                            }
                            if (IsNoResult)
                            {
                                NoResult.Add(item.InfoApiId);
                            }
                        }
                    }
                }
                else if((API)item.TypeAPI == API.Adincube)
                {
                    var url = Helpers.Instance.GetApiLink(item.TypeAPI.Value, item.Token, item.Container, start, end);
                    var Data = Helpers.Instance.GetJSON(url);
                    try
                    {
                        if (Data != null && Data["status"] == 401)
                        {
                            NoResult.Add(item.InfoApiId);
                        }
                    }
                    catch
                    {
                        var totalDays = 0;
                        var RowData = new List<DataApi>();
                        foreach (var adin in Data)
                        {
                            totalDays++;
                            foreach (var unit in adin["impressions"])
                            {
                                if(RowData != null && RowData.Any(x=>x.CountryCode == unit.Key))
                                {
                                    var plus = RowData.FirstOrDefault(x => x.CountryCode == unit.Key);
                                    plus.Impressions += Convert.ToInt32(unit.Value);
                                    continue;
                                }
                                var insert = new DataApi();
                                insert.CountryCode = unit.Key;
                                insert.Impressions = Convert.ToInt32(unit.Value);
                                RowData.Add(insert);

                            }
                            foreach (var unit in adin["clicks"])
                            {
                                if (RowData != null && RowData.Any(x => x.CountryCode == unit.Key))
                                {
                                    var plus = RowData.FirstOrDefault(x => x.CountryCode == unit.Key);
                                    plus.Clicks += Convert.ToInt32(unit.Value);
                                    continue;
                                }
                                var insert = new DataApi();
                                insert.CountryCode = unit.Key;
                                insert.Clicks = Convert.ToInt32(unit.Value);
                                RowData.Add(insert);
                            }
                            foreach (var unit in adin["revenues"])
                            {
                                if (RowData != null && RowData.Any(x => x.CountryCode == unit.Key))
                                {
                                    var plus = RowData.FirstOrDefault(x => x.CountryCode == unit.Key);
                                    plus.Revenue += Convert.ToInt32(unit.Value);
                                    continue;
                                }
                                var insert = new DataApi();
                                insert.CountryCode = unit.Key;
                                insert.Revenue = Convert.ToInt32(unit.Value);
                                RowData.Add(insert);
                            }
                            foreach (var unit in adin["fillRates"])
                            {
                                if (RowData != null && RowData.Any(x => x.CountryCode == unit.Key))
                                {
                                    var plus = RowData.FirstOrDefault(x => x.CountryCode == unit.Key);
                                    plus.Fillrate += Convert.ToDouble(unit.Value);
                                    continue;
                                }
                                var insert = new DataApi();
                                insert.CountryCode = unit.Key;
                                insert.Fillrate += Convert.ToInt32(unit.Value);
                                RowData.Add(insert);
                            }
                        }
                        if(RowData!=null && RowData.Any())
                        {
                            
                            foreach (var unit in RowData){
                                unit.Revenue = (unit.Revenue  * CTRValue) / 100;
                                unit.CTR = unit.Impressions > 0 ? ((double)unit.Clicks / unit.Impressions) * 100 : 0;
                                unit.Fillrate = unit.Fillrate / totalDays * 100;
                                unit.Ecpm = unit.Revenue / unit.Impressions * 1000;
                            }
                            ListData.Add(new ListDataApi()
                            {
                                Html = BuildStringData(RowData, item, IsAdmin),
                                TotalImpressions = item.Impression == 1 || IsAdmin ? RowData.Sum(x => x.Impressions) : 0,
                                TotalClicks = item.Click == 1 || IsAdmin ? RowData.Sum(x => x.Clicks) : 0,
                                TotalRevenue = item.Revenues == 1 || IsAdmin? RowData.Sum(x => x.Revenue): 0,
                                //TotalFillrate = RowData.Average(x => x.Fillrate),
                                InfoApiId = item.InfoApiId
                            });
                        }
                        else
                        {
                            NoResult.Add(item.InfoApiId);
                        }
                    }
                }
            }
            NoData.NoResult = NoResult;
            NoData.TimeOut = TimeOut;
            if (ListData != null && ListData.Any())
            {
                
                return Json(new
                {
                    Status = 1,
                    NoData = JsonConvert.SerializeObject(NoData),
                    Msg = JsonConvert.SerializeObject(ListData)
                });
            }

            return Json(new
            {
                Status = -1,
                Msg = "Không lấy được dữ liệu.",
                NoData = JsonConvert.SerializeObject(NoData),
            });
        }



        public ActionResult Contact()
        {

            return View();
        }

        private string BuildStringData(List<DataApi> ListData, InfoApi Info, bool IsAdmin)
        {
            var stringRsl = string.Empty;
            foreach (var item in ListData)
            {
                if (IsAdmin)
                {
                    stringRsl += "<tr>";
                    stringRsl += "<td>" + item.CountryCode + "</td>";
                    stringRsl += "<td>" + item.Impressions + "</td>";
                    stringRsl += "<td>" + item.Clicks + "</td>";
                    stringRsl += "<td>" + item.Fillrate.ToString("0.##") + "</td>";
                    stringRsl += "<td>" + item.Revenue + "$</td>";
                    stringRsl += "<td>" + item.CTR.ToString("0.##") + "</td>";
                    if((API)Info.TypeAPI == API.Appodeal)
                    {
                        stringRsl += "<td>" + item.Requests + "</td>";
                        stringRsl += "<td>" + item.Fills + "</td>";
                        stringRsl += "<td>" + item.Views + "</td>";
                    }
                    stringRsl += "<td>" + item.Ecpm.ToString("0.##") + "</td>";
                    stringRsl += "</tr>";
                }
                else
                {
                    stringRsl += "<tr>";
                    stringRsl += "<td>" + item.CountryCode + "</td>";
                    if (Info.Impression == 1)
                        stringRsl += "<td>" + item.Impressions + "</td>";
                    if (Info.Click == 1)
                        stringRsl += "<td>" + item.Clicks + "</td>";
                    if (Info.Fillrate == 1)
                        stringRsl += "<td>" + item.Fillrate.ToString("0.##") + "</td>";
                    if (Info.Revenues == 1)
                        stringRsl += "<td>" + item.Revenue + "$ </td>";
                    if (Info.CTR == 1)
                        stringRsl += "<td>" + item.CTR.ToString("0.##") + "</td>";
                    if ((API)Info.TypeAPI == API.Appodeal)
                    {
                        if (Info.Requests == 1)
                            stringRsl += "<td>" + item.Requests + "</td>";
                        if (Info.Fills == 1)
                            stringRsl += "<td>" + item.Fills + "</td>";
                        if (Info.Views == 1)
                            stringRsl += "<td>" + item.Views + "</td>";
                        
                    }
                    if (Info.Ecpm == 1)
                        stringRsl += "<td>" + item.Ecpm.ToString("0.##") + "</td>";
                    stringRsl += "</tr>";
                }
            }
            return stringRsl;
        }
        
    }
}