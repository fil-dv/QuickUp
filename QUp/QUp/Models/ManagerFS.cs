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
                fbd.SelectedPath = @"x:\Реєстри\ЦиФРа\999  5555 Test";
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    QMediator.PathToReg = fbd.SelectedPath;
                    QMediator.PathToProg = QMediator.PathToReg.Replace("Реєстри", "Progs\\Registry");
                    FindSource(QMediator.PathToReg, FileType.Reg);
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
                    CopyRegFiles(sourceDir);
                else
                    CopyProgFiles(sourceDir);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не найдена папка для копирования файлов.");
                return;
            }
        }

        static void CopyRegFiles(DirectoryInfo sourceDir)
        {
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
            DirectoryInfo destDir = new DirectoryInfo(QMediator.PathToReg);

            QMediator.ResultReport += "Копируем файлы из " + sourceDir + " в " + destDir + "." + Environment.NewLine;

            foreach (var item in listToCopy)
            {
                File.Copy(item.FullName, destDir + "\\" + item.Name, true);
                QMediator.ResultReport += item.Name + Environment.NewLine;
            }
            CopyProgFiles(sourceDir);
        }

        static void CopyProgFiles(DirectoryInfo sourceDir)
        {
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
            DirectoryInfo destDir = new DirectoryInfo(QMediator.PathToReg);

            QMediator.ResultReport += "Копируем файлы из " + sourceDir + " в " + destDir + "." + Environment.NewLine;

            foreach (var item in listToCopy)
            {
                File.Copy(item.FullName, destDir + "\\" + item.Name, true);
                QMediator.ResultReport += item.Name + Environment.NewLine;
            }
        }
    }
}
