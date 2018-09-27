using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUp.Infrastr
{
    public static class QSettings
    {
        public static string _connentionString = "User ID=import_user;password=sT7hk9Lm;Data Source=CD_WORK";
        //public static string _connentionString = "User ID=import_user;password=sT7hk9Lm;Data Source=CD_TEST";
        public static string ConnentionString { get { return _connentionString; } }
    }
}
