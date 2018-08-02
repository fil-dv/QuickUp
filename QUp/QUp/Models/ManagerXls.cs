using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace QUp.Models
{
    public static class ManagerXls
    {
        static Application _xlApp;
        static Workbook _xlWorkBook;
        static Worksheet _xlWorkSheet;
        static Range _xlRange;


        public static int GetFileCollCount(string path)
        {
            int count = 0;
            _xlApp = new Application();
            _xlWorkBook = _xlApp.Workbooks.Open(path, 0, true, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            _xlWorkSheet = _xlWorkBook.Sheets[1];
            _xlRange = _xlWorkSheet.UsedRange;
            var v = _xlRange.Columns;
            count = v.Count;
            return count;
        }
    }
}
