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
    public enum TaskName { PredProgs, PostProgs, Oktel };
    public delegate void ResultIsReady(string res);
    

    public static class ManagerFS
    {
        static string _report = "";
        static public event Action SplitCompletHandler;
        static public event Action<bool> Initialized;
        public static event ResultIsReady ReportUpdated;         

        #region creating files
        enum FileType
        {
            Reg,
            Prog
        };
        
        public static void CreateNewFiles()
        {
            _report = "";
            ReportUpdated?.Invoke(UpdateResultReport());

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

                    Initialized?.Invoke(true);
                }
            }
            ReportUpdated?.Invoke(UpdateResultReport()); 
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
        public static void RunCtrlCreator()
        {
            _report = "Создание контрола...";
            ReportUpdated?.Invoke(UpdateResultReport());

            ProcessStartInfo pInfo = new ProcessStartInfo("c-creator.exe");
            if (QMediator.PathToRegDest != null)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(QMediator.PathToRegDest);
                FileInfo[] files = directoryInfo.GetFiles("*.xls*");
                string path = "";
                if (files.Length > 0)
                {
                    path = files[0].FullName;
                }
                pInfo.Arguments = "\"" + path + "\"";                
            }
            pInfo.WorkingDirectory = @"d:\Dima\Programming\git\Control-creator\c-creator\bin\Release";
            Process p = Process.Start(pInfo);
            ReportUpdated?.Invoke(UpdateResultReport());
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

        #region ProgsToExexute
        public static void ProgsToExec(TaskName taskName)
        {
            _report = "Выполнение программ...\n\n";
            ReportUpdated?.Invoke(UpdateResultReport());
            if (QMediator.PathToProgDest != null)
            {
                string str;
                if (taskName == TaskName.Oktel)
                {
                    str = "\\post!\\oktel";
                }
                else
                {
                    str = taskName == TaskName.PredProgs ? "\\!pred" : "\\post!";
                }
                
                string path = QMediator.PathToProgDest + str;
                if (!new DirectoryInfo(path).Exists)
                {
                    Directory.CreateDirectory(path);
                    _report = "Нет программ для выполнения.";
                }
                else
                {
                    List<string> filePathList = GetFilesForExec(path);
                    if (filePathList.Count < 1)
                    {
                        _report = "Нет программ для выполнения.";
                        ReportUpdated?.Invoke(UpdateResultReport());
                        return;
                    }
                    foreach (var item in filePathList)
                    {
                        string[] queryList = SplitFile(item);
                        foreach (var query in queryList)
                        {
                            ManagerDB.ExecCommand(query);
                        }
                        _report += Environment.NewLine + "Отработал файл:\t\t" + item.Substring(item.LastIndexOf("\\") + 1);
                    }                    
                }                
            }
            else
            {
                _report = "Не определен путь к программам.";
            }

            ReportUpdated?.Invoke(UpdateResultReport());
        }

        private static string[] SplitFile(string path)
        {            
            string fileText = File.ReadAllText(path, Encoding.Default);
            string[] queries = fileText.Split(';');
            return queries;
        }

        private static List<string> GetFilesForExec(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fiArr = di.GetFiles();
            List<string> fileList = new List<string>();
            foreach (var item in fiArr)
            {
                if (!item.Name.Contains("--"))
                {
                    fileList.Add(item.FullName);
                }
            }
            return fileList;
        }
        #endregion

        #region SearchCtrl
        public static void SearchCtrl()
        {
            _report = "Подбор контрола..." + Environment.NewLine;
            ReportUpdated?.Invoke(UpdateResultReport());

            DirectoryInfo di = new DirectoryInfo(QMediator.PathToRegDest);
            FileInfo[] fArr = di.GetFiles("imp.xls*");
            if (fArr.Length < 1)
            {
                _report = string.Format("В папке {0} не найден файл imp.xls* ", di.FullName);
                ReportUpdated?.Invoke(UpdateResultReport());
                return;
            }
            string pathToImp = fArr[0].FullName;
            int xlsCollCount = ManagerXls.GetFileCollCount(pathToImp);
            FileInfo[] csvArr = GetCsvList();
            List<FileInfo> csvCheckedList = GetCheckedList(csvArr, xlsCollCount);

            csvCheckedList.Sort((x, y) => DateTime.Compare(y.LastWriteTime, x.LastWriteTime));

            FileInfo resultFile = csvCheckedList[0];            

            if (resultFile.DirectoryName != QMediator.PathToRegDest)
            {
                File.Copy(resultFile.FullName, QMediator.PathToRegDest + "\\" + resultFile.Name, true);
                _report += "Скопирован, подходящий по размеру контрол, из " + resultFile.FullName + Environment.NewLine;
            }
            else
            {
                _report += "Текущий контрол подходит по размеру." + Environment.NewLine;
            }

            //_report += "путь к екселю: " + pathToImp + Environment.NewLine;
            _report += "С толбцов в экселе: " + xlsCollCount + Environment.NewLine;
            _report += "Строк в контроле: " + GetCsvRowCount(resultFile);

            //foreach (var item in csvCheckedList)
            //{
            //    _report += (item.FullName + "\t" + "в контроле строк: " + GetCsvRowCount(item) + "   " + item.LastWriteTime + Environment.NewLine);
            //}

            ReportUpdated?.Invoke(UpdateResultReport());
        }

        private static List<FileInfo> GetCheckedList(FileInfo[] csvArr, int rowCount)
        {
            List<FileInfo> resList = new List<FileInfo>();

            foreach (var item in csvArr)
            {
                if (GetCsvRowCount(item) == rowCount)
                {
                    resList.Add(item);
                }
            }
            return resList;
        }

        private static int GetCsvRowCount(FileInfo item)
        {
            int count = 0;
            bool countFlag = false;
            string[] lines = File.ReadAllLines(item.FullName);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length < 1)    continue;
                if (lines[i][0] == '-' && lines[i][1] == '-')   continue;
                if (lines[i][0] == '(') countFlag = true;
                if (lines[i].ToLower().Contains("end_of_reg"))  countFlag = false;
                if (countFlag)  count++;
            }
            return count;
        }

        private static FileInfo[] GetCsvList()
        {            
            string path = QMediator.PathToRegDest.Substring(0, QMediator.PathToRegDest.LastIndexOf("\\"));
            DirectoryInfo di = new DirectoryInfo(path);
            return di.GetFiles("*.ctl", SearchOption.AllDirectories);
        }
        #endregion



        private static string UpdateResultReport()
        {
            return Environment.NewLine + "   " + _report.Replace(Environment.NewLine, Environment.NewLine + "   ");
        }
    }
}
