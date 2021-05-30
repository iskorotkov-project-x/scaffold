using Model;
using System.Collections.Generic;

namespace Business
{
    public interface ILoader
    {
        /// <summary>
        /// Loading files for template
        /// </summary>
        /// <param name="language">Programming language name</param>
        /// <param name="template">Template name</param>
        /// <param name="plugins">List with loaded plugins names</param>
        /// <returns></returns>
        Template Load(string language, string template, IEnumerable<string> plugins);

        /// <summary>
        /// Get all available pairs of language and template
        /// </summary>
        /// <returns></returns>
        IEnumerable<TemplateInfo> GetAllLanguagesAndTemplateNames();
    }
}
