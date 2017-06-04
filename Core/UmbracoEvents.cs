using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Web.Mvc;

namespace Endzone.Umbraco.PatternLib.Core
{

    /// <summary>
    /// Registers site specific Umbraco application event handlers
    /// </summary>
    public class UmbracoEvents : ApplicationEventHandler
    {

        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);


            //Replace the Umbraco RenderViewEngine with PatternLibRazorViewEngine
            // this allows us to override View locations without too many extra steps
            IViewEngine remove = null;
            ViewEngines.Engines.ForEach(x => {
                if (x.GetType() == typeof(RenderViewEngine))
                    remove = x;
            });
            if (remove != null)
                ViewEngines.Engines.Remove(remove);

            ViewEngines.Engines.Add(new PatternLibRazorViewEngine());
            
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //map routes for patternlab viewer
            RouteTable.Routes.MapRoute(
                name: "patternlib",
                url: "patternlib/{action}",
                defaults: new { controller = "PatternLib" });
            RouteTable.Routes.MapRoute(
                name: "patternlib.pattern",
                url: "patternlib/pattern/{*path}",
                defaults: new { controller = "PatternLib", action = "Pattern" }
            );
        }

    }
}