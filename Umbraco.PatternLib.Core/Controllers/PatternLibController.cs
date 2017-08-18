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
    [PatternViewerEnabled]
    public class PatternLibController : Controller
    {
        const string viewTemplatePath = "~/App_Plugins/Patternlib/viewer/{0}.cshtml";
        const string defaultViewsFolder = "~/App_Plugins/Patternlib/DefaultViews/";
        const string viewsFolderPath = "~/Views/_patternlib/";
        const string staticPatternsPath = viewsFolderPath + "static/";
        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            EnsureDefaultViews();
        }

        string ViewTemplate(string name)
        {
            return string.Format(viewTemplatePath, name);
        }

        public ActionResult Index()
        {
            return View(ViewTemplate("index"));
        }

        /// <summary>
        /// View the static (ie markup only) patternlib
        /// </summary>
        /// <returns></returns>
        public ActionResult Static(string path)
        {
            var patternUrl = String.IsNullOrEmpty(path) ? null : "/patternlib/pattern/" + path;
            ViewBag.PatternUrl = patternUrl;
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

        private void EnsureDefaultViews()
        {
            string destination = Server.MapPath(viewsFolderPath);
            if (!Directory.Exists(destination))
            {
                string source = Server.MapPath(defaultViewsFolder);

                int pathLen = source.Length;

                foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                {
                    string subPath = dirPath.Substring(pathLen);
                    string newpath = Path.Combine(destination, subPath);
                    Directory.CreateDirectory(newpath);
                }

                foreach (string filePath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                {
                    string subPath = filePath.Substring(pathLen);
                    string newpath = Path.Combine(destination, subPath);
                    System.IO.File.Copy(filePath, newpath);
                }
            }
        }

        private static void GetPatternsFromDir(string rootD, string d, Pattern patternDir)
        {
            foreach (var f in Directory.EnumerateFiles(d, "*.htm", SearchOption.TopDirectoryOnly))
            {
                //We ignore patterns starting with underscore (_)
                if (!Path.GetFileNameWithoutExtension(f).StartsWith("_"))
                {
                    patternDir.Add(new Pattern(f, rootD));
                }
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
