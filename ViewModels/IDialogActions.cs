using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsInfo.ViewModels
{
    /// <summary>
    /// Операции взаимодействия с окнами
    /// </summary>
    internal interface IDialogActions
    {
        /// <summary>
        /// Закрыть диалоговое окно с заданным результатом
        /// </summary>
        /// <param name="success">Успешный результат</param>
        void CloseDialogWithResult(bool success);
    }
}
