using QUp.Infrastr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QUp.Models
{
    
    public static class ManagerFS
    {
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
                fbd.SelectedPath = @"x:\Реєстри\Акцент\test";
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
            if (dirList[dirList.Count - 2] != null)
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
                System.Windows.Forms.MessageBox.Show("Не найдена папка для копирования файлов.");
                return;
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

            QMediator.ResultReport += Environment.NewLine + "Копируем файлы из " + Environment.NewLine + sourceDir + Environment.NewLine + " в " + destDir + "." + Environment.NewLine;

            foreach (var item in listToCopy)
            {
                File.Copy(item.FullName, destDir + "\\" + item.Name, true);
                QMediator.ResultReport += item.Name + Environment.NewLine;
            }
        }

        static void CopyProgFiles()
        {
            DirectoryInfo sourceDir = new DirectoryInfo(QMediator.PathToProgSource);
            FileInfo[] files = sourceDir.GetFiles();
            List<FileInfo> listToCopy = new List<FileInfo>();
            foreach (var item in files)
            {
                listToCopy.Add(item);
            }

            DirectoryInfo destDir = new DirectoryInfo(QMediator.PathToProgDest);

            QMediator.ResultReport += Environment.NewLine + "Копируем файлы из " + Environment.NewLine + sourceDir + Environment.NewLine + " в " + destDir + "." + Environment.NewLine;

            foreach (var item in listToCopy)
            {
                File.Copy(item.FullName, destDir + "\\" + item.Name, true);
                QMediator.ResultReport += item.Name + Environment.NewLine;
            }
        }
    }
}
