using QUp.Infrastr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QUp.Models
{
    
    public static class ManagerFS
    {
        #region creating files
        enum FileType
        {
            Reg,
            Prog
        };

        public static void CreateNewFiles()
        {
            QMediator.ResultReport = "";
            using (var fbd = new FolderBrowserDialog())
            {
                //fbd.SelectedPath = @"x:\Реєстри\ЄАПБ (Факторинг)\";
                fbd.SelectedPath = @"x:\Реєстри\ЯЯЯTest\1  1111";
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    QMediator.PathToRegDest = fbd.SelectedPath;
                    QMediator.PathToProgDest = QMediator.PathToRegDest.Replace("Реєстри", "Progs\\Registry");
                    if (!QMediator.PathToRegDest.Contains("ЦиФРа"))          // для ЦФР ничего не создаем
                        Directory.CreateDirectory(QMediator.PathToProgDest);
                    FindSource(QMediator.PathToRegDest, FileType.Reg);
                    if(!QMediator.PathToRegDest.Contains("ЦиФРа"))          // для ЦФР ничего не копируем
                        FindSource(QMediator.PathToProgDest, FileType.Prog);
                    UpdateResultReport();
                }
            }
        }

        static void FindSource(string path, FileType fileType )
        {
            int index = path.LastIndexOfAny(new char[] { '\\' });
            string pathToParent = path.Substring(0, index);
            string[] dirs = Directory.GetDirectories(pathToParent);
            List<DirectoryInfo> dirList = new List<DirectoryInfo>();
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i].Contains("  "))
                {
                    DirectoryInfo directory = new DirectoryInfo(dirs[i]);
                    dirList.Add(directory);
                }
            }
            dirList.Sort((x, y) => DateTime.Compare(x.CreationTime, y.CreationTime));
            DirectoryInfo sourceDir = null;

            if (dirList.Count > 1 && dirList[dirList.Count - 2] != null)
            {
                sourceDir = dirList[dirList.Count - 2];

                if (fileType == FileType.Reg)
                {
                    QMediator.PathToRegSource = sourceDir.FullName;
                    CopyRegFiles();
                }
                else
                {
                    QMediator.PathToProgSource = sourceDir.FullName;
                    CopyProgFiles();
                }
            }

            else
            {
                if (fileType == FileType.Reg)
                {
                    QMediator.ResultReport += "У данного контрагента не найдена папка для копирования файлов заливки.";
                    CopyRegFiles();
                }
                else
                {
                    QMediator.ResultReport += Environment.NewLine + "У данного контрагента не найдена папка для копирования программ.";
                    CopyProgFiles();
                }
            }
        }

        static void CopyRegFiles()
        {
            DirectoryInfo sourceDir = new DirectoryInfo(QMediator.PathToRegSource);
            FileInfo[] files = sourceDir.GetFiles();
            List<FileInfo> listToCopy = new List<FileInfo>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Extension.ToLower() == ".bat" ||
                    files[i].Extension.ToLower() == ".ctl" ||
                    files[i].Extension.ToLower() == ".txt")
                {
                    listToCopy.Add(files[i]);
                }
            }
            DirectoryInfo destDir = new DirectoryInfo(QMediator.PathToRegDest);

            QMediator.ResultReport += Environment.NewLine + "Копируем файлы из " + Environment.NewLine + "   " +  sourceDir + Environment.NewLine + "в " + destDir + Environment.NewLine + Environment.NewLine;

            foreach (var item in listToCopy)
            {
                File.Copy(item.FullName, destDir + "\\" + item.Name, true);
                QMediator.ResultReport += item.Name + Environment.NewLine;
            }
        }

        static void CopyProgFiles()
        {
            QMediator.ResultReport += Environment.NewLine + Environment.NewLine + "Копируем содержимое папки  " + Environment.NewLine + "    " + QMediator.PathToProgSource + Environment.NewLine + " в " + QMediator.PathToProgDest + "." + Environment.NewLine;

            foreach (string dirPath in Directory.GetDirectories(QMediator.PathToProgSource, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(QMediator.PathToProgSource, QMediator.PathToProgDest));
            }
            foreach (string newPath in Directory.GetFiles(QMediator.PathToProgSource, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(QMediator.PathToProgSource, QMediator.PathToProgDest), true);
            }
        }
        #endregion

        #region RunControlCreator
        public static void RunCtrlCreator()
        {
            QMediator.ResultReport = "";

            ProcessStartInfo pInfo = new ProcessStartInfo("c-creator.exe");
            DirectoryInfo directoryInfo = new DirectoryInfo(QMediator.PathToRegDest);
            FileInfo[] files = directoryInfo.GetFiles("*.xls*");
            string path = "";
            if (files.Length > 0)
            {
                path = files[0].FullName;
            }
            pInfo.Arguments = "\"" + path + "\"";
            pInfo.WorkingDirectory = @"d:\Dima\Programming\git\Control-creator\c-creator\bin\Release";
            Process p = Process.Start(pInfo);
        }
        #endregion





        private static void UpdateResultReport()
        {
            QMediator.ResultReport = Environment.NewLine + Environment.NewLine + "   " + QMediator.ResultReport.Replace(Environment.NewLine, Environment.NewLine + "   ");
        }
    }
}
