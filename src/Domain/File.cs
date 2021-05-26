using System.IO;
using System.Text.RegularExpressions;

namespace Model
{
    /// <summary>
    /// Custom File, that include standart FileInfo. Method ContainsTemplates need for testing
    /// </summary>
    public class File
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
