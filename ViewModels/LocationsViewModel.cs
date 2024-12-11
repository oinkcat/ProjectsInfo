using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsInfo.ViewModels
{
    /// <summary>
    /// Модель представления окна путей поиска проектов
    /// </summary>
    internal class LocationsViewModel
    {
        private string pathText;

        /// <summary>
        /// Пути поиска
        /// </summary>
        public ObservableCollection<string> Paths { get; }

        /// <summary>
        /// Текст поля нового пути поиска
        /// </summary>
        public string NewPathText
        {
            get => pathText;

            set
            {
                pathText = value;
                AddNewPathCommand.IsEnabled = !String.IsNullOrWhiteSpace(pathText);
            }
        }

        /// <summary>
        /// Команда добавления нового пути поиска
        /// </summary>
        public ActionCommand AddNewPathCommand { get; }

        /// <summary>
        /// Команда удаления пути поиска
        /// </summary>
        public ActionCommand RemovePathCommand { get; }

        public LocationsViewModel()
        {
            Paths = new ObservableCollection<string>();

            AddNewPathCommand = new ActionCommand(AddNewPath, false);
            RemovePathCommand = new ActionCommand(RemovePathItem);
        }

        private void AddNewPath(object _)
        {
            string pathToAdd = NewPathText.Trim();

            if(!Paths.Any(p => p.Equals(pathToAdd)))
            {
                Paths.Add(pathToAdd);
            }
        }

        private void RemovePathItem(object path)
        {
            Paths.Remove(path as string);
        }
    }
}
