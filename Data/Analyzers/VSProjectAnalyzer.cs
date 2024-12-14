using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace ProjectsInfo.Data.Analyzers
{
    /// <summary>
    /// Собирает информацию о проектах в стиле Visual Studio
    /// </summary>
    internal class VSProjectAnalyzer
    {
        private readonly string projectFileName;

        private readonly ProjectInfo info;

        public VSProjectAnalyzer(string projFileName)
        {
            projectFileName = projFileName;

            info = new ProjectInfo
            {
                Directory = Path.GetDirectoryName(projFileName)
            };
        }

        /// <summary>
        /// Получить детальную информацию о проекте
        /// </summary>
        public async Task<ProjectInfo> GetProjectInfoAsync()
        {
            info.Name = Path.GetFileNameWithoutExtension(projectFileName);

            return info;
        }
    }
}
