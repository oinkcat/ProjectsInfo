using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectsInfo.Data.Analyzers;

namespace ProjectsInfo.Data
{
    /// <summary>
    /// Распознает тип проекта
    /// </summary>
    internal class ProjectRecognizer
    {
        private class ProjectTraits
        {
            public string ProjectFileName { get; set; }

            public string ProjectDirectory { get; set; }

            public Type AnalyzerType { get; set; }
        }

        private readonly ProjectTraits[] knownProjectTraits =
        {
            new ProjectTraits
            {
                ProjectFileName = "*.*proj",
                ProjectDirectory = null,
                AnalyzerType = typeof(VSProjectAnalyzer)
            }
        };

        /// <summary>
        /// Получить анализатор проекта для данного каталога
        /// </summary>
        /// <param name="dir">Анализируемый каталог</param>
        /// <returns>Анализатор проекта</returns>
        public BaseProjectAnalyzer GetAnalyzerForKnownProject(DirectoryInfo dir)
        {
            foreach(var traits in knownProjectTraits)
            {
                var infoFile = dir.EnumerateFiles(traits.ProjectFileName).SingleOrDefault();
                bool fileFound = infoFile != null;

                bool dirFound = String.IsNullOrEmpty(traits.ProjectDirectory) ||
                    dir.EnumerateDirectories(traits.ProjectDirectory).Any();

                if(fileFound && dirFound)
                {
                    return Activator.CreateInstance(traits.AnalyzerType, infoFile.FullName) 
                        as BaseProjectAnalyzer;
                }
            }

            return null;
        }
    }
}
