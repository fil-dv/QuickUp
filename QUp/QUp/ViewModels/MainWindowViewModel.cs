using QUp.Infrastr;
using QUp.Models;
using QUp.Models.DbLayer;
using QUp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QUp
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region properties

        string _resultText;
        public string ResultText
        {
            get { return _resultText; }
            set
            {
                _resultText = value;
                OnPropertyChanged();
            }
        }

        bool _isInitialize = false;
        public bool IsInitialize
        {
            get { return _isInitialize; }
            set
            {

                _isInitialize = value;
                OnPropertyChanged();
            }
        }

        bool _isCountEntered = false;
        public bool IsCountEntered
        {
            get { return _isCountEntered; }
            set
            {
                _isCountEntered = value;
                OnPropertyChanged();
            }
        }

        string _currentPath = String.Empty;
        public string CurrentPath
        {
            get { return _currentPath; }
            set
            {
                _currentPath = value;
                OnPropertyChanged();
            }
        }


        string _dealCount = String.Empty;
        public string DealCount
        {
            get { return _dealCount; }
            set
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(value, "[^0-9]"))
                {
                    _dealCount = String.Empty;
                    IsCountEntered = false;                    
                }
                else
                {
                    _dealCount = value;
                    IsCountEntered = true;
                }                
                OnPropertyChanged();
            }
        }

        bool _isChecked = false;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }





        private void ReportUpdated(string res)
        {
            if (res.Contains("По количеству дел и по контролу все норм."))
            {
                IsChecked = true;
            }
            ResultText = res;
        }

        private void Initialized(bool b)
        {
            IsInitialize = b;
            CurrentPath = QMediator.PathToRegDest;
        }
        #endregion

        #region InitializeCommand
        ICommand _initializeCommand;
        public ICommand InitializeCommand
        {
            get
            {
                if (_initializeCommand == null)
                {
                    _initializeCommand = new RelayCommand(
                    p => true,
                    p => Initialize());
                }
                return _initializeCommand;
            }
        }

        void Initialize()
        {
            ManagerFS.ReportUpdated += ReportUpdated;
            ManagerFS.Initialized += Initialized;
            ManagerDB.ReportUpdated += ReportUpdated;

            ManagerFS.Initialize();
            if (!String.IsNullOrEmpty(QMediator.PathToRegDest))
            {
                QLoger.AddRecordToLog("Заливаем реестр " + QMediator.PathToRegDest);
            }
        }
        #endregion

        #region CreateNewFilesCommand
        ICommand _createNewFilesCommand;
        public ICommand CreateNewFilesCommand
        {
            get
            {
                if (_createNewFilesCommand == null)
                {
                    _createNewFilesCommand = new RelayCommand(
                    p => true,
                    p => CreateNewFiles());
                }
                return _createNewFilesCommand;
            }
        }

        void CreateNewFiles()
        {
            ResultText = String.Empty;
            ManagerFS.CreateNewFiles();
            ManagerFS.RunFiles();
            QLoger.AddRecordToLog(ResultText);
        }
        #endregion

        #region CreateCtrlCommand
        ICommand _createCtrlCommand;
        public ICommand CreateCtrlCommand
        {
            get
            {
                if (_createCtrlCommand == null)
                {
                    _createCtrlCommand = new RelayCommand(
                    p => true,
                    p => CreateCtrl());
                }
                return _createCtrlCommand;
            }
        }

        void CreateCtrl()
        {
            ResultText = String.Empty;
            ManagerFS.RunCtrlCreator();
        }
        #endregion

        #region SearchControlCommand
        ICommand _searchCtrlCommand;
        public ICommand SearchCtrlCommand
        {
            get
            {
                if (_searchCtrlCommand == null)
                {
                    _searchCtrlCommand = new RelayCommand(
                    p => true,
                    p => SearchCtrl());
                }
                return _searchCtrlCommand;
            }
        }

        void SearchCtrl()
        {            
            ManagerFS.SearchCtrl();
            //MessageBox.Show("PostProgCommand");           
        }
        #endregion

        #region GetExcelPosCommand
        ICommand _getExcelPosCommand;
        public ICommand GetExcelPosCommand
        {
            get
            {
                if (_getExcelPosCommand == null)
                {
                    _getExcelPosCommand = new RelayCommand(
                    p => true,
                    p => GetExcelPos());
                }
                return _getExcelPosCommand;
            }
        }

        void GetExcelPos()
        {
            ManagerWin.CreateDbFielNameWin();          
        }
        #endregion

        #region CheckCommand
        ICommand _checkCommand;
        public ICommand CheckCommand
        {
            get
            {
                if (_checkCommand == null)
                {
                    _checkCommand = new RelayCommand(
                    p => true,
                    p => Check());
                }
                return _checkCommand;
            }
        }

        void Check()
        {
            if (IsCountEntered)
            {
                ManagerDB.PreCheck(DealCount);
                QLoger.AddRecordToLog(ResultText);
            }
            else
            {
                ResultText = "\n\n\tВведите количество заливаемых дел.";
            }   
        }
        #endregion

        #region NextCommand
        ICommand _nextCommand;
        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                {
                    _nextCommand = new RelayCommand(
                    p => true,
                    p => Next());
                }
                return _nextCommand;
            }
        }

        void Next()
        {
            //MessageBox.Show("Next");
            ManagerWin.CreateRegUploadWin();
        }
        #endregion
    }
}
