using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProjectsInfo.Data;
using ProjectsInfo.Data.Storage;

namespace ProjectsInfo.ViewModels
{
    /// <summary>
    /// Модель представления главного окна
    /// </summary>
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string nameFilter;

        private ProjectInfo[] allScannedProjects;

        /// <summary>
        /// Все найденные проекты
        /// </summary>
        public ObservableCollection<ProjectInfo> FoundProjects { get; private set; }

        /// <summary>
        /// Число найденных проектов
        /// </summary>
        public int NumberOfProjectsFound => FoundProjects.Count;

        /// <summary>
        /// Горово к сканированию
        /// </summary>
        public bool IsReady => ScanCommand.IsEnabled;

        /// <summary>
        /// Происходит сканирование
        /// </summary>
        public bool IsScanning => !IsReady;

        /// <summary>
        /// Фильтр по имени проекта
        /// </summary>
        public string NameFilterText
        {
            get => nameFilter;

            set
            {
                nameFilter = value;
                PerformFilteringByName();
            }
        }

        /// <summary>
        /// Команда сканирования каталогов
        /// </summary>
        public ActionCommand ScanCommand { get; }

        /// <summary>
        /// Команда редактирования расположений
        /// </summary>
        public ActionCommand EditLocationsCommand { get; }

        /// <summary>
        /// Команда открытия каталога проекта
        /// </summary>
        public ActionCommand OpenProjectDirectoryCommand { get; }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            FoundProjects = new ObservableCollection<ProjectInfo>();
            ScanCommand = new ActionCommand(BeginProjectsScan);
            EditLocationsCommand = new ActionCommand(EditProjectLocations);
            OpenProjectDirectoryCommand = new ActionCommand(OpenProjectDirectory);

            PerformStartupAction();
        }

        private async void PerformStartupAction()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));

            var allLocations = await new ProjectLocationsStore().LoadLocationsAsync();

            if (allLocations.Any())
            {
                ScanCommand.Execute(null);
            }
            else
            {
                EditLocationsCommand.Execute(null);
            }
        }

        private async void BeginProjectsScan(object _)
        {
            ScanCommand.IsEnabled = false;
            NameFilterText = String.Empty;
            ModelItemChanged(nameof(IsReady));
            ModelItemChanged(nameof(IsScanning));
            ModelItemChanged(nameof(NameFilterText));
            FoundProjects.Clear();

            var projectLocations = await new ProjectLocationsStore().LoadLocationsAsync();
            var scanner = new ProjectsScanner(projectLocations);
            var scanProgress = new Progress<ProjectInfo>(NewProjectInfoScanned);
            await scanner.PerformScanAsync(scanProgress);

            allScannedProjects = new ProjectInfo[FoundProjects.Count];
            FoundProjects.CopyTo(allScannedProjects, 0);

            ScanCommand.IsEnabled = true;
            ModelItemChanged(nameof(IsReady));
            ModelItemChanged(nameof(IsScanning));
        }

        private void NewProjectInfoScanned(ProjectInfo info)
        {
            FoundProjects.Add(info);
            ModelItemChanged(nameof(NumberOfProjectsFound));
        }

        private void EditProjectLocations(object _)
        {
            var editResult = new LocationsWindow().ShowDialog();

            if(editResult.Value)
            {
                BeginProjectsScan(null);
            }
        }

        private void OpenProjectDirectory(object param)
        {
            var openDirParams = new ProcessStartInfo(param as string)
            {
                Verb = "Open"
            };

            Process.Start(openDirParams);
        }

        private void PerformFilteringByName()
        {
            if(allScannedProjects == null) { return; }

            IEnumerable<ProjectInfo> resultProjects = String.IsNullOrEmpty(nameFilter)
                ? allScannedProjects
                : allScannedProjects
                    .Where(p => p.Name.ToLower().Contains(nameFilter.ToLower()));

            FoundProjects = new ObservableCollection<ProjectInfo>(resultProjects);
            ModelItemChanged(nameof(FoundProjects));
            ModelItemChanged(nameof(NumberOfProjectsFound));
        }

        private void ModelItemChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
