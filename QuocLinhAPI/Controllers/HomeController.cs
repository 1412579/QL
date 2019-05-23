using System;
using System.Collections.Generic;
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
            if(user == null)
                return Redirect("~/Login");
            var model = new List<InfoApi>();
            if (user.Role == (int)Role.User)
                model = BUS_API.Instance.GetAllByUserId(user.UserId);
            else model = BUS_API.Instance.GetAll();
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
            if(model.TrickAvarageId > 0)
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
            if(user == null)
                return Json(new
                {
                    Status = -1,
                    Msg = "Không có quyền truy cập."
                });

            if ((Role)user.Role == Role.Admin)
            {
                var model = BUS_API.Instance.GetAll();
                if (!string.IsNullOrEmpty(ListSelected))
                {
                    model = model.Where(x => ListSelected.IndexOf(x.InfoApiId.ToString()) >= 0).ToList();
                }
                foreach (var item in model)
                {
                    if((API)item.TypeAPI == API.Appodeal)
                    {
                        var url = Helpers.Instance.GetApiLink(item.TypeAPI.Value, item.Token, item.Container, start, end, item.UserIdApi.Value);
                        var objTask = Helpers.Instance.GetJSON(url);
                        if(objTask != null)
                        {
                            var TaskId = 0;
                            if (objTask["task_id"] != null)
                              TaskId = Convert.ToInt32(objTask["task_id"]);
                            if(TaskId > 0)
                            {
                                url = Helpers.Instance.GetApiLink(item.TypeAPI.Value, item.Token, item.Container, start, end, item.UserIdApi.Value,TypeAppodeal.Output,TaskId);
                                Thread.Sleep(2000);
                                var rsl = Helpers.Instance.GetJSON(url);
                                if(rsl != null && rsl["code"] == 0 && rsl["message"] == "success")
                                {
                                    return Json(new
                                    {
                                        Status = 1,
                                        Msg = JsonConvert.SerializeObject(rsl)
                                    });
                                }

                            }
                        }
                    }
                }
            }

            return Json(new
            {
                Status = -1,
                Msg = "Không lấy được dữ liệu."
            });
        }



        public ActionResult Contact()
        {

            return View();
        }

        private string BuildStringData()
        {
            var rsl = string.Empty;

            return rsl;
        }

    }
}