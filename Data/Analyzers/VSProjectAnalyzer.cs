using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Runtime.ExceptionServices;

namespace ProjectsInfo.Data.Analyzers
{
    /// <summary>
    /// Собирает информацию о проектах в стиле Visual Studio
    /// </summary>
    internal class VSProjectAnalyzer
    {
        private const string ProjFileSuffix = "proj";
        private const string ProjXmlPropertyNodeName = "PropertyGroup";
        
        private readonly string projectFileName;

        private readonly ProjectInfo info;

        public VSProjectAnalyzer(string projFileName)
        {
            projectFileName = projFileName;

            info = new ProjectInfo
            {
                Directory = Path.GetDirectoryName(projFileName),
                CreateDate = DateTime.MaxValue
            };
        }

        /// <summary>
        /// Получить детальную информацию о проекте
        /// </summary>
        public ProjectInfo GetProjectInfo()
        {
            // Основная информация  проекте
            string projFileExt = Path.GetExtension(projectFileName);
            int projLangIdLength = projFileExt.Length - ProjFileSuffix.Length;
            info.MainLanguage = projFileExt.Substring(0, projLangIdLength).TrimStart('.');

            CollectInfoFromProjXmlFile();
            
            if(String.IsNullOrEmpty(info.Name))
            {
                info.Name = Path.GetFileNameWithoutExtension(projectFileName);
            }

            // Ближайшая дата изменения файлов данных
            var projectBaseDir = Directory.GetParent(projectFileName);

            string[] binaryDirs =
            {
                Path.Combine(projectBaseDir.FullName, "bin"),
                Path.Combine(projectBaseDir.FullName, "obj")
            };

            var exclude = new HashSet<string>(binaryDirs);
            info.LastModifyDate = FindLatestFileModifyDate(projectBaseDir, exclude);

            // Ближайшая дата изменения файлов артефактов сборки
            var binDir = new DirectoryInfo(binaryDirs[0]);
            var objDir = new DirectoryInfo(binaryDirs[1]);

            if(binDir.Exists || objDir.Exists)
            {
                var maxBinDate = FindLatestFileModifyDate(binDir, null);
                var maxObjDate = FindLatestFileModifyDate(objDir, null);

                info.LastBuildDate = maxBinDate > maxObjDate ? maxBinDate : maxObjDate;
            }

            return info;
        }

        private void CollectInfoFromProjXmlFile()
        {
            var xProjDocument = XDocument.Load(projectFileName);

            var xPropertyElems = xProjDocument.Descendants(XName.Get(ProjXmlPropertyNodeName))
                .SelectMany(e => e.Elements())
                .ToList();

            foreach(var xPropElem in xPropertyElems)
            {
                string propValue = xPropElem.Value;

                switch(xPropElem.Name.LocalName)
                {
                    case "TargetFramework":
                        info.TargetFramework = propValue;
                        break;

                    case "OutputType":
                        info.OutputType = propValue.ToLower().EndsWith("exe")
                            ? BuiltOutputType.Executable
                            : BuiltOutputType.Library;
                        break;

                    case "Name":
                    case "AssemblyName":
                        info.Name = propValue;
                        break;
                };
            }
        }

        private DateTime FindLatestFileModifyDate(DirectoryInfo dir, ISet<string> excludeDirs)
        {
            if(!dir.Exists) { return DateTime.MinValue; }

            var maxFileModifyDate = DateTime.MinValue;
            var filesInDir = dir.GetFiles();

            if(filesInDir.Any())
            {
                maxFileModifyDate = filesInDir.Max(f => f.LastWriteTime);

                var minCreateDate = filesInDir.Min(f => f.CreationTime);
                info.CreateDate = (minCreateDate < info.CreateDate)
                    ? minCreateDate
                    : info.CreateDate;
            }

            foreach(var subDir in dir.EnumerateDirectories())
            {
                if(subDir.Name.StartsWith(".")) { continue; }
                if(excludeDirs?.Contains(subDir.FullName) ?? false) { continue; }

                var maxModifyDateInSubDir = FindLatestFileModifyDate(subDir, excludeDirs);
                maxFileModifyDate = (maxModifyDateInSubDir > maxFileModifyDate)
                    ? maxModifyDateInSubDir
                    : maxFileModifyDate;
            }

            return maxFileModifyDate;
        }
    }
}
