using System.Collections.Generic;

namespace Model
{
    public class Template
    {
        public string Language { get; init; }
        public string Name { get; init; }
        public IEnumerable<File> Files { get; init; }
        public IEnumerable<Plugin> Plugins { get; init; }
    }
}
