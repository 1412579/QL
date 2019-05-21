using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BUS;
namespace QuocLinhAPI.Controllers
{
    public class LoginController : BaseController
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

        public ActionResult Edit(int userid)
        {
            var model = BUS_User.Instance.GetById(userid);
            if(model != null)
                return View(model);
            return RedirectToAction("Listing");
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
                    Status = id,
                    Msg = ""
                });
            }
        }

        [HttpPost]
        public ActionResult UpdateUserByAdmin(string password, int UserId,int Role)
        {
            //if (!IsAdminLogged())
            //    return Redirect("~/");
            var IsValid = BUS_User.Instance.UpdatePasswordAdmin(UserId, password,Role);
            if (IsValid > 0)
            {
                return Json(new
                {
                    Status = 1,
                    Msg = ""
                });
            }
            else
            {
                return Json(new
                {
                    Status = -1,
                    Msg = "Không cập nhật được thông tin tài khoản, vui lòng thử lại sau."
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