using QUp.Models;
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
            ManagerFS.ReportUpdated += ReportUpdated; 
           // ManagerFS.Initialized += Initialized;
           // ManagerFS.Initialize();

            //MessageBox.Show("Pred");
            ResultText = String.Empty;
            ManagerFS.ProgsToExec(TaskName.PredProgs);
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
            MessageBox.Show("PostProgCommand");
            ResultText = String.Empty;
            ManagerFS.ProgsToExec(TaskName.PostProgs);
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
            MessageBox.Show("OktelProg");
            //ResultText = String.Empty;
            //ManagerFS.ProgsToExec(TaskName.Oktel);
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
            ResultText = String.Empty;
            ManagerWin.CreateRegInitWin();
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
            //MessageBox.Show("Fill");
            ManagerDB.StepByStep();
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
            //MessageBox.Show("Fill");
            ManagerDB.FinishCheck();
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
            //MessageBox.Show("Fill");
            ManagerDB.ToArchive();
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
            //MessageBox.Show("Fill");
            ManagerDB.StatusR();
        }
        #endregion


    }
}
