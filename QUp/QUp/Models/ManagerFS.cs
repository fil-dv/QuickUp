using QUp.Infrastr;
using QUp.Models.DbLayer;
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
    public enum TaskName { PredProgs, BackUp, AdrSplit, FillProj, CurrChange, StepByStep, PostProgs, FinishCheck, MoveToArc, Oktel, StateR, NoTask };
    public enum MashinState { PredProgsState, BackUpState, AdrSplitState, FillProjState, CurrChangeState, StepByStepState, PostProgsState, FinishCheckState, MoveToArcState, OktelState, StatusRState, StopState };
    enum FileType { Reg, Prog };
    public enum ExecProgsType { PredProgs, PostProgs, Oktel };

     public class ManagerFS : QManagerBase
    {
        static string _report = "";
        static public event Action SplitCompletHandler;
        static public event Action<bool> Initialized;        
        public static event Action<string> ReportUpdated;
        static public event Action<TaskName> TaskFinished;

        #region InitializeApp
        static public void Initialize()
        {
            try
            {
                _report = "";
                using (var fbd = new FolderBrowserDialog())
                {
                    //fbd.SelectedPath = @"x:\Реєстри\ЄАПБ (Факторинг)\";
                    fbd.SelectedPath = @"x:\Реєстри\ЯЯЯTest\1  1111";
                    DialogResult result = fbd.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        QMediator.PathToRegDest = fbd.SelectedPath;
                        QMediator.PathToProgDest = QMediator.PathToRegDest.Replace("Реєстри", "Progs\\Registry");
                        Initialized?.Invoke(true);
                    }
                }
                ReportUpdated?.Invoke(_report);
            }
            catch (Exception ex)
            {
                ExceptionHandler("Initialize()", ex.Message);
            }            
        }
        #endregion

        #region creating files
        
        public static void CreateNewFiles()
        {
            try
            {
                Directory.CreateDirectory(QMediator.PathToProgDest);
                FindSource(QMediator.PathToRegDest, FileType.Reg);
                FindSource(QMediator.PathToProgDest, FileType.Prog);

                ReportUpdated?.Invoke(_report);
            }
            catch (Exception ex)
            {
                ExceptionHandler("CreateNewFiles()", ex.Message);
            }
        }

        static void FindSource(string path, FileType fileType )
        {
            try
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
            catch (Exception ex)
            {
                ExceptionHandler("FindSource()", ex.Message);
            }            
        }

        static void CopyRegFiles()
        {
            try
            {
                DirectoryInfo sourceDir = new DirectoryInfo(QMediator.PathToRegSource);
                FileInfo[] files = sourceDir.GetFiles();
                List<FileInfo> listToCopy = new List<FileInfo>();
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Extension.ToLower() == ".bat" ||
                        files[i].Extension.ToLower() == ".ctl" ||
                        files[i].Extension.ToLower() == ".txt" ||
                        files[i].Extension.ToLower() == ".exe")
                    {
                        listToCopy.Add(files[i]);
                    }
                }
                DirectoryInfo destDir = new DirectoryInfo(QMediator.PathToRegDest);

                _report += Environment.NewLine + "Копируем файлы из " + Environment.NewLine + "   " + sourceDir + Environment.NewLine + "в " + destDir + Environment.NewLine + Environment.NewLine;

                foreach (var item in listToCopy)
                {
                    File.Copy(item.FullName, destDir + "\\" + item.Name, true);
                    _report += item.Name + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("CopyRegFiles()", ex.Message);
            }            
        }

        static void CopyProgFiles()
        {
            try
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
            catch (Exception ex)
            {
                ExceptionHandler("CopyProgFiles()", ex.Message);
            }
        }

        public static void RunFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(QMediator.PathToRegDest);
            FileInfo[] xlsFiles = dir.GetFiles("*.xls*");
            if (xlsFiles.Length > 0)
            {
                Process.Start(xlsFiles[0].FullName);
            }
            FileInfo[] ctlFiles = dir.GetFiles("*.ctl");
            if (ctlFiles.Length > 0)
            {
                Process.Start(ctlFiles[0].FullName);
            }
        }
        #endregion

        #region RunControlCreator
        public static void RunCtrlCreator()
        {
            try
            {
                //_report = "Создание контрола...";
                ReportUpdated?.Invoke(_report);

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

                if (File.Exists(@"x:\utils\Control-creator\c-creator\bin\Release\c-creator.exe"))
                {
                    pInfo.WorkingDirectory = @"x:\utils\Control-creator\c-creator\bin\Debug\";
                }                

                Process p = Process.Start(pInfo);
                ReportUpdated?.Invoke(_report);
            }
            catch (Exception ex)
            {
                ExceptionHandler("RunCtrlCreator", ex.Message);
            }            
        }
        #endregion

        #region Split adress
        public static void SplitAdr()
        {
            try
            {
                _report = "\n\n\t  Разбивка адресов...";
                ReportUpdated?.Invoke(_report);

                SplitCompletHandler += UpdateReport;

                ProcessStartInfo pInfo = new System.Diagnostics.ProcessStartInfo(@"x:\utils\AdressSpliter\Adr\Adr\bin\Debug\Adr.exe");
                if (QMediator.PathToRegDest != null)
                {
                    pInfo.Arguments = ("\"" + QMediator.PathToRegDest + "\"");
                }
                Process process = Process.Start(pInfo);
                process.EnableRaisingEvents = true;
                process.Exited += SplitFinished;
            }
            catch (Exception ex)
            {                
                ExceptionHandler("SplitAdr()", ex.Message);
            }            
        }        

        private static void SplitFinished(object sender, EventArgs e)
        {
            SplitCompletHandler?.Invoke();
        }

        private static void UpdateReport()
        {
            try
            {
                _report = ReadLog();
                ReportUpdated?.Invoke(_report);
                TaskFinished?.Invoke(TaskName.AdrSplit);
            }
            catch (Exception ex)
            {
                ExceptionHandler("UpdateReport()", ex.Message);
            }            
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
            try
            {                
                int index = str.LastIndexOf("------------------");
                str = str.Substring(index + 18);               
            }
            catch (Exception ex)
            {
                ExceptionHandler("ReadLog()", ex.Message);
            }
            return str;
        }
        #endregion

        #region ProgsToExexute
        public static void ProgsToExec(ExecProgsType taskName)
        {
            try
            {
                _report = "Выполнение программ...\n\n";
                ReportUpdated?.Invoke(_report);
                if (QMediator.PathToProgDest != null)
                {
                    string str;
                    if (taskName == ExecProgsType.Oktel)
                    {
                        str = "\\post!\\oktel";
                        QMediator.CurrentTaskName = TaskName.Oktel;
                        DbNotification.ResultWaiter();
                    }
                    else
                    {
                        str = taskName == ExecProgsType.PredProgs ? "\\!pred" : "\\post!";
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
                            ReportUpdated?.Invoke(_report);

                            if (taskName == ExecProgsType.PredProgs) TaskFinished?.Invoke(TaskName.PredProgs);
                            else if (taskName == ExecProgsType.PostProgs) TaskFinished?.Invoke(TaskName.PostProgs);
                            else TaskFinished?.Invoke(TaskName.Oktel);

                            return;
                        }
                        foreach (var item in filePathList)
                        {                           
                            _report += Environment.NewLine + "Файл:\t\t" + item;
                            bool isOk = true;
                            string fileText = File.ReadAllText(item, Encoding.Default);
                            try
                            {
                                if (fileText.ToLower().Contains("begin"))
                                {
                                    ManagerDB.ExecCommand(fileText);
                                }
                                else
                                {
                                    isOk = SplitAndExec(fileText);
                                }
                            }
                            catch (Exception ex)
                            {
                                isOk = false;
                                ExceptionHandler("ProgsToExec()", ex.Message);
                            }                            
                            _report += ("\t" + (isOk == true ? "отработал нормально." : "отработал с ошибками.") + Environment.NewLine);
                        }
                    }
                }
                else
                {                   
                    _report = "Не определен путь к программам.";
                }

                ReportUpdated?.Invoke(_report);
                if (taskName == ExecProgsType.PredProgs) TaskFinished?.Invoke(TaskName.PredProgs);
                else if (taskName == ExecProgsType.PostProgs) TaskFinished?.Invoke(TaskName.PostProgs);
                else TaskFinished?.Invoke(TaskName.Oktel);
            }
            catch (Exception ex)
            {
                ExceptionHandler("ProgsToExec()", ex.Message);
            }            
        }

        private static bool SplitAndExec(string fileText)
        {
            try
            {
                bool isOk = true;
                List<string> queryList = SplitQuery(fileText);
                foreach (var query in queryList)
                {
                    try
                    {
                        ManagerDB.ExecCommand(query);
                    }
                    catch (Exception)
                    {
                        isOk = false;
                    }
                }
                return isOk;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        //private static List<string> SplitFile(string path)
        //{
        //    List<string> listStr = new List<string>();
        //    try
        //    {
        //        string fileText = File.ReadAllText(path, Encoding.Default);
        //        string[] queries = fileText.Trim().Split(';');                

        //        foreach (var item in queries)
        //        {
        //            if (item.Trim().Length > 0)
        //            {
        //                listStr.Add(item.Trim());
        //            }
        //        }               
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler("SplitFile()", ex.Message);      
        //    }
        //    return listStr;
        //}

        private static List<string> SplitQuery(string query)
        {
            List<string> listStr = new List<string>();
            try
            {
                //string fileText = File.ReadAllText(path, Encoding.Default);
                string[] queries = query.Trim().Split(';');

                foreach (var item in queries)
                {
                    if (item.Trim().Length > 0)
                    {
                        listStr.Add(item.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("SplitQuery()", ex.Message);                
            }
            return listStr;
        }

        private static List<string> GetFilesForExec(string path)
        {
            List<string> fileList = new List<string>();
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] fiArr = di.GetFiles();                
                foreach (var item in fiArr)
                {
                    if (!item.Name.Contains("--"))
                    {
                        fileList.Add(item.FullName);
                    }
                }               
            }
            catch (Exception ex)
            {
                ExceptionHandler("GetFilesForExec()", ex.Message);
            }
            return fileList;
        }
        #endregion

        #region SearchCtrl
        public static void SearchCtrl()
        {
            try
            {
                _report = "Подбор контрола..." + Environment.NewLine;
                ReportUpdated?.Invoke(_report);

                DirectoryInfo di = new DirectoryInfo(QMediator.PathToRegDest);
                FileInfo[] fArr = di.GetFiles("imp.xls*");
                if (fArr.Length < 1)
                {
                    _report = string.Format("В папке {0} не найден файл imp.xls* ", di.FullName);
                    ReportUpdated?.Invoke(_report);
                    return;
                }
                string pathToImp = fArr[0].FullName;
                int xlsCollCount = ManagerXls.GetFileCollCount(pathToImp);
                FileInfo[] csvArr = GetCsvList();
                List<FileInfo> csvCheckedList = GetCheckedList(csvArr, xlsCollCount);

                if (csvCheckedList.Count < 1)
                {
                    _report += "Нет подходящего контрола.";
                    ReportUpdated?.Invoke(_report);
                    return;
                }

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

                ReportUpdated?.Invoke(_report);
            }
            catch (Exception ex)
            {
                ExceptionHandler("SearchCtrl()", ex.Message);
            }
            
        }

        private static List<FileInfo> GetCheckedList(FileInfo[] csvArr, int rowCount)
        {
            List<FileInfo> resList = new List<FileInfo>();
            try
            {
                foreach (var item in csvArr)
                {
                    if (GetCsvRowCount(item) == rowCount)
                    {
                        resList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("GetCheckedList()", ex.Message);
            }            
            return resList;
        }

        private static int GetCsvRowCount(FileInfo item)
        {
            int count = 0;
            try
            {
                bool countFlag = false;
                string[] lines = File.ReadAllLines(item.FullName);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Length < 1) continue;
                    if (lines[i][0] == '-' && lines[i][1] == '-') continue;
                    if (lines[i][0] == '(') countFlag = true;
                    if (lines[i].ToLower().Contains("end_of_reg")) countFlag = false;
                    if (countFlag) count++;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("GetCsvRowCount()", ex.Message);
            }
            return count;
        }

        private static FileInfo[] GetCsvList()
        {
            DirectoryInfo di = null;
            try
            {
                string path = QMediator.PathToRegDest.Substring(0, QMediator.PathToRegDest.LastIndexOf("\\"));
                di = new DirectoryInfo(path);                
            }
            catch (Exception ex)
            {
                ExceptionHandler("GetCsvList()", ex.Message);
            }
            return di.GetFiles("*.ctl", SearchOption.AllDirectories);
        }
        #endregion

        #region GetExcelPosition
        internal static string GetNumberFromCtl(string fieldName)
        {
            string res = "Нет результата";
            try
            {
                DirectoryInfo di = new DirectoryInfo(QMediator.PathToRegDest);
                FileInfo[] arr = di.GetFiles("*.ctl");
                if (arr.Length == 0)
                {
                    res = "Не найден файл контрола";
                    return res;
                }
                if (arr.Length > 1)
                {
                    res = "Найдено несколько файлов контрола.";
                    return res;
                }
                string[] lines = File.ReadAllLines(arr[0].FullName, Encoding.Default);
                int count = 1;
                bool startCount = false;
                List<string> list = GetCleanList(lines);                 

                foreach (var item in list)
                {   
                    if (startCount)
                    {
                        if (item.ToLower() == fieldName.ToLower().Trim())
                        {
                            return count.ToString();
                        }                            
                        else count++;
                    }
                    if (item.Trim() == "(")
                    {
                        startCount = true;
                    }
                    if (item.Trim() == ")")
                    {
                        res = " Поле " + fieldName + " не найдено в контроле.";
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("GetNumberFromCtl()", ex.Message);
            }            
            return res;
        }

        private static List<string> GetCleanList(string[] lines)
        {
            List<string> list = new List<string>();
            try
            {
                foreach (var item in lines)
                {
                    if (item.Length == 0 || (item.Length > 0 && item.Trim()[0] == '-')) continue;
                    string line = item;
                    if (line.Contains(",")) line = line.Substring(0, item.IndexOf(',')).Trim();
                    if (line.Contains(" ")) line = line.Substring(0, item.IndexOf(' ')).Trim();
                    list.Add(line);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler("GetCleanList()", ex.Message);
            }
            
            return list;
        }
        #endregion
    }
}
