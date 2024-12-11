using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsInfo.Models
{
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
        public DateTime LastBuildDate { get; set; }

        /// <summary>
        /// Использует систему управления версиями
        /// </summary>
        public bool IsInGit { get; set; }

        //...
    }
}
