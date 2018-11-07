﻿using QUp.Infrastr;
using QUp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QUp.ViewModels
{
    public class UploadWindowViewModel : INotifyPropertyChanged
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
        
        bool _regInit = false;
        public bool RegInit
        {
            get { return _regInit; }
            set
            {
                _regInit = value;
                OnPropertyChanged();
            }
        }

        bool _curNeedToChange = false;
        public bool CurNeedToChange
        {
            get { return _curNeedToChange; }
            set
            {
                _curNeedToChange = value;
                OnPropertyChanged();
            }
        }

        string _currentPath = QMediator.PathToRegDest;
        public string CurrentPath
        {
            get { return _currentPath; }
            set
            {
                _currentPath = value;
                OnPropertyChanged();
            }
        }

        string _initRegButtonText = "Инициализация реестра";
        public string InitRegButtonText
        {
            get { return _initRegButtonText; }
            set
            {
                _initRegButtonText = value;
                OnPropertyChanged();
            }
        }

        string _autoUpButtonText = "Автопилот";
        public string AutoUpButtonText
        {
            get { return _autoUpButtonText; }
            set
            {
                _autoUpButtonText = value;
                OnPropertyChanged();
            }
        }

        string _predProgButtonText = "Запуск предпрограмм";
        public string PredProgButtonText
        {
            get { return _predProgButtonText; }
            set
            {
                _predProgButtonText = value;
                OnPropertyChanged();
            }
        }

        string _postProgButtonText = "Запуск постпрограмм";
        public string PostProgButtonText
        {
            get { return _postProgButtonText; }
            set
            {
                _postProgButtonText = value;
                OnPropertyChanged();
            }
        }

        string _oktelButtonText = "Октел";
        public string OktelButtonText
        {
            get { return _oktelButtonText; }
            set
            {
                _oktelButtonText = value;
                OnPropertyChanged();
            }
        }

        string _backUpButtonText = "Создание backUp";
        public string BackUpButtonText
        {
            get { return _backUpButtonText; }
            set
            {
                _backUpButtonText = value;
                OnPropertyChanged();
            }
        }

        string _adrButtonText = "Разбор адресов";
        public string AdrButtonText
        {
            get { return _adrButtonText; }
            set
            {
                _adrButtonText = value;
                OnPropertyChanged();
            }
        }

        string _fillProjButtonText = "Заливка в Projects";
        public string FillProjButtonText
        {
            get { return _fillProjButtonText; }
            set
            {
                _fillProjButtonText = value;
                OnPropertyChanged();
            }
        }

        string _ccyButtonText = "Перевод в валюту кредита";
        public string CcyButtonText
        {
            get { return _ccyButtonText; }
            set
            {
                _ccyButtonText = value;
                OnPropertyChanged();
            }
        }

        string _stepButtonText = "Заливка инфо";
        public string StepButtonText
        {
            get { return _stepButtonText; }
            set
            {
                _stepButtonText = value;
                OnPropertyChanged();
            }
        }

        string _finishButtonText = "Постпроверка";
        public string FinishButtonText
        {
            get { return _finishButtonText; }
            set
            {
                _finishButtonText = value;
                OnPropertyChanged();
            }
        }

        string _archButtonText = "Списание в архив";
        public string ArchButtonText
        {
            get { return _archButtonText; }
            set
            {
                _archButtonText = value;
                OnPropertyChanged();
            }
        }

        string _rButtonText = "Статус R";
        public string RButtonText
        {
            get { return _rButtonText; }
            set
            {
                _rButtonText = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region AutoUpCommand
        ICommand _autoUpCommand;
        public ICommand AutoUpCommand
        {
            get
            {
                if (_autoUpCommand == null)
                {
                    _autoUpCommand = new RelayCommand(
                    p => true,
                    p => AutoUp());
                }
                return _autoUpCommand;
            }
        }

        void AutoUp()
        {
            try
            {
                QMediator.IsAuto = true;
                ManagerFS.TaskFinished += TaskFinished;
                ManagerDB.TaskFinished += TaskFinished;
                PredProg();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from AutoUp()" + ex.Message);
            }
        }

        private void TaskFinished(TaskName taskName)
        {
            if (QMediator.IsAuto)
            {
                bool isContinue = true;

                switch (taskName)
                {
                    case TaskName.PredProgs:
                        isContinue = CheckTaskResult("Запуск предпрограмм", "Не все предпрограммы отработали корректно, продолжаем заливку?", "отработал с ошибками.");
                        if (!isContinue)
                        {
                            QMediator.IsAuto = false;
                            return;
                        }
                        else
                        {
                            CreateBackUpAuto();
                        }
                        break;

                    case TaskName.AdrSplit:
                        FillTables();
                        break;

                    case TaskName.FillProj:
                        if (CurNeedToChange)
                        {
                            ChangeCurrency();
                        }
                        else
                        {
                            StepByStep();
                        }                        
                        break;

                    case TaskName.CurrChange:
                        StepByStep();
                        break;

                    case TaskName.StepByStep:
                        isContinue = CheckTaskResult("Заливка инфо", "Надо проанализировать результаты, продолжаем заливку?", "---");
                        if (!isContinue)
                        {
                            QMediator.IsAuto = false;
                            return;
                        }
                        else
                        {
                            PostProg();
                        }
                        break;

                    case TaskName.PostProgs:
                        isContinue = CheckTaskResult("Запуск постпрограмм", "Не все постпрограммы отработали корректно, продолжаем заливку?", "отработал с ошибками.");
                        if (!isContinue)
                        {
                            QMediator.IsAuto = false;
                            return;
                        }
                        else
                        {
                            FinishCheck();
                        }
                        break;
                    case TaskName.FinishCheck:
                        isContinue = CheckTaskResult("Окончательная проверка", "Что-то не залито или залито не корректно, продолжаем заливку?", "Есть");
                        if (!isContinue)
                        {
                            QMediator.IsAuto = false;
                            return;
                        }
                        else
                        {
                            ToArchive();
                        }
                        break;

                    case TaskName.MoveToArc:
                        OktelProg();
                        break;

                    case TaskName.Oktel:
                        StatusR();
                        break;

                    case TaskName.StateR:
                        ResultText = "Реестр успешно залит. Путь к лог-файлу:" + Path.Combine(QMediator.PathToRegDest, "_upload.log") + ".";
                        QMediator.CurrentTaskName = TaskName.Default;
                        break;

                    default:
                        ResultText = "Текущая задача не определена.";
                        break;
                }
            }
        }

        private void CreateBackUpAuto()
        {
            if (IsIceNotExists())
            {
                string backUpTableName = ManagerDB.GetBackUpName();
                string count = ManagerDB.CreateBackUp(backUpTableName);
                ResultText = "Создана таблица " + backUpTableName + ", количество записей - " + count + ".";
                if (backUpTableName.Length > 0 && count.Length > 0 && QMediator.IsAuto)
                {
                    SplitAdress();
                }
            }
            else
            {
                SplitAdress();
            }
        }

        private bool IsIceNotExists()
        {
            bool res = true;
            if (QMediator.PathToRegDest.Contains("ПУМБ") || QMediator.PathToRegDest.Contains("Simple money (Foxtrot)"))
            {
                string message = "Если требуется создание новой ICE таблицы нажмите \"YES\" \n или долейте данные в существующюю таблицу в ручную и нажмите \"NO\".";
                if (MessageBox.Show(message, "Создание backup таблицы", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    res = false;
                }                
            }
            return res;
        }

        private bool CheckTaskResult(string taskName, string message, string checkString)
        {
            if (ResultText.Contains(checkString))
            {
                if (MessageBox.Show(message, taskName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return false;
                }                
            }
            return true;
        }
        #endregion


        #region PredProgCommand
        ICommand _predProgCommand;
        public ICommand PredProgCommand
        {
            get
            {
                if (_predProgCommand == null)
                {
                    _predProgCommand = new RelayCommand(
                    p => true,
                    p => PredProg());
                }
                return _predProgCommand;
            }
        }

        void PredProg()
        {
            ResultText = String.Empty;
            ManagerFS.ProgsToExec(ExecProgsType.PredProgs);
            if (!PredProgButtonText.Contains("(выполнено)"))  PredProgButtonText += " (выполнено)";
        }

        private void ReportUpdated(string res)
        {
            ResultText = res;
            QLoger.AddRecordToLog(ResultText);
        }
        #endregion

        #region PostProgCommand
        ICommand _postProgCommand;
        public ICommand PostProgCommand
        {
            get
            {
                if (_postProgCommand == null)
                {
                    _postProgCommand = new RelayCommand(
                    p => true,
                    p => PostProg());
                }
                return _postProgCommand;
            }            
        }

        void PostProg()
        {
            //MessageBox.Show("PostProgCommand");
            ResultText = String.Empty;
            ManagerFS.ProgsToExec(ExecProgsType.PostProgs);
            if (!PostProgButtonText.Contains("(выполнено)")) PostProgButtonText += " (выполнено)"; 
        }
        #endregion

        #region OktelCommand
        ICommand _oktelProgCommand;
        public ICommand OktelProgCommand
        {
            get
            {
                if (_oktelProgCommand == null)
                {
                    _oktelProgCommand = new RelayCommand(
                    p => true,
                    p => OktelProg());
                }
                return _oktelProgCommand;
            }
        }

        void OktelProg()
        {
            //MessageBox.Show("OktelProg");
            ResultText = String.Empty;
            ManagerFS.ProgsToExec(ExecProgsType.Oktel);
            if (!OktelButtonText.Contains("(выполнено)")) OktelButtonText += " (выполнено)";
        }
        #endregion

        #region SplitAdressCommand
        ICommand _splitAdressCommand;
        public ICommand SplitAdressCommand
        {
            get
            {
                if (_splitAdressCommand == null)
                {
                    _splitAdressCommand = new RelayCommand(
                    p => true,
                    p => SplitAdress());
                }
                return _splitAdressCommand;
            }
        }

        void SplitAdress()
        {
            ResultText = String.Empty;
            ManagerFS.SplitAdr();
            if (!AdrButtonText.Contains("(выполнено)")) AdrButtonText += " (выполнено)";
        }
        #endregion

        #region CreateBackUpCommand
        ICommand _createBackUpCommand;
        public ICommand CreateBackUpCommand
        {
            get
            {
                if (_createBackUpCommand == null)
                {
                    _createBackUpCommand = new RelayCommand(
                    p => true,
                    p => CreateBackUpWin());
                }
                return _createBackUpCommand;
            }
        }

        void CreateBackUpWin()
        {
            ResultText = String.Empty;
            ManagerWin.CreateBackUpWin();
            if (!BackUpButtonText.Contains("(выполнено)")) BackUpButtonText += " (выполнено)";
        }
        #endregion

        #region RegInitWindowCommand
        ICommand _regInitWindowCommand;
        public ICommand RegInitWindowCommand
        {
            get
            {
                if (_regInitWindowCommand == null)
                {
                    _regInitWindowCommand = new RelayCommand(
                    p => true,
                    p => RegInitWindow());
                }
                return _regInitWindowCommand;
            }
        }

        void RegInitWindow()
        {
            ManagerFS.ReportUpdated += ReportUpdated;
            ManagerDB.ReportUpdated += ReportUpdated;
            ManagerDB.RegInitialized += ManagerDB_RegInitialized;
            ResultText = String.Empty;
            ManagerWin.CreateRegInitWin();            
        }

        private void ManagerDB_RegInitialized(bool obj)
        {
            RegInit = true;
            if (!InitRegButtonText.Contains("(выполнено)"))
            {
                InitRegButtonText += " (выполнено)";
            }            
        }
        #endregion

        #region FillTablesCommand
        ICommand _fillTablesCommand;
        public ICommand FillTablesCommand
        {
            get
            {
                if (_fillTablesCommand == null)
                {
                    _fillTablesCommand = new RelayCommand(
                    p => true,
                    p => FillTables());
                }
                return _fillTablesCommand;
            }
        }

        void FillTables()
        {
            //MessageBox.Show("Fill");
            ManagerDB.FillTables();
            if (!FillProjButtonText.Contains("(выполнено)")) FillProjButtonText += " (выполнено)";
        }
        #endregion

        #region StepByStepCommand
        ICommand _stepByStepCommand;
        public ICommand StepByStepCommand
        {
            get
            {
                if (_stepByStepCommand == null)
                {
                    _stepByStepCommand = new RelayCommand(
                    p => true,
                    p => StepByStep());
                }
                return _stepByStepCommand;
            }
        }

        void StepByStep()
        {
           // MessageBox.Show("StepByStep");
            ManagerDB.StepByStep();
            if (!StepButtonText.Contains("(выполнено)")) StepButtonText += " (выполнено)";
        }
        #endregion

        #region ChangeCurrencyCommand
        ICommand _changeCurrencyCommand;
        public ICommand ChangeCurrencyCommand
        {
            get
            {
                if (_changeCurrencyCommand == null)
                {
                    _changeCurrencyCommand = new RelayCommand(
                    p => true,
                    p => ChangeCurrency());
                }
                return _changeCurrencyCommand;
            }
        }

        void ChangeCurrency()
        {
            // MessageBox.Show("ChangeCurrency");
            ManagerDB.ChangeCurrency();
            if (!CcyButtonText.Contains("(выполнено)")) CcyButtonText += " (выполнено)";
        }
        #endregion

        #region FinishCheckCommand
        ICommand _finishCheckCommand;
        public ICommand FinishCheckCommand
        {
            get
            {
                if (_finishCheckCommand == null)
                {
                    _finishCheckCommand = new RelayCommand(
                    p => true,
                    p => FinishCheck());
                }
                return _finishCheckCommand;
            }
        }

        void FinishCheck()
        {
            //MessageBox.Show("FinishCheck");
            ManagerDB.FinishCheck();
            if (!FinishButtonText.Contains("(выполнено)")) FinishButtonText += " (выполнено)";
        }
        #endregion

        #region ToArchiveCommand
        ICommand _toArchiveCommand;
        public ICommand ToArchiveCommand
        {
            get
            {
                if (_toArchiveCommand == null)
                {
                    _toArchiveCommand = new RelayCommand(
                    p => true,
                    p => ToArchive());
                }
                return _toArchiveCommand;
            }
        }

        void ToArchive()
        {
            //MessageBox.Show("ToArchive");
            ManagerDB.ToArchive();
            if (!ArchButtonText.Contains("(выполнено)")) ArchButtonText += " (выполнено)";
        }
        #endregion

        #region StatusRCommand
        ICommand _statusRCommand;
        public ICommand StatusRCommand
        {
            get
            {
                if (_statusRCommand == null)
                {
                    _statusRCommand = new RelayCommand(
                    p => true,
                    p => StatusR());
                }
                return _statusRCommand;
            }
        }

        void StatusR()
        {
            //MessageBox.Show("StatusRCommand");
            ManagerDB.StatusR();
            if (!RButtonText.Contains("(выполнено)")) RButtonText += " (выполнено)";
        }
        #endregion

    }
}
