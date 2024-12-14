using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectsInfo.Data;
using ProjectsInfo.Data.Storage;

namespace ProjectsInfo.ViewModels
{
    /// <summary>
    /// Модель представления главного окна
    /// </summary>
    internal class MainViewModel
    {
        public ObservableCollection<ProjectInfo> FoundProjects { get; }

        /// <summary>
        /// Команда сканирования каталогов
        /// </summary>
        public ActionCommand ScanCommand { get; }

        /// <summary>
        /// Команда редактирования расположений
        /// </summary>
        public ActionCommand EditLocationsCommand { get; }

        public MainViewModel()
        {
            FoundProjects = new ObservableCollection<ProjectInfo>();
            ScanCommand = new ActionCommand(BeginProjectsScan);
            EditLocationsCommand = new ActionCommand(EditProjectLocations);

            //PromptForLocationsIfAbsent();
        }

        private async void PromptForLocationsIfAbsent()
        {
            var allLocations = await new ProjectLocationsStore().LoadLocationsAsync();

            if(!allLocations.Any())
            {
                EditProjectLocations(null);
            }
        }

        private async void BeginProjectsScan(object _)
        {
            ScanCommand.IsEnabled = false;
            FoundProjects.Clear();

            var projectLocations = await new ProjectLocationsStore().LoadLocationsAsync();
            var scanner = new ProjectsScanner(projectLocations);
            var scanProgress = new Progress<ProjectInfo>(NewProjectInfoScanned);
            await scanner.PerformScanAsync(scanProgress);

            ScanCommand.IsEnabled = true;
        }

        private void NewProjectInfoScanned(ProjectInfo info)
        {
            FoundProjects.Add(info);
        }

        private void EditProjectLocations(object _)
        {
            var editResult = new LocationsWindow().ShowDialog();

            if(editResult.Value)
            {
                BeginProjectsScan(null);
            }
        }
    }
}
