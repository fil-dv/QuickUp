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
    class RegInitWindowViewModel : INotifyPropertyChanged
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

        string _regName = String.Empty;
        public string RegName
        {
            get { return _regName; }
            set
            {
                _regName = value;
                OnPropertyChanged();
            }
        }

        string _startDate = String.Empty;
        public string StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        string _stopDate = String.Empty;
        public string StopDate
        {
            get { return _stopDate; }
            set
            {
                _stopDate = value;
                OnPropertyChanged();
            }
        }

        bool _isReady = false;
        public bool IsReady
        {
            get { return _isReady; }
            set
            {
                _isReady = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region RegInitCommand
        ICommand _regInitCommand;
        public ICommand RegInitCommand
        {
            get
            {
                if (_regInitCommand == null)
                {
                    _regInitCommand = new RelayCommand(
                    p => true,
                    p => RegInit());
                }
                return _regInitCommand;
            }
        }

        void RegInit()
        {
            ResultText = String.Empty;

            if (VerifyData())
            {
                MessageBox.Show("Init");
                ManagerDB.RegInit();
            }
            else
            {
                ResultText = "\n\tВведены некорректные данные.";
            }
        }

        private bool VerifyData()
        {
            return false;
        }
        #endregion


    }
}
