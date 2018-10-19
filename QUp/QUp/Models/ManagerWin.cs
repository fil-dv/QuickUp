using QUp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QUp.Models
{
    public static class ManagerWin
    {
        public static void CreateDbFielNameWin()
        {
            WindowDbField win = new WindowDbField();
            win.ShowDialog();
        }

        public static void CreateRegUploadWin()
        {
            UploadWindow win = new UploadWindow();            
            win.ShowDialog();
        }

        public static void CreateRegInitWin()
        {
            RegInitWindow win = new RegInitWindow();
            win.ShowDialog();
        }

        public static void CreateBackUpWin()
        {
            BackUpWindow win = new BackUpWindow();
            win.ShowDialog();
        }        
    }
}
