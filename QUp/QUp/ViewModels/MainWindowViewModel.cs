using QUp.Infrastr;
using QUp.Models;
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

        private void ReportUpdated(string res)
        {
            ResultText = res;
        }

        private void Initialized(bool b)
        {
            IsInitialize = b;
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
            ManagerFS.ReportUpdated += ReportUpdated;
            ManagerFS.Initialized += Initialized;
            ManagerFS.CreateNewFiles();
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
            ManagerFS.RunCtrlCreator();            
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
            ManagerFS.SplitAdr();
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
            ManagerFS.ProgsToExec(TaskName.PredProgs);
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
            //ManagerFS.ProgsToExec(TaskName.Oktel);
        }
        #endregion
    }
}
