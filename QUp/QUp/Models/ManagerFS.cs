using QUp.Infrastr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QUp.Models
{

    public delegate void ResultIsReady(string res);

    public static class ManagerFS
    {
        static string _report = "";
        static public event Action SplitCompletHandler;
        public static event ResultIsReady ReportUpdated;

        #region creating files
        enum FileType
        {
            Reg,
            Prog
        };
        
        public static string CreateNewFiles()
        {            
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
                }
            }
            return UpdateResultReport();
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
                    _report += "У данного контрагента не найдена папка для копирования файлов заливки.";
                    CopyRegFiles();
                }
                else
                {
                    _report += Environment.NewLine + "У данного контрагента не найдена папка для копирования программ.";
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

            _report += Environment.NewLine + "Копируем файлы из " + Environment.NewLine + "   " +  sourceDir + Environment.NewLine + "в " + destDir + Environment.NewLine + Environment.NewLine;

            foreach (var item in listToCopy)
            {
                File.Copy(item.FullName, destDir + "\\" + item.Name, true);
                _report += item.Name + Environment.NewLine;
            }
        }

        static void CopyProgFiles()
        {
            _report += Environment.NewLine + Environment.NewLine + "Копируем содержимое папки  " + Environment.NewLine + "    " + QMediator.PathToProgSource + Environment.NewLine + " в " + QMediator.PathToProgDest + "." + Environment.NewLine;

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
        public static string RunCtrlCreator()
        {
            _report = "";

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

            return _report;
        }
        #endregion

        #region Split adress
        public static void SplitAdr()
        {
            _report = "\n\n\t  Разбивка адресов...";
            ReportUpdated?.Invoke(UpdateResultReport());

            SplitCompletHandler += UpdateReport;

            ProcessStartInfo pInfo = new System.Diagnostics.ProcessStartInfo(@"d:\Dima\Programming\git\AdressSpliter\Adr\Adr\bin\Debug\Adr.exe");
            if (QMediator.PathToRegDest != null)
            {
                pInfo.Arguments = ("\"" + QMediator.PathToRegDest + "\"");
            }
            Process process = Process.Start(pInfo);
            process.EnableRaisingEvents = true;
            process.Exited += SplitFinished;
           // process.Close();


                   
            //Thread thread = new Thread(Split);
            //thread.Start();  
            //_report = ReadLog();
            //ReportUpdated?.Invoke(UpdateResultReport());
        }

        private static void SplitFinished(object sender, EventArgs e)
        {
            SplitCompletHandler?.Invoke();
        }

        private static void UpdateReport()
        {
            _report = ReadLog();
            ReportUpdated?.Invoke(UpdateResultReport());
        }

        //private static void Split()
        //{
        //    ProcessStartInfo pInfo = new System.Diagnostics.ProcessStartInfo(@"d:\Dima\Programming\git\AdressSpliter\Adr\Adr\bin\Debug\Adr.exe");
        //    if(QMediator.PathToRegDest != null)
        //    {
        //        pInfo.Arguments = ("\"" + QMediator.PathToRegDest + "\"");
        //    }                
        //    Process process = Process.Start(pInfo);
        //   // process.Exited += SplitFinished;
        //    process.Close();
        //}



        private static string ReadLog()
        {
            string str = File.ReadAllText("log.txt");
            int index = str.LastIndexOf("------------------");
            str = str.Substring(index + 18);
            return str;
        }
        #endregion



        private static string UpdateResultReport()
        {
            return Environment.NewLine + "   " + _report.Replace(Environment.NewLine, Environment.NewLine + "   ");
        }
    }
}
