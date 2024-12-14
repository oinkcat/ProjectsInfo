using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using ProjectsInfo.Data.Analyzers;

namespace ProjectsInfo.Data
{
    /// <summary>
    /// Сканирует проекты в заданных расположениях
    /// </summary>
    internal class ProjectsScanner
    {
        private const int ProjectScanDepth = 2;

        private readonly IEnumerable<string> projectLocations;

        private bool isBusy;

        private IProgress<ProjectInfo> progressReporter;

        public ProjectsScanner(IEnumerable<string> locations)
        {
            projectLocations = locations;
        }

        /// <summary>
        /// Произвести поиск проектов в расположениях
        /// </summary>
        /// <param name="progress">Прогресс поиска (новый найденный проект)</param>
        public async Task PerformScanAsync(IProgress<ProjectInfo> progress)
        {
            if(isBusy) { return; }

            isBusy = true;
            progressReporter = progress;

            foreach(string location in projectLocations)
            {
                await FindProjectsInLocation(new DirectoryInfo(location), 0);
            }

            isBusy = false;
        }

        private async Task FindProjectsInLocation(DirectoryInfo location, int depth)
        {
            const string ProjectFilePattern = "*.*proj";

            if(depth > ProjectScanDepth) { return; }

            // Файлы проекта
            foreach (var projFile in location.GetFiles(ProjectFilePattern))
            {
                var projectInfoAnalyzer = new VSProjectAnalyzer(projFile.FullName);
                var projectInfo = await projectInfoAnalyzer.GetProjectInfoAsync();
                progressReporter.Report(projectInfo);
            }

            // Подкаталоги
            foreach(var subDirectory in location.GetDirectories())
            {
                await FindProjectsInLocation(subDirectory, depth + 1);
            }
        }
    }
}
