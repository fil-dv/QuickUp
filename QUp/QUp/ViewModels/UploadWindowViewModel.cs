﻿using QUp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            //MessageBox.Show("Pred");
            ResultText = String.Empty;
            ManagerFS.ProgsToExec(TaskName.PredProgs);
            PredProgButtonText += " (выполнено)";
        }

        private void ReportUpdated(string res)
        {
            ResultText = res;
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
            ManagerFS.ProgsToExec(TaskName.PostProgs);
            PostProgButtonText += " (выполнено)";
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
            ManagerFS.ProgsToExec(TaskName.Oktel);
            OktelButtonText += " (выполнено)";
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
            AdrButtonText += " (выполнено)";
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
            BackUpButtonText += " (выполнено)";
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
            FillProjButtonText += " (выполнено)";
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
            StepButtonText += " (выполнено)";
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
            CcyButtonText += " (выполнено)";
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
            FinishButtonText += " (выполнено)";
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
            ArchButtonText += " (выполнено)";
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
            RButtonText += " (выполнено)";
        }
        #endregion

    }
}
