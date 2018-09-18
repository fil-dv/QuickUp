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
    }
}
