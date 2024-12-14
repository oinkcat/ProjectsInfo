using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using ProjectsInfo.Data.Storage;

namespace ProjectsInfo.ViewModels
{
    /// <summary>
    /// Модель представления окна путей поиска проектов
    /// </summary>
    internal class LocationsViewModel
    {
        private string pathText;

        private readonly ProjectLocationsStore locationsStore;

        /// <summary>
        /// Взаимодействие с окном
        /// </summary>
        public IWindowActions WindowHelper { get; set; }

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

        /// <summary>
        /// Команда сохранения путей поиска из списка
        /// </summary>
        public ActionCommand SavePathsCommand { get; set; }

        public LocationsViewModel()
        {
            locationsStore = new ProjectLocationsStore();

            Paths = new ObservableCollection<string>(Task.Run(LoadLocations).Result);

            AddNewPathCommand = new ActionCommand(AddNewPath, false);
            RemovePathCommand = new ActionCommand(RemovePathItem);
            SavePathsCommand = new ActionCommand(SaveLocations);
        }

        private async Task<IList<string>> LoadLocations()
        {
            return await locationsStore.LoadLocationsAsync();
        }

        private async void SaveLocations(object _)
        {
            await locationsStore.SaveLocationsAsync(Paths);
            WindowHelper.CloseDialogWithResult(true);
        }

        private void AddNewPath(object _)
        {
            string pathToAdd = NewPathText.Trim();

            if(!Paths.Any(p => p.Equals(pathToAdd)))
            {
                if(Directory.Exists(pathToAdd))
                {
                    Paths.Add(pathToAdd);
                }
                else
                {
                    MessageBox.Show(messageBoxText: "Расположение не существует",
                                    caption: "Невозможно добавить",
                                    button: MessageBoxButton.OK,
                                    icon: MessageBoxImage.Error);
                }
            }
        }

        private void RemovePathItem(object pathParam)
        {
            Paths.Remove(pathParam as string);
        }
    }
}
