using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Endzone.Umbraco.PatternLib.Core.Controllers
{
    public class PatternLibController : Controller
    {
        const string viewTemplatePath = "~/Views/_patternlib/viewer/{0}.cshtml";

        string ViewTemplate(string name)
        {
            return string.Format(viewTemplatePath, name);
        }

        /// <summary>
        /// View the static (ie markup only) patternlib
        /// </summary>
        /// <returns></returns>
        public ActionResult Static()
        {
            return View(ViewTemplate("static"));
        }

        [ChildActionOnly]
        public ActionResult PatternNav()
        {
            return View(ViewTemplate("_patternNavBar"));
        }
    }
}
