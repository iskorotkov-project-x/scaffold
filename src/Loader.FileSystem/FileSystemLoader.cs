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
        private readonly string _pathToPlugins;

        public FileSystemLoader(string pathToTemplates, string pathToPlugins)
        {
            _pathToTemplates = pathToTemplates;
            _pathToPlugins = pathToPlugins;
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
            var dirPath = Path.Join(_pathToTemplates, language, template);
            if (!System.IO.Directory.Exists(dirPath))
            {
                throw new System.Exception($"{dirPath} is not exists!");
            }

            pluginsName = pluginsName.ToList();
            var plugins = new List<Plugin>();
            var pluginPath = _pathToPlugins;

            foreach(var plgName in pluginsName)
            { 
                var tempPlugins = System.IO.Directory.GetFiles(pluginPath, $"{plgName}.*", SearchOption.AllDirectories)
                    .Select(x => new Model.Plugin() { Info = new FileInfo(x) });


                if (tempPlugins.Count() == 0)
                {
                    throw new System.Exception($"plugin {plgName} is not exists!");
                }

                plugins.AddRange(tempPlugins);
            }


            return new Template()
            {
                Language = language,
                Name = template,
                Files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories)
                    .Select(x => new Model.File() { Info = new FileInfo(x) }),

                Plugins = plugins,
                RootDirectory = new DirectoryInfo(dirPath),
            };
        }
    }
}
