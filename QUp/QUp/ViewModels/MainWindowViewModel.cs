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
            ManagerFS.CreateNewFiles();           
            ResultText = QMediator.ResultReport;
        }

        
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
            ResultText = QMediator.ResultReport;            
        }
    }
}
