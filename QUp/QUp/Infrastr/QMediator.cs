using QUp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUp.Infrastr
{
    public static class QMediator
    {
        static string _pathToRegSource = @"X:\Реєстри\Default!!!";
        public static string PathToRegSource { get { return _pathToRegSource; } set { _pathToRegSource = value; } }
        public static string PathToRegDest { get; set; }

        static string _pathToProgSource = @"X:\Progs\Registry\Default!!!";
        public static string PathToProgSource { get { return _pathToProgSource; } set { _pathToProgSource = value; } }
        public static string PathToProgDest { get; set; }

        public static string ResultReport { get; set; }     
        
        public static bool IsAuto { get; set; }

        public static TaskName CurrentTaskName { get; set; }

        public static MashinState CurrentState { get; set; }
    }
}
