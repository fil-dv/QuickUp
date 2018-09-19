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
                _regName = value.Trim();
                OnPropertyChanged();
            }
        }

        string _startDate = String.Empty;
        public string StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value.Trim();
                OnPropertyChanged();
            }
        }

        string _stopDate = String.Empty;
        public string StopDate
        {
            get { return _stopDate; }
            set
            {
                _stopDate = value.Trim();
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
            ManagerDB.ReportUpdated += ManagerDB_ReportUpdated;

            if (VerifyData())
            {
                //MessageBox.Show("Init");
                ManagerDB.RegInit(RegName, StartDate, StopDate);
            }
        }

        private void ManagerDB_ReportUpdated(string res)
        {
            ResultText = res;
        }

        private bool VerifyData()
        {
            bool res = true;            

            try
            {

                if (RegName.ToUpper()[0] != 1050 && RegName.ToUpper()[0] != 1060)
                {
                    res = false;
                    ResultText = "\n\tИмя реестра должно начинаться на буквы \"К\" или \"Ф\".";
                }
                if (!RegName.Contains("(") || !RegName.Contains(")"))
                {
                    res = false;
                    ResultText += "\n\tИмя реестра должно содержать скобки.";
                } 

                DateTime start = DateTime.Parse(StartDate);
                DateTime stop = DateTime.Parse(StopDate);
            }            
            catch (Exception ex)
            {
                res = false;
                ResultText += "\n\tНекорректный формат даты.";
            }
            return res;
        }
        #endregion


    }
}
