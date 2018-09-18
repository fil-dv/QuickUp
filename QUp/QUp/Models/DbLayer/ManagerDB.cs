using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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

        #region SqlProc
        public static void ExecProc(string procName)
        {
            try
            {
                using (OracleConnection _con = new OracleConnection(QSettings.ConnentionString))
                {
                    using (OracleCommand cmd = new OracleCommand(procName, _con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("count_of_deals", OracleDbType.Int32).Value = 1282;
                        _con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region PreCheck
        public static void PreCheck(string count)
        {
            try
            {


                using (OracleConnection _con = new OracleConnection(QSettings.ConnentionString))
                {
                    using (OracleCommand cmd = new OracleCommand("first_check", _con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("count_of_deals", OracleDbType.Int32).Value = count;
                        _con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion



    }
}
