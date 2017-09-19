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
        private const string RazorPatternViewsPath = PatternViewsPath + "razor/";

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
        public ActionResult Header(bool? hideControls, bool? hidePatternInfoControl, bool? useRazor)
        {
            var patterns = new PatternLibrary();

            var path = Server.MapPath(useRazor.GetValueOrDefault() ? RazorPatternViewsPath : StaticPatternViewsPath);
            
            foreach (var dirPath in Directory.EnumerateDirectories(path))
            {
                var pattern = new Pattern(dirPath, path);

                GetPatternsFromDir(pattern, useRazor.GetValueOrDefault());

                patterns.Add(pattern);
            }

            ViewBag.HideControls = hideControls;
            ViewBag.HidePatternInfoControl = hidePatternInfoControl;
            ViewBag.UseRazor = useRazor;

            return PartialView(ViewTemplate("Partials/_Header"), patterns);
        }

        private static void GetPatternsFromDir(Pattern pattern, bool useRazor)
        {
            // get all patterns in this directory
            foreach (var filePath in Directory.EnumerateFiles(pattern.FullDirectoryPath, (useRazor ? "*.cshtml" : "*.htm"), SearchOption.TopDirectoryOnly))
            {
                // We ignore patterns starting with underscore (_)
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                if (fileNameWithoutExtension != null && (useRazor || !fileNameWithoutExtension.StartsWith("_")))
                {
                    pattern.Add(new Pattern(filePath, pattern.RootPath));
                }
            }

            // get any sub directories and their patterns
            foreach (var dirPath in Directory.EnumerateDirectories(pattern.FullDirectoryPath))
            {
                var dirPattern = new Pattern(dirPath, pattern.RootPath);

                GetPatternsFromDir(dirPattern, useRazor);

                pattern.Add(dirPattern);
            }
        }

        /// <summary>
        /// View the index page of patternlib.
        /// </summary>
        /// <param name="useRazor"></param>
        /// <returns></returns>
        public ActionResult Index(bool useRazor)
        {
            ViewBag.UseRazor = useRazor;

            return View(ViewTemplate("Index"));
        }

        /// <summary>
        /// Returns the static or Razor iframe viewer for this pattern.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="useRazor"></param>
        /// <returns></returns>
        public ActionResult PatternViewer(string path, bool useRazor)
        {
            ViewBag.UseRazor = useRazor;

            var viewsPath = useRazor ? RazorPatternViewsPath : StaticPatternViewsPath;

            var patternPath = Server.MapPath(Path.Combine(viewsPath, path.TrimEnd("/").EnsureEndsWith(useRazor ? ".cshtml" : ".htm")));
            var rootPath = Server.MapPath(viewsPath);

            var pattern = new Pattern(patternPath, rootPath);

            return View(ViewTemplate("PatternViewer"), pattern);
        }

        /// <summary>
        /// Returns the static or Razor HTML for this pattern (wrapped in the PatternMaster).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="useRazor"></param>
        /// <returns></returns>
        public ActionResult Pattern(string path, bool useRazor)
        {
            ViewBag.UseRazor = useRazor;

            var viewsPath = useRazor ? RazorPatternViewsPath : StaticPatternViewsPath;

            var rootPath = Server.MapPath(viewsPath);
            var patternPath = Path.Combine(rootPath, path);
            
            var pattern = new Pattern(patternPath, rootPath);

            if (pattern.IsList)
            {
                // get child patterns
                GetPatternsFromDir(pattern, false);
            }

            var patterMasterPath = Path.Combine(viewsPath, "_PatternMaster.cshtml");

            return View(patterMasterPath, pattern);
        }

        /// <summary>
        /// Returns the Razor view with mocked viewmodel.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult PatternWithModel(string path)
        {
            var patternPath = Path.Combine(RazorPatternViewsPath, path);

            // TODO: create model mock

            return PartialView(patternPath);
        }
    }
}
