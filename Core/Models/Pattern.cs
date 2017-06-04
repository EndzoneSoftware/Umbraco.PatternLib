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
                //lets ignore prefix numbers (used for ordering only)
                return Regex.Replace(fileName, "^[0-9]{0,2}[-]{0,1}", "");
            }
        }

        public List<Pattern> Patterns { get; }

        public Pattern(string path)
        {
            this.FullPath = path;
            Patterns = new List<Pattern>();
        }

        public void Add(Pattern p)
        {
            Patterns.Add(p);
        }
    }
}
