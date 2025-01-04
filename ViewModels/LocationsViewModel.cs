using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using ProjectsInfo.Data.Storage;

using FolderDialog = System.Windows.Forms.FolderBrowserDialog;
using WfDialogResult = System.Windows.Forms.DialogResult;

namespace ProjectsInfo.ViewModels
{
    /// <summary>
    /// Модель представления окна путей поиска проектов
    /// </summary>
    internal class LocationsViewModel : BaseViewModel
    {
        private string pathText;

        private readonly ProjectLocationsStore locationsStore;

        /// <summary>
        /// Взаимодействие с окном
        /// </summary>
        public IDialogActions WindowHelper { get; set; }

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
        /// Команда выбора папки
        /// </summary>
        public ActionCommand BrowseFolderCommand { get; }

        /// <summary>
        /// Команда сохранения путей поиска из списка
        /// </summary>
        public ActionCommand SavePathsCommand { get; }

        public LocationsViewModel()
        {
            locationsStore = new ProjectLocationsStore();

            Paths = new ObservableCollection<string>(Task.Run(LoadLocations).Result);

            AddNewPathCommand = new ActionCommand(AddNewPath, false);
            RemovePathCommand = new ActionCommand(RemovePathItem);
            BrowseFolderCommand = new ActionCommand(BrowseFolder);
            SavePathsCommand = new ActionCommand(SaveLocations);
        }

        private async Task<IList<string>> LoadLocations()
        {
            return await locationsStore.LoadLocationsAsync();
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

        private void BrowseFolder(object _)
        {
            var selectFolderDialog = new FolderDialog
            {
                ShowNewFolderButton = false
            };
            
            if(selectFolderDialog.ShowDialog() == WfDialogResult.OK)
            {
                NewPathText = selectFolderDialog.SelectedPath;
                ModelItemChanged(nameof(NewPathText));
            }
        }

        private async void SaveLocations(object _)
        {
            await locationsStore.SaveLocationsAsync(Paths);
            WindowHelper.CloseDialogWithResult(true);
        }
    }
}
