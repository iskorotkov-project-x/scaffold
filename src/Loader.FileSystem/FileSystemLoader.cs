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



        public Template Load(string language, string template, IEnumerable<string> pluginsName)
        {
            var filePath = Path.Join(_pathToTemplates, language, template);
            if (!Directory.Exists(filePath))
            {
                throw new System.Exception($"{filePath} is not exists!");
            }


            pluginsName = (List<string>)pluginsName;
            var plugins = new List<Plugin>();
            var pluginPath = Path.Join(_pathToTemplates, "plugins");

            foreach(var plgName in pluginsName)
            { 
                var tempPlugins = Directory.GetFiles(pluginPath, $"{plgName}.*", SearchOption.AllDirectories)
                    .Select(x => new Model.Plugin() { Info = new FileInfo(x) });


                if (tempPlugins.Count<Plugin>() == 0)
                {
                    throw new System.Exception($"plugin {plgName} is not exists!");
                }

                plugins.AddRange(tempPlugins);
            }


            return new Template()
            {
                Language = language,
                Name = template,
                Files = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories)
                    .Select(x => new Model.File() { Info = new FileInfo(x) }),

                Plugins = plugins,
                RootDirectory = new DirectoryInfo(filePath),
            };
        }
    }
}
