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

        static string _report = "";
        static System.Windows.Threading.DispatcherTimer _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        static event Action PreCheckHandler;
        public static event ResultIsReady ReportUpdated;

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
        //public static void ExecProc(string procName)
        //{
        //    try
        //    {
        //        using (OracleConnection _con = new OracleConnection(QSettings.ConnentionString))
        //        {
        //            using (OracleCommand cmd = new OracleCommand(procName, _con))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add("count_of_deals", OracleDbType.Int32).Value = 1282;
        //                _con.Open();
        //                cmd.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        #endregion

        #region PreCheck
        public static void PreCheck(string count)
        {
            try
            {
                PreCheckHandler += ManagerDB_PreCheckHandler;
                ResultWaiter();

                using (OracleConnection con = new OracleConnection(QSettings.ConnentionString))
                {
                    using (OracleCommand cmd = new OracleCommand("reg_upload.first_check", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("count_of_deals", OracleDbType.Int32).Value = Convert.ToInt32(count);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void ManagerDB_PreCheckHandler()
        {
            GetResultFromDb();
        }

        private static void GetResultFromDb()
        {
            try
            {
                string query = "select t.comments from Q_REPORT t where t.done = 1";
                string res = "";
                using (OracleConnection con = new OracleConnection(QSettings.ConnentionString))
                {
                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        con.Open();
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            res = reader[0].ToString();
                        }
                    }
                }
                _report = res;
                ReportUpdated?.Invoke(UpdateResultReport());
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        private static void ResultWaiter()
        {
            
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Start();
        }

        private static void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string query = "select count(*) from Q_REPORT t where t.done = 1";
                using (OracleConnection con = new OracleConnection(QSettings.ConnentionString))
                {
                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        con.Open();
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (Convert.ToInt32(reader[0]) > 0)
                            {
                                _dispatcherTimer.Stop();
                                PreCheckHandler?.Invoke();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void RegInit()
        {

        }


        private static string UpdateResultReport()
        {
            return Environment.NewLine + "   " + _report.Replace(Environment.NewLine, Environment.NewLine + "   ");
        }
    }
}
