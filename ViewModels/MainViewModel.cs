using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsInfo.ViewModels
{
    /// <summary>
    /// Модель представления главного окна
    /// </summary>
    internal class MainViewModel
    {
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
            ScanCommand = new ActionCommand(BeginProjectsScan);
            EditLocationsCommand = new ActionCommand(EditProjectLocations);
        }

        private void BeginProjectsScan(object _)
        {

        }

        private void EditProjectLocations(object _)
        {
            new LocationsWindows().ShowDialog();
        }
    }
}
