using System.IO;
using System.Web.Mvc;

namespace Endzone.Umbraco.PatternLib.Core.Attributes
{
    /// <summary>
    /// Ensures that default views are copied across.
    /// </summary>
    internal class EnsureDefaultViewsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string destination = filterContext.HttpContext.Server.MapPath(Constants.PatternViewsPath);

            if (Directory.Exists(destination))
            {
                return;
            }

            string source = filterContext.HttpContext.Server.MapPath(Constants.DefaultViewsFolder);

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
                File.Copy(filePath, newpath);
            }
        }
    }
}