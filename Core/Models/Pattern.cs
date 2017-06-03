using System.IO;

namespace Endzone.Umbraco.PatternLib.Core.Models
{
    public class Pattern
    {
        FileInfo file;

        public string Name
        {
            get
            {
                return file.Name;
            }
        }

        public Pattern(FileInfo file)
        {
            this.file = file;
        }
    }
}
