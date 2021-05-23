using System.Collections.Generic;

namespace Model
{
    public class Template
    {
        public string Language { get; init; }
        public string TemplateName { get; init; }
        public IEnumerable<File> Files { get; init; }
        public IEnumerable<Plugin> Plugins { get; init; }
    }
}
