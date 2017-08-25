using Endzone.Umbraco.PatternLib.Core.Models;
using System.IO;
using System.Web.Mvc;
using Umbraco.Core;

namespace Endzone.Umbraco.PatternLib.Core.Controllers
{
    [PatternViewerEnabled]
    public class PatternLibController : Controller
    {
        private const string ViewsTemplatePath = "~/App_Plugins/Umbraco.PatternLib/Views/{0}.cshtml";
        private const string DefaultViewsFolder = "~/App_Plugins/Umbraco.PatternLib/DefaultViews/";

        private const string PatternViewsPath = "~/Views/_patternlib/";
        private const string StaticPatternViewsPath = PatternViewsPath + "static/";

        private static string ViewTemplate(string name)
        {
            return string.Format(ViewsTemplatePath, name);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            EnsureDefaultViews();
        }

        private void EnsureDefaultViews()
        {
            string destination = Server.MapPath(PatternViewsPath);

            if (Directory.Exists(destination))
            {
                return;
            }

            string source = Server.MapPath(DefaultViewsFolder);

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

        [ChildActionOnly]
        public ActionResult Header(bool? hideControls)
        {
            var patterns = new PatternLibrary();

            var path = Server.MapPath(StaticPatternViewsPath);
            
            foreach (var dirPath in Directory.EnumerateDirectories(path))
            {
                var pattern = new Pattern(dirPath, path);

                GetPatternsFromDir(pattern);

                patterns.Add(pattern);
            }

            ViewBag.HideControls = hideControls;

            return PartialView(ViewTemplate("Partials/_Header"), patterns);
        }

        private static void GetPatternsFromDir(Pattern pattern)
        {
            // get all patterns in this directory
            foreach (var filePath in Directory.EnumerateFiles(pattern.FullPath, "*.htm", SearchOption.TopDirectoryOnly))
            {
                // We ignore patterns starting with underscore (_)
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                if (fileNameWithoutExtension != null && !fileNameWithoutExtension.StartsWith("_"))
                {
                    pattern.Add(new Pattern(filePath, pattern.RootPath));
                }
            }

            // get any sub directories and their patterns
            foreach (var dirPath in Directory.EnumerateDirectories(pattern.FullPath))
            {
                var dirPattern = new Pattern(dirPath, pattern.RootPath);

                GetPatternsFromDir(dirPattern);

                pattern.Add(dirPattern);
            }
        }

        /// <summary>
        /// View the index page of patternlib.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(ViewTemplate("Index"));
        }

        /// <summary>
        /// Returns the static iframe viewer for this pattern.
        /// </summary>
        /// <returns></returns>
        public ActionResult StaticViewer(string path)
        {
            var patternPath = Server.MapPath(Path.Combine(StaticPatternViewsPath, path.TrimEnd("/").EnsureEndsWith(".htm")));
            var rootPath = Server.MapPath(StaticPatternViewsPath);

            var pattern = new Pattern(patternPath, rootPath);

            return View(ViewTemplate("Pattern"), pattern);
        }

        /// <summary>
        /// Returns the static HTML for this pattern (wrapped in the PatternMaster).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult Static(string path)
        {
            var rootPath = Server.MapPath(StaticPatternViewsPath);
            var patternPath = Path.Combine(rootPath, path);

            var pattern = new Pattern(patternPath, rootPath);

            var patterMasterPath = Path.Combine(StaticPatternViewsPath, "_PatternMaster.cshtml");

            return View(patterMasterPath, pattern);
        }
    }
}
