using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                throw;
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
                throw;
                //MessageBox.Show("Exception from MyLetterManager.PriorityManager.ExecCommand()" + ex.Message);
            }
        }

        public static void FirstCheckPlProc(string count)
        {
            try
            {

                OracleCommand cmd = new OracleCommand
                {
                    CommandText = "reg_upload.set_r_status_loop",
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
            

        }

    }
}
