using System.IO;
using System.Text.RegularExpressions;

namespace Model
{
    /// <summary>
    /// Custom Plugin, that include standart FileInfo. Method ContainsTemplates need for testing
    /// </summary>
    public class Plugin
    {
        public FileInfo Info { get; init; }

        /// <summary>
        /// Check if there's any unreplaced template with format {{ .Something }}
        /// </summary>
        /// <returns></returns>
        public bool ContainsTemplates()
        {
            var text = System.IO.File.ReadAllText(Info.FullName);
            return Regex.IsMatch(text, @"{{ \..* }}");
        }
    }
}
