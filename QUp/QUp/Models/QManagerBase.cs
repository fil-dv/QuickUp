using QUp.Infrastr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QUp.Models
{
    public class QManagerBase
    {
        #region ExceptionHandler
        public static void ExceptionHandler(string methodName, string exMessage)
        {
            QMediator.IsAuto = false;
            string message = "Exception from " + methodName + Environment.NewLine + exMessage;
            QLoger.AddRecordToLog(message);
            MessageBox.Show(message);
        }
        #endregion
    }
}
