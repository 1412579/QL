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

        #region Đăng nhập
        public ActionResult Index()
        {
            if (Logon())
                return Redirect("~/");
            return View();
        }


        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            if (Logon())
                return Redirect("~/");
            var user = BUS_User.Instance.CheckLogin(username, password);
            if (user != null)
            {
                Session["user"] = user;
                return Redirect("~/");
            }
            else
            {
                ViewBag.Msg = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                user = new User()
                {
                    UserName = username,
                    Password = password
                };
                return View(user);
            }
        }

        #endregion

        #region Quản lý user của Admin

        public ActionResult AddEdit()
        {
            if (!IsAdminLogged())
                return Redirect("~/");
            return View();
        }

        public ActionResult Listing()
        {
            if (!IsAdminLogged())
                return Redirect("~/");
            var model = BUS_User.Instance.GetAll();
            return View(model);
        }

        public ActionResult Edit(int userid)
        {
            if (!IsAdminLogged())
                return Redirect("~/");
            var model = BUS_User.Instance.GetById(userid);
            if (model != null)
            {
                ViewBag.ListApi = BUS_API.Instance.GetAllByUserId(model.UserId);
                return View(model);
            }
            return RedirectToAction("Listing");
        }


        [HttpPost]
        public ActionResult AddEdit(string username, string password, int IsAdmin)
        {
            if (!IsAdminLogged())
                return Json(new
                {
                    Status = -1,
                    Msg = "Không có quyền truy cập."
                });
            var user = BUS_User.Instance.CheckExistUsername(username);
            if (user > 0)
            {
                Session["user"] = user;
                return Json(new
                {
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
        public ActionResult UpdateUserByAdmin(string password, int UserId, int Role)
        {
            if (!IsAdminLogged())
                return Json(new
                {
                    Status = -1,
                    Msg = "Không có quyền truy cập."
                });
            var IsValid = BUS_User.Instance.UpdatePasswordAdmin(UserId, password, Role);
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


        [HttpPost]
        public ActionResult AddAPI(int UserId,int TypeAPI,string Token,int UserIdAPI,string Container, int Impression, int Click, int CTR, int Revenues, int Fillrate)
        {
            if (!IsAdminLogged())
                return Json(new
                {
                    Status = -1,
                    Msg = "Không có quyền truy cập."
                });
                var dto = new InfoApi()
                {
                    UserId = UserId,
                    TypeAPI = TypeAPI,
                    Token = Token,
                    UserIdApi = UserIdAPI,
                    Container = Container,
                    Impression = Impression,
                    Click = Click,
                    CTR = CTR,
                    Revenues = Revenues,
                    Fillrate = Fillrate
                };
                var id = BUS_API.Instance.Insert(dto);
                return Json(new
                {
                    Status = id,
                    Msg = ""
                });
        }

        #endregion

        public ActionResult IsLogged()
        {
            if (Session["user"] == null)
            {
                Response.Redirect("~/Login");
            }
            return Content("");
        }

    }
}