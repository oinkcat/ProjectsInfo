using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsInfo.Data
{
    /// <summary>
    /// Тип построенного приложения
    /// </summary>
    public enum BuiltOutputType
    {
        Library,
        Executable,
        WebApp
    }

    /// <summary>
    /// Информация о проекте
    /// </summary>
    public class ProjectInfo
    {
        /// <summary>
        /// Имя проекта
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Базовый каталог
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Основной язык программирования
        /// </summary>
        public string MainLanguage { get; set; }

        /// <summary>
        /// Целевая среда
        /// </summary>
        public string TargetFramework { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Дата последней модификации исходного кода
        /// </summary>
        public DateTime LastModifyDate { get; set; }

        /// <summary>
        /// Дата последней модификации артефактов сборки
        /// </summary>
        public DateTime? LastBuildDate { get; set; }

        /// <summary>
        /// Тип приложения
        /// </summary>
        public BuiltOutputType OutputType { get; set; }

        /// <summary>
        /// Использует систему управления версиями
        /// </summary>
        public bool IsInGit { get; set; }

        /// <summary>
        /// Является ли частью решения Visual Studio
        /// </summary>
        public bool IsInSolution { get; set; }
    }
}
