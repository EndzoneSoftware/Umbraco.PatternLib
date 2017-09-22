using System.IO;
using System.Text.RegularExpressions;

namespace Endzone.Umbraco.PatternLib.Core.Razor
{
    internal class RazorParser
    {
        private static readonly Regex RegexModel = new Regex(@"^\@model ([A-Za-z0-9\.]*)$");
        private static readonly Regex RegexInherits = new Regex(@"^\@inherits (.+?(?=\<))\<([A-Za-z0-9\.]+?(?=\>))\>$");

        /// <summary>
        /// Get the Razor model type expected for this partial.
        /// </summary>
        /// <param name="razor"></param>
        /// <returns></returns>
        public static string GetModelTypeName(string razor)
        {
            if (string.IsNullOrEmpty(razor))
            {
                return null;
            }

            string modelTypeName = null;

            // read through partial line by line and find model specification
            using (var reader = new StringReader(razor))
            {
                bool found = false;
                string line;

                while (!found && (line = reader.ReadLine()) != null)
                {
                    if (RegexModel.IsMatch(line))
                    {
                        // partial uses @model syntax
                        modelTypeName = RegexModel.Match(line).Groups[1].Value;

                        found = true;
                    }
                    else if (RegexInherits.IsMatch(line))
                    {
                        // partial uses @inherits syntax
                        modelTypeName = RegexInherits.Match(line).Groups[2].Value;

                        found = true;
                    }
                }
            }

            return modelTypeName;
        }
    }
}
