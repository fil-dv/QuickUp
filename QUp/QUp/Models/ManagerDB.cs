using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbLayer;
using Oracle.ManagedDataAccess.Client;
using QUp.Infrastr;

namespace QUp.Models
{
    public static class ManagerDB
    {
        static OracleConnect _con;

        static ManagerDB()
        {            
            try
            {
                _con = new OracleConnect(QSettings.ConnentionString);
                _con.OpenConnect();
            }
            catch (Exception)
            {
                //MessageBox.Show("Exception from MyLetterManager.LetterManager.CreateConnect()" + ex.Message);
            }
        }

        public static void ExecCommand(string command)
        {
            try
            {
                if (_con != null)
                {
                    _con.ExecCommand(command);
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Exception from MyLetterManager.PriorityManager.ExecCommand()" + ex.Message);
            }
        }
        
    }
}
