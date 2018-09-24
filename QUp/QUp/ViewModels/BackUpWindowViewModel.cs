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
    class BackUpWindowViewModel : INotifyPropertyChanged
    {
        public BackUpWindowViewModel()
        {
            BackUpName = ManagerDB.GetBackUpName();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region properties
        string _backUpName;
        public string BackUpName
        {
            get { return _backUpName; }
            set
            {
                _backUpName = value;
                OnPropertyChanged();
            }
        }
        
        string _backUpReport;
        public string BackUpReport
        {
            get { return _backUpReport; }
            set
            {
                _backUpReport = value;
                OnPropertyChanged();
            }
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
                    p => CreateBackUp());
                }
                return _createBackUpCommand;
            }
        }

        void CreateBackUp()
        {
           // MessageBox.Show("CreateBackUp");
            string str = ManagerDB.CreateBackUp(BackUpName);
            if (str.Length < 1) BackUpReport = "Таблица не создана.";
            else BackUpReport = "Таблица успешно создана. Количество записей - " + str;

        }
        #endregion


    }
}
