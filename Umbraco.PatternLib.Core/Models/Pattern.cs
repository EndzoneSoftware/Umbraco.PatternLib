using MarkdownSharp;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Umbraco.Core;

namespace Endzone.Umbraco.PatternLib.Core.Models
{
    public class PatternLibrary : List<Pattern>
    {
    }

    public class Pattern
    {
        /// <summary>
        /// Creates new instance of Pattern class and sets pattern paths.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rootPath"></param>
        public Pattern(string path, string rootPath)
        {
            FullPath = path;
            RootPath = rootPath;
            RelativePath = FullPath.Replace(RootPath, string.Empty);
            Patterns = new List<Pattern>();
        }

        /// <summary>
        /// The full path to the directory or file.
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// The root path of the directory or file.
        /// </summary>
        public string RootPath { get; }

        /// <summary>
        /// The relative path to the directory or file.
        /// </summary>
        private string RelativePath { get; }

        /// <summary>
        /// If this is a directory containing files.
        /// </summary>
        public bool IsList => Path.GetExtension(FullPath) == string.Empty;

        /// <summary>
        /// Name of the directory or pattern file.
        /// </summary>
        public string Name
        {
            get
            {
                var fileName = Path.GetFileNameWithoutExtension(FullPath);

                //lets ignore prefix numbers (used for ordering only) in format 00-
                return fileName == null ? null : Regex.Replace(fileName, "^[0-9]{0,2}[-]{0,1}", "");
            }
        }

        /// <summary>
        /// Link to the static pattern file.
        /// </summary>
        public string StaticUrl => "/patternlib/static/" + RelativePath.Replace(@"\", "/");

        /// <summary>
        /// Link to the static pattern viewer.
        /// </summary>
        public string StaticViewerUrl => StaticUrl.Replace(".htm", string.Empty).EnsureEndsWith("/");

        /// <summary>
        /// Link to the Razor pattern file.
        /// </summary>
        public string RazorUrl => "/patternlib/razor/" + RelativePath.Replace(@"\", "/");

        /// <summary>
        /// Link to the static pattern viewer.
        /// </summary>
        public string RazorViewerUrl => RazorUrl.Replace(".cshtml", string.Empty).EnsureEndsWith("/");

        /// <summary>
        /// Get raw file content.
        /// </summary>
        public MvcHtmlString Code => IsList ? MvcHtmlString.Empty : new MvcHtmlString(File.ReadAllText(FullPath));

        /// <summary>
        /// Get pattern notes from Markdown file.
        /// </summary>
        public MvcHtmlString Notes
        {
            get
            {
                if (IsList)
                {
                    return MvcHtmlString.Empty;
                }

                var markdownPath = FullPath.Replace(".htm", ".md");

                if (!File.Exists(markdownPath))
                {
                    return MvcHtmlString.Empty;
                }

                var markdownText = File.ReadAllText(markdownPath);

                // convert Markdown to HTML
                var md = new Markdown();

                var markdownHtml = md.Transform(markdownText);

                return new MvcHtmlString(markdownHtml);
            }
        } 

        /// <summary>
        /// List of child pattern files or directories.
        /// </summary>
        public List<Pattern> Patterns { get; }

        /// <summary>
        /// Adds a new pattern to the list.
        /// </summary>
        /// <param name="p"></param>
        public void Add(Pattern p)
        {
            Patterns.Add(p);
        }
    }
}
