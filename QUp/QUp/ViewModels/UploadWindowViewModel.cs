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



    }
}
