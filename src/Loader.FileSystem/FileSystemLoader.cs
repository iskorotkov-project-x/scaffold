using Business;
using Model;
using System;
using System.Linq;
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
                throw new System.Exception($"{_pathToTemplates} is not exists!");
            }

            var templateInfos = new List<TemplateInfo>();
            foreach (var lang in di.GetDirectories())
            {
                foreach (var template in lang.GetDirectories())
                {
                    templateInfos.Add(new TemplateInfo { Language = lang.Name, Name = template.Name });
                }
            }

            return templateInfos;
        }

        public Template Load(string language, string template)
        {
            var dirPath = $"{_pathToTemplates}/{language}/{template}";
            if (!new DirectoryInfo(dirPath).Exists)
            {
                throw new System.Exception($"{dirPath} is not exists!");
            }

            return new Template()
            {
                Language = language,
                Name = template,
                Files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories)
                    .Select(x => new Model.File() { Info = new FileInfo(x) }),
            };
        }
    }
}
