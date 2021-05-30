using System;
using Business;
using Model;
using System.IO;
using System.Collections.Generic;

namespace Generator.Local
{
    public class LocalGenerator : IGenerator
    {
        public Project Generate(string pathToProject, Template template)
        {
            var createdFiles = new List<Model.File>();
            var createdDirectories = new List<Model.Directory>();
            foreach (var file in template.Files)
            {
                if (file.Info.Name == "template.yml")
                {
                    continue;
                }

                // delete root directory from file name
                var tempFilePath = file.Info.FullName.Replace($"{template.RootDirectory.FullName}", "");

                var newFileName =  Path.Join(pathToProject, tempFilePath);

                var fi = new FileInfo(newFileName);

                // if there is no directory
                fi.Directory.Create();

                // copy file
                file.Info.CopyTo(newFileName);

                createdFiles.Add(new Model.File {Info = fi});
            }

            foreach (var file in template.Plugins)
            {
                // make only file name. ...\fileName.txt -> fileName.txt
                var shortFilePath = Path.GetFileName(file.Info.FullName);
                shortFilePath = Path.Join($"{{{{ .{nameof(Context.ProjectName)} }}}}", shortFilePath);

                var newFileName = Path.Join(pathToProject, shortFilePath);

                var fi = new FileInfo(newFileName);

                // if there is no directory
                fi.Directory.Create();

                // copy file
                file.Info.CopyTo(newFileName);

                createdFiles.Add(new Model.File { Info = fi });
            }

            return new Project

            {
                CreatedFiles = createdFiles
            };
        }
    }
}
