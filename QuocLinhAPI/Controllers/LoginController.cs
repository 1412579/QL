using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BUS;
namespace QuocLinhAPI.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult AddEdit()
        {

            return View();
        }

        public ActionResult Listing()
        {
            var model = BUS_User.Instance.GetAll();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string username, string password, string url)
        {
            var user = BUS_User.Instance.CheckLogin(username, password);
            if (user != null)
            {
                Session["user"] = user;
                return Json("1");
            }
            else
            {
                return Json("0");
            }
        }

        [HttpPost]
        public ActionResult AddEdit(string username, string password, int IsAdmin)
        {
            var user = BUS_User.Instance.CheckExistUsername(username);
            if (user > 0)
            {
                Session["user"] = user;
                return Json(new {
                    Status = -1,
                    Msg = "Tên tài khoản đã tồn tại."
                });
            }
            else
            {
                var dto = new User()
                {
                    UserName = username,
                    Password = password,
                    Role = IsAdmin
                };
                var id = BUS_User.Instance.Insert(dto);
                return Json(new
                {
                    Status = 1,
                    Msg = ""
                });
            }
        }

        public ActionResult IsLogged()
        {
            //if (Session["user"] == null)
            //{
            //    Response.Redirect("~/Login");
            //}
            return Content("");
        }

        

    }
}