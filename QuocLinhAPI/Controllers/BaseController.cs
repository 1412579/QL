using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BUS;
namespace QuocLinhAPI.Controllers
{
    public class BaseController : Controller
    {
        protected bool IsAdminLogged()
        {
            if (Session["user"] == null)
            {
                return false;
            }
            else
            {
                var user = Session["user"] as User;
                if (user.Role == 1)
                    return true;
            }
            return false;
        }

    }
}