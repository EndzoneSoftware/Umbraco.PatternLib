using System.Linq;
using Umbraco.Web.Mvc;

namespace Endzone.Umbraco.PatternLib.Core.Razor
{
    public class PatternLibRazorViewEngine : RenderViewEngine
    {
        public PatternLibRazorViewEngine() : base()
        {
            // extend the default list of partial views with the PatternLib view path.
            var umbracoPartialViewlocations = PartialViewLocationFormats.ToList();

            umbracoPartialViewlocations.Add($"{Constants.RazorPatternViewsPath}{{0}}.cshtml");

            PartialViewLocationFormats = umbracoPartialViewlocations.ToArray();
        }
    }
}