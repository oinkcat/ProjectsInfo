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
        private class ScanContext
        {
            public bool IsInSolution { get; set; }

            public bool IsInGit { get; set; }
        }

        private const int ProjectScanDepth = 2;

        private const string GitDirectoryName = ".git";
        private const string SolutionExtension = ".sln";

        private readonly IEnumerable<string> projectLocations;

        private readonly ProjectRecognizer recognizer;

        private bool isBusy;

        private IProgress<ProjectInfo> progressReporter;

        public ProjectsScanner(IEnumerable<string> locations)
        {
            projectLocations = locations;
            recognizer = new ProjectRecognizer();
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

            await Task.Run(() =>
            {
                foreach (string location in projectLocations)
                {
                    var scanCtx = new ScanContext();
                    ScanProjects(new DirectoryInfo(location), 0, scanCtx);
                }
            });

            isBusy = false;
        }

        // Сканировать проекты в заданном каталоге с определенной глубиной поиска
        private void ScanProjects(DirectoryInfo location, int depth, ScanContext ctx)
        {
            if(depth > ProjectScanDepth) { return; }

            // Принадлежность к решению и git репозиторию
            if(Directory.Exists(Path.Combine(location.FullName, GitDirectoryName)))
            {
                ctx.IsInGit = true;
            }

            if(location.GetFiles(String.Concat('*', SolutionExtension)).Any())
            {
                ctx.IsInSolution = true;
            }

            // Определить тип проекта в каталоге и проанализировать его
            var projectInfoAnalyzer = recognizer.GetAnalyzerForKnownProject(location);

            if(projectInfoAnalyzer != null)
            {
                var projectInfo = projectInfoAnalyzer.GetProjectInfo();
                projectInfo.IsInGit = ctx.IsInGit;
                projectInfo.IsInSolution = ctx.IsInSolution;

                progressReporter.Report(projectInfo);
            }

            // Подкаталоги
            foreach(var subDirectory in location.GetDirectories())
            {
                ScanProjects(subDirectory, depth + 1, ctx);
            }
        }
    }
}
