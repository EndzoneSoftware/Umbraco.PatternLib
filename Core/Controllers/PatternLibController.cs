using Endzone.Umbraco.PatternLib.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Endzone.Umbraco.PatternLib.Core.Controllers
{
    public class PatternLibController : Controller
    {
        const string viewTemplatePath = "~/Views/_patternlib/viewer/{0}.cshtml";
        const string staticPatternsPath = "~/Views/_patternlib/patterns/";

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
            //todo ensure that these dirs exist
            var path = Server.MapPath(staticPatternsPath);
            var patterns = new PatternLibrary();
            foreach (var d in Directory.EnumerateDirectories(path))
            {
                var patternDir = new Pattern(d);
                GetPatternsFromDir(d, patternDir);
                patterns.Add(patternDir);
            }
            return View(ViewTemplate("_patternNavBar"), patterns);
        }

        private static void GetPatternsFromDir(string d, Pattern patternDir)
        {
            foreach (var f in Directory.EnumerateFiles(d, "*.htm", SearchOption.TopDirectoryOnly))
            {
                patternDir.Add(new Pattern(f));
            }
            foreach (var subDir in Directory.EnumerateDirectories(d))
            {
                var pattern = new Pattern(subDir);
                GetPatternsFromDir(subDir, pattern);
                patternDir.Add(pattern);
            }
        }
    }
}
