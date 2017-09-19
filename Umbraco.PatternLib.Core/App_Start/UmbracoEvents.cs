using Endzone.Umbraco.PatternLib.Core.Razor;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Web.Mvc;

namespace Endzone.Umbraco.PatternLib.Core.App_Start
{
    /// <summary>
    /// Registers site specific Umbraco application event handlers
    /// </summary>
    public class UmbracoEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            // Replace the Umbraco RenderViewEngine with PatternLibRazorViewEngine.
            // This allows us to override View locations without too many extra steps.
            ViewEngines.Engines.RemoveAll(x => x is RenderViewEngine);
            ViewEngines.Engines.Add(new PatternLibRazorViewEngine());
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            RouteTable.Routes.MapRoute(
                name: "patternlib",
                url: "patternlib/",
                defaults: new { controller = "PatternLib", action = "Index", useRazor = false }
            );

            RouteTable.Routes.MapRoute(
                name: "patternlib.header",
                url: "patternlib/header",
                defaults: new { controller = "PatternLib", action = "Header" }
            );

            RouteTable.Routes.MapRoute(
                name: "patternlib.static.pattern.only",
                url: "patternlib/static/{*path}",
                constraints: new { path = @".+\.htm$" },
                defaults: new { controller = "PatternLib", action = "Pattern", useRazor = false }
            );

            RouteTable.Routes.MapRoute(
                name: "patternlib.static.pattern.viewer",
                url: "patternlib/static/{*path}",
                constraints: new { path = @".+(?<!\.htm)$" },
                defaults: new { controller = "PatternLib", action = "PatternViewer", useRazor = false }
            );

            RouteTable.Routes.MapRoute(
                name: "patternlib.razor",
                url: "patternlib/razor/",
                defaults: new { controller = "PatternLib", action = "Index", useRazor = true }
            );

            RouteTable.Routes.MapRoute(
                name: "patternlib.razor.pattern.model",
                url: "patternlib/razor/patternwithmodel",
                defaults: new { controller = "PatternLib", action = "PatternWithModel" }
            );

            RouteTable.Routes.MapRoute(
                name: "patternlib.razor.pattern.only",
                url: "patternlib/razor/{*path}",
                constraints: new { path = @".+\.cshtml$" },
                defaults: new { controller = "PatternLib", action = "Pattern", useRazor = true }
            );

            RouteTable.Routes.MapRoute(
                name: "patternlib.razor.pattern.viewer",
                url: "patternlib/razor/{*path}",
                constraints: new { path = @".+(?<!\.cshtml)$" },
                defaults: new { controller = "PatternLib", action = "PatternViewer", useRazor = true }
            );
        }

    }
}