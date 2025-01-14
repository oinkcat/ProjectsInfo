﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace ProjectsInfo.Data.Analyzers
{
    /// <summary>
    /// Собирает информацию о проектах в стиле Visual Studio
    /// </summary>
    internal class VSProjectAnalyzer : BaseProjectAnalyzer
    {
        private const string ProjFileSuffix = "proj";
        private const string ProjXmlPropertyNodeName = "PropertyGroup";

        private readonly static string[] webRelatedPropNameParts =
        {
            "IISExpress",
            "Nodejs",
            "NodeJs",
            "WebBrowser",
            "SpaRoot"
        };

        public VSProjectAnalyzer(string projFileName) : base(projFileName) { }

        /// <summary>
        /// Получить детальную информацию о проекте
        /// </summary>
        public override ProjectInfo GetProjectInfo()
        {
            // Основная информация  проекте
            string projFileExt = Path.GetExtension(projectInfoFileName);
            int projLangIdLength = projFileExt.Length - ProjFileSuffix.Length;
            info.MainLanguage = projFileExt.Substring(0, projLangIdLength).TrimStart('.');

            CollectInfoFromProjXmlFile();
            
            if(String.IsNullOrEmpty(info.Name))
            {
                info.Name = Path.GetFileNameWithoutExtension(projectInfoFileName);
            }

            // Ближайшая дата изменения файлов данных
            var projectBaseDir = Directory.GetParent(projectInfoFileName);

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

                if(info.LastBuildDate == DateTime.MinValue)
                {
                    info.LastBuildDate = null;
                }
            }

            return info;
        }

        private void CollectInfoFromProjXmlFile()
        {
            var xProjDocument = XDocument.Load(projectInfoFileName);
            var sdkAttr = xProjDocument.Root.Attribute(XName.Get("Sdk"));
            bool probablyWebApp = (sdkAttr != null) && sdkAttr.Value.EndsWith(".Web");

            var xPropertyElems = xProjDocument.Root
                .Elements()
                .Where(e => e.Name.LocalName.Equals(ProjXmlPropertyNodeName))
                .SelectMany(e => e.Elements())
                .ToList();

            foreach(var xPropElem in xPropertyElems)
            {
                var (propName, propValue) = (xPropElem.Name.LocalName, xPropElem.Value);

                switch(propName)
                {
                    case "TargetFramework":
                    case "TargetFrameworks":
                    case "TargetFrameworkVersion":
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

                    default:
                        if(webRelatedPropNameParts.Any(n => propName.Contains(n)))
                        {
                            probablyWebApp = true;
                        }
                        break;
                };
            }

            if((info.OutputType != BuiltOutputType.Executable) && probablyWebApp)
            {
                info.OutputType = BuiltOutputType.WebApp;
            }
        }
    }
}
