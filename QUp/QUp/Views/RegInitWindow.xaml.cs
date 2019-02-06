using QUp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QUp.Views
{
    /// <summary>
    /// Interaction logic for RegInitWindow.xaml
    /// </summary>
    public partial class RegInitWindow : Window
    {
        public RegInitWindow()
        {
            InitializeComponent();
            RegInitWindowViewModel vm = new RegInitWindowViewModel();
            this.DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new Action(this.Close);
        }

        //private void Close(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}
    }
}
