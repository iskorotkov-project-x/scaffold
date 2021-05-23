using Business;
using Model;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Templater.Regex
{
    /// <summary>
    /// Templater use regex
    /// </summary>
    public class RegexTemplater : ITemplater
    {
        public Project Substitute(Context ctx, Project project)
        {
            var processedFiles = new List<Model.File>();
            if (project.ProcessedFiles != null)
            {
                processedFiles.AddRange(project.ProcessedFiles);
            }

            var tempCreatedFiles = project.CreatedFiles.ToList();
            for (int i = 0; i < tempCreatedFiles.Count; i++)
            {
                // Replace templates in TEXT
                var file = tempCreatedFiles[i];
                var text = System.IO.File.ReadAllText(file.Info.FullName);
                var processedText = ReplaceTemplates(text, ctx);
                System.IO.File.WriteAllText(file.Info.FullName, processedText);

                // Replace templates in DIRECTORY NAMES
                var processedDirectoryName = ReplaceTemplates(file.Info.Directory.FullName, ctx);
                if (file.Info.Directory.FullName != processedDirectoryName)
                {
                    System.IO.Directory.Move(file.Info.Directory.FullName, processedDirectoryName);
                    for (int j = 0; j < tempCreatedFiles.Count; j++)
                    {
                        tempCreatedFiles[j] = new Model.File
                        {
                            Info = new FileInfo(
                                tempCreatedFiles[j].Info.FullName.Replace(file.Info.Directory.FullName, processedDirectoryName)
                            )
                        };
                    }
                }

                // Replace templates in FILE NAMES
                var processedFileName = ReplaceTemplates(file.Info.FullName, ctx);
                if (file.Info.FullName != processedFileName)
                {
                    System.IO.File.Move(Path.Combine(processedDirectoryName, file.Info.Name), processedFileName);
                    file = new Model.File {Info = new FileInfo(processedFileName)};
                }

                processedFiles.Add(file);
            }

            return new Project
            {
                ProcessedFiles = processedFiles
            };
        }

        public string ReplaceTemplates(string textWithTemplates, Context ctx)
        {
            var processedText = textWithTemplates.Replace($"{{{{ .{nameof(ctx.ProjectName)} }}}}", ctx.ProjectName);
            processedText = processedText.Replace($"{{{{ .{nameof(ctx.Version)} }}}}", ctx.Version);
            processedText = processedText.Replace($"{{{{ .{nameof(ctx.Description)} }}}}", ctx.Description);
            return processedText;
        }
    }
}
