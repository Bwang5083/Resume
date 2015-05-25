using finalExam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace finalExam.Controllers
{
    public class RemoteController : Controller
    {
        // GET: Remote
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ProvinceCodeValidation(string provinceCode)
        {
            OECContext db = new OECContext();

            if (provinceCode.Length != 2)
                return Json("Province Code must be exactly 2 letters long", JsonRequestBehavior.AllowGet);

            try
            {
                var ProCode = db.province.Find(provinceCode);
                if (ProCode == null)
                    return Json("Province Code must be on file!", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "error validating province code" + ex.GetBaseException().Message);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
