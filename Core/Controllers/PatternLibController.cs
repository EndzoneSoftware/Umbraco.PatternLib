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
        
        /// <summary>
        /// View a specific pattern
        /// </summary>
        /// <returns></returns>
        public ActionResult Pattern(string path)
        {
            var rootPath = Server.MapPath(staticPatternsPath);
            var fullPath = Path.Combine(rootPath, path);
            var pattern = new Pattern(fullPath, rootPath);
            return View(ViewTemplate("pattern"), pattern);
        }



        [ChildActionOnly]
        public ActionResult PatternNav()
        {
            //todo ensure that these dirs exist
            var path = Server.MapPath(staticPatternsPath);
            var patterns = new PatternLibrary();
            foreach (var d in Directory.EnumerateDirectories(path))
            {
                var patternDir = new Pattern(d, path);
                GetPatternsFromDir(path, d, patternDir);
                patterns.Add(patternDir);
            }
            return View(ViewTemplate("_patternNavBar"), patterns);
        }

        private static void GetPatternsFromDir(string rootD, string d, Pattern patternDir)
        {
            foreach (var f in Directory.EnumerateFiles(d, "*.htm", SearchOption.TopDirectoryOnly))
            {
                patternDir.Add(new Pattern(f, rootD));
            }
            foreach (var subDir in Directory.EnumerateDirectories(d))
            {
                var pattern = new Pattern(subDir, rootD);
                GetPatternsFromDir(rootD, subDir, pattern);
                patternDir.Add(pattern);
            }
        }
    }
}
