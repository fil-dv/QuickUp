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
    class WindowDbFieldViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string _fieldName;
        public string FieldName
        {
            get { return _fieldName; }
            set
            {
                _fieldName = value;
                OnPropertyChanged();
            }
        }

        string _columnNumber;
        public string ColumnNumber
        {
            get { return _columnNumber; }
            set
            {
                _columnNumber = value;
                OnPropertyChanged();
            }
        }

        ICommand _getColumnNumberCommand;
        public ICommand GetColumnNumberCommand
        {
            get
            {
                if (_getColumnNumberCommand == null)
                {
                    _getColumnNumberCommand = new RelayCommand(
                    p => true,
                    p => GetColumnNumber());
                }
                return _getColumnNumberCommand;
            }
        }

        private void GetColumnNumber()
        {
            //MessageBox.Show("GetColumnNumber()");
            string number = ManagerFS.GetNumberFromCtl(FieldName);
            if (number.Any(char.IsLetter))
            {
                ColumnNumber = number;
            }
            else
            {
                ColumnNumber = ManagerXls.TranslitNumberToLetters(number);
            }
        }
    }
}
