using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QUp.Infrastr
{
    public static class QLoger
    {
        public static void AddRecordToLog(string message)
        {
            try
            {
                if (String.IsNullOrEmpty(message))
                {
                    return;
                }
                string path = Path.Combine(QMediator.PathToRegDest, "_upload_log.txt");
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                string fileText = File.ReadAllText(path);
                if (!fileText.Contains(message))
                {
                    string textToAdd = Environment.NewLine + DateTime.Now + "\t" + Environment.UserName + Environment.NewLine + message + Environment.NewLine;
                    File.AppendAllText(path, textToAdd);                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
