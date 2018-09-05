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

        public static string TranslitNumberToLetters(string str)
        {
            string res = "";
            int num;
            if (int.TryParse(str, out num))
            {
                res = "Столбец № " + str + (" (" + GetLetters(num) + ")");
            }
            return res;
        }

        private static string GetLetters(int num)
        {            
            const int alfabet = 26;
            char[] arr = new char[] {'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z' };
            int size = (num > alfabet? 2 : 1);
            char[] invertRes = new char[size];
            int rest;
            for (int i = 0; i < size; i++)
            {
                rest = num % alfabet;
                num = num / alfabet;
                if (rest == 0)
                {
                    invertRes[i] = arr[arr.Length - 1];
                }
                else
                {
                    invertRes[i] = arr[rest - 1];
                }                     
            }
            string res = "";
            for (int i = invertRes.Length - 1; i >= 0; i--)
            {
                res += invertRes[i];
            }
            return res.ToUpper();
        }
    }
}
