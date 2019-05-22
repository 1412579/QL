using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BUS;
namespace QuocLinhAPI.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var json = Helpers.Instance.GetJSON("https://api.adincube.com/api/1.3/public/reporting/overview?start=2019-05-05&end=2019-05-15&auth_token=e59658c1967040aa82333e5e25533caf");
            return View();
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

        public ActionResult Contact()
        {

            return View();
        }
    }
}