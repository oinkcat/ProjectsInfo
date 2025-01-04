using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsInfo.ViewModels
{
    /// <summary>
    /// Базовая функциональность модели представления
    /// </summary>
    internal abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Элемент модели изменен
        /// </summary>
        /// <param name="name">Название элемента</param>
        protected void ModelItemChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
