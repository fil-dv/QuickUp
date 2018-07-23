//using Microsoft.Win32;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using QUp.Infrastr;
using System.Text.RegularExpressions;

namespace QUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {       

        public MainWindow()
        {
            InitializeComponent();
            InitPath();
            FindSource();
        }

        private void FindSource()
        {
            int index = QSettings.PathToReg.LastIndexOfAny(new char[] { '\\' });
            string pathToParent = QSettings.PathToReg.Substring(0,index);
            string[] dirs = Directory.GetDirectories(pathToParent);
            List<DirectoryInfo> dirList = new List<DirectoryInfo>();
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i].Contains("  "))
                {
                    DirectoryInfo directory = new DirectoryInfo(dirs[i]);
                    dirList.Add(directory);
                }
            }
            dirList.Sort((x, y) => DateTime.Compare(x.CreationTime, y.CreationTime));
            DirectoryInfo sourceDir = null;
            if (dirList[dirList.Count - 2] != null)
            {
                sourceDir = dirList[dirList.Count - 2];
                CopyFiles(sourceDir);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Не найдена папка для копирования файлов.");
                return;
            }
        }

        private void CopyFiles(DirectoryInfo sourceDir)
        {
            FileInfo[] files = sourceDir.GetFiles();
            List<FileInfo> listToCopy = new List<FileInfo>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Extension.ToLower() == ".bat" ||
                    files[i].Extension.ToLower() == ".ctl" ||
                    files[i].Extension.ToLower() == ".txt")
                {
                    listToCopy.Add(files[i]);
                }
            }
            DirectoryInfo destDir = new DirectoryInfo(QSettings.PathToReg);
            foreach (var item in listToCopy)
            {
                File.Copy(item.FullName, destDir + "\\" +item.Name, true);
            }
        }

        private void InitPath()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                //fbd.SelectedPath = @"x:\Реєстри\ЄАПБ (Факторинг)\";
                fbd.SelectedPath = @"x:\Реєстри\ЦиФРа\999  5555 Test";
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    QSettings.PathToReg = fbd.SelectedPath; 
                }
            }
        }

        private void OpenDir(object sender, RoutedEventArgs e)
        {
            InitPath();
        }
    }
}
