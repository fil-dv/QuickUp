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

        #region CreateNewFiles
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
            ResultText = ManagerFS.CreateNewFiles();
        }
        #endregion

        #region CreateCtrl
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
            ResultText = ManagerFS.RunCtrlCreator();            
        }
        #endregion


        #region SplitAdress
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
            //MessageBox.Show("SplitAdress");
            ManagerFS.ReportUpdated += ReportUpdated; ;
            ManagerFS.SplitAdr();
        }

        private void ReportUpdated(string res)
        {
            ResultText = res;
        }
        #endregion
    }
}
