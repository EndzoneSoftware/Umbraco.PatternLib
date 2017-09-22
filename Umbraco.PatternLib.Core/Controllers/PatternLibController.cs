using Endzone.Umbraco.PatternLib.Core.Attributes;
using Endzone.Umbraco.PatternLib.Core.Models;
using Endzone.Umbraco.PatternLib.Core.Razor;
using System.IO;
using System.Web.Mvc;
using Endzone.Umbraco.PatternLib.Core.Mocks;
using Umbraco.Core;

namespace Endzone.Umbraco.PatternLib.Core.Controllers
{
    [PatternLibEnabled]
    [EnsureDefaultViews]
    public class PatternLibController : Controller
    {
        private static string ViewTemplate(string name)
        {
            return string.Format(Constants.ViewsTemplatePath, name);
        }

        [ChildActionOnly]
        public ActionResult Header(bool? hideControls, bool? hidePatternInfoControl, bool? useRazor)
        {
            var patterns = new PatternLibrary();

            var path = Server.MapPath(useRazor.GetValueOrDefault() ? Constants.RazorPatternViewsPath : Constants.StaticPatternViewsPath);
            
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

            var viewsPath = useRazor ? Constants.RazorPatternViewsPath : Constants.StaticPatternViewsPath;

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

            var viewsPath = useRazor ? Constants.RazorPatternViewsPath : Constants.StaticPatternViewsPath;

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
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult PatternWithModel(string path, string code)
        {
            object model = null;

            var patternPath = Path.Combine(Constants.RazorPatternViewsPath, path);

            var modelTypeName = RazorParser.GetModelTypeName(code);

            if (!string.IsNullOrEmpty(modelTypeName))
            {
                var modelType = TypeFinder.GetTypeByName(modelTypeName);

                if (modelType != null)
                {
                    // get the full file path of the mock data JSON file
                    var patternDataPath = Server.MapPath(patternPath.Replace(".cshtml", ".json"));

                    model = MockBuilder.Create(modelType, patternDataPath);
                }
            }

            return PartialView(patternPath, model);
        }
    }
}
