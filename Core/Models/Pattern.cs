using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Endzone.Umbraco.PatternLib.Core.Models
{
    public class PatternLibrary : List<Pattern>
    {

    }
    public class Pattern
    {
        /// <summary>
        /// The full path to the directory or file
        /// </summary>
        public string FullPath { get; }
        /// <summary>
        /// The base path to the web "root" for the patterns directory
        /// </summary>
        public string RootPath { get; }

        public bool IsList
        {
            get
            {
                return Path.GetExtension(this.FullPath) == String.Empty;
            }
        }

        public string Name
        {
            get
            {
                string fileName = Path.GetFileNameWithoutExtension(this.FullPath);
                //lets ignore prefix numbers (used for ordering only) in format 00-
                return Regex.Replace(fileName, "^[0-9]{0,2}[-]{0,1}", "");
            }
        }

        public string LinkPath
        {
            get
            {
                return FullPath.Replace(RootPath, "");
            }
        }

        public string Code
        {
            get
            {
                return File.ReadAllText(FullPath);
            }
        }

        public List<Pattern> Patterns { get; }

        public Pattern(string path, string rootPath)
        {
            this.FullPath = path;
            this.RootPath = rootPath;
            Patterns = new List<Pattern>();
        }

        public void Add(Pattern p)
        {
            Patterns.Add(p);
        }
    }
}
