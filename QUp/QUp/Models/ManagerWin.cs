using QUp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QUp.Models
{
    class ManagerWin
    {
        public void CreateDbFielNameWin()
        {
            WindowDbField win = new WindowDbField();
            //ElementHost.EnableModelessKeyboardInterop(win);

            win.ShowDialog();
        }
    }
}
