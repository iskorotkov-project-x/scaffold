using Business;
using Model;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace Loader.FileSystem
{
    public class FileSystemLoader : ILoader
    {
        private readonly string _pathToTemplates;

        public FileSystemLoader(string pathToTemplates)
        {
            _pathToTemplates = pathToTemplates;
        }

        public IEnumerable<TemplateInfo> GetAllLanguagesAndTemplateNames()
        {
            var di = new DirectoryInfo(_pathToTemplates);
            if (!di.Exists)
            {
                return null;
            }

            var templateInfos = new List<TemplateInfo>();
            foreach (var lang in di.GetDirectories())
            {
                foreach (var template in lang.GetDirectories())
                {
                    templateInfos.Add(new TemplateInfo {Language = lang.Name, TemplateName = template.Name});
                }
            }

            return templateInfos;
        }

        public Template Load(string language, string template)
        {
            throw new System.NotImplementedException();
        }
    }
}
