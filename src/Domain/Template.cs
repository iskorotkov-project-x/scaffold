using System.Collections.Generic;
using System.IO;

namespace Model
{
    /// <summary>
    /// Template contains all info about template, files and plugins used by it
    /// </summary>
    public class Template : TemplateInfo
    {
        public DirectoryInfo RootDirectory { get; init; }

        // All files of template
        public IEnumerable<File> Files { get; init; }
        public IEnumerable<Plugin> Plugins { get; init; }
    }
}
