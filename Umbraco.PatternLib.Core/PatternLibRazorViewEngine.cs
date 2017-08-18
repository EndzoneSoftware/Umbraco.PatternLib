using System.Linq;
using Umbraco.Web.Mvc;

namespace Endzone.Umbraco.PatternLib.Core
{
    public class PatternLibRazorViewEngine : RenderViewEngine
    {
        public PatternLibRazorViewEngine() : base()
        {
            const string patternLibViewFolder = "~/Views/_patterns";

           /* 
           * in the future we might support page templates in patternlib?

            var umbracoViewlocations = ViewLocationFormats.ToList();
            var patternLibViewLocations = umbracoViewlocations.Select(x => x.Replace("~/Views",patternLibViewFolder) ).ToArray();
            umbracoViewlocations.InsertRange(0, patternLibViewLocations);

            ViewLocationFormats = umbracoViewlocations.ToArray();
            */

            var umbracoPartialViewlocations = PartialViewLocationFormats.ToList();
            var patternLibPartialViewLocations = umbracoPartialViewlocations.Select(x => x.Replace("~/Views", patternLibViewFolder) ).ToArray();
            umbracoPartialViewlocations.AddRange(patternLibPartialViewLocations);

            PartialViewLocationFormats = umbracoPartialViewlocations.ToArray();
        }
    }
}