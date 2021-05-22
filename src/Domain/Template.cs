using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Template contains all info about template, files and plugins used by it
    /// </summary>
    public class Template : TemplateInfo
    {
        // All files of template
        public IEnumerable<File> Files { get; init; }
        public IEnumerable<Plugin> Plugins { get; init; }
    }
}
