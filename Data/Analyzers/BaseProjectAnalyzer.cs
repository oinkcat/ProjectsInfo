using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectsInfo.Data.Analyzers
{
    /// <summary>
    /// Базовый класс анализатора проекта
    /// </summary>
    internal abstract class BaseProjectAnalyzer
    {
        protected readonly string projectInfoFileName;

        protected readonly ProjectInfo info;

        protected BaseProjectAnalyzer(string infoFileName)
        {
            projectInfoFileName = infoFileName;

            info = new ProjectInfo
            {
                Directory = Path.GetDirectoryName(projectInfoFileName),
                CreateDate = DateTime.MaxValue
            };
        }

        /// <summary>
        /// Получить детальную информацию о проекте
        /// </summary>
        public abstract ProjectInfo GetProjectInfo();

        /// <summary>
        /// Найти самую последнюю дату изменения файлов
        /// </summary>
        /// <param name="dir">Каталог для поиска</param>
        /// <param name="ignoreDirs">Каталоги для пропуска</param>
        /// <returns>Самая последняя дата изменения файлов в каталоге</returns>
        protected DateTime FindLatestFileModifyDate(DirectoryInfo dir, ISet<string> ignoreDirs)
        {
            if (!dir.Exists) { return DateTime.MinValue; }

            var maxFileModifyDate = DateTime.MinValue;
            var filesInDir = dir.GetFiles();

            if (filesInDir.Any())
            {
                maxFileModifyDate = filesInDir.Max(f => f.LastWriteTime);

                var minCreateDate = filesInDir.Min(f => f.CreationTime);
                info.CreateDate = (minCreateDate < info.CreateDate)
                    ? minCreateDate
                    : info.CreateDate;
            }

            foreach (var subDir in dir.EnumerateDirectories())
            {
                if (subDir.Name.StartsWith(".")) { continue; }
                if (ignoreDirs?.Contains(subDir.FullName) ?? false) { continue; }

                var maxModifyDateInSubDir = FindLatestFileModifyDate(subDir, ignoreDirs);
                maxFileModifyDate = (maxModifyDateInSubDir > maxFileModifyDate)
                    ? maxModifyDateInSubDir
                    : maxFileModifyDate;
            }

            return maxFileModifyDate;
        }
    }
}
