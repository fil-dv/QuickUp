using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using QUp.Infrastr;

namespace QUp.Models
{
    public static class ManagerDB
    {
        static OracleConnection _con;

        static string _report = "";
        static System.Windows.Threading.DispatcherTimer _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        static event Action ProcDoneHandler;
        public static event ResultIsReady ReportUpdated;

        static ManagerDB()
        {            
            try
            {
                _con = new OracleConnection(QSettings.ConnentionString);
                _con.Open();               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from ManagerDB()" + ex.Message);
            }
        }

        public static void ExecCommand(string command)
        {
            try
            {
                using (OracleCommand cmd = new OracleCommand(command, _con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from ExecCommand()" + ex.Message);
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
        //        MessageBox.Show("Exception from ExecProc()" + ex.Message);
        //    }
        //}
        #endregion

        #region PreCheck
        public static void PreCheck(string count)
        {
            try
            {
                ProcDoneHandler += ManagerDB_ProcDoneHandler;
                ResultWaiter();

                using (OracleCommand cmd = new OracleCommand("reg_upload.first_check", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("count_of_deals", OracleDbType.Int32).Value = Convert.ToInt32(count);
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from PreCheck()" + ex.Message); 
            }
        }
        #endregion

        private static void ManagerDB_ProcDoneHandler()
        {
            GetResultFromDb();
        }

        private static void GetResultFromDb()
        {
            try
            {
                string query = "select t.comments from Q_REPORT t where t.done = 1";
                string res = "";

                using (OracleCommand cmd = new OracleCommand(query, _con))
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        res = reader[0].ToString();
                    }
                }
                
                _report = res;
                ReportUpdated?.Invoke(UpdateResultReport());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from GetResultFromDb()" + ex.Message);
            }
        }

        #region ResultWaiter
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
                                ProcDoneHandler?.Invoke();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from DispatcherTimer_Tick()" + ex.Message);
            }
        }
        #endregion

        #region RegInit
        public static void RegInit(string regName, string startDate, string stopDate)
        {
            try
            {
                ProcDoneHandler += ManagerDB_ProcDoneHandler;
                ResultWaiter();

                DateTime start = DateTime.Parse(startDate);              

                 using (OracleCommand cmd = new OracleCommand("reg_upload.initReg", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("regLongName", OracleDbType.Varchar2).Value = regName;
                    cmd.Parameters.Add("payStart", OracleDbType.Date).Value = start;
                    if (stopDate != null)
                    {
                        DateTime stop = DateTime.Parse(stopDate);
                        cmd.Parameters.Add("payStop", OracleDbType.Date).Value = stop;
                    }                  
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from RegInit()" + ex.Message);
            }
        }
        #endregion


        #region FillTables
        public static void FillTables()
        {
            try
            {
                ProcDoneHandler += ManagerDB_ProcDoneHandler;
                ResultWaiter();

                using (OracleCommand cmd = new OracleCommand("reg_upload.fill_suvd", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from FillTables()" + ex.Message);
            }
        }
        #endregion

        #region StepByStep
        public static void StepByStep()
        {
            try
            {
                ProcDoneHandler += ManagerDB_ProcDoneHandler;
                ResultWaiter();

                using (OracleCommand cmd = new OracleCommand("reg_upload.step_by_step", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    _con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from StepByStep()" + ex.Message);
            }
        }
        #endregion

        #region FinishCheck
        public static void FinishCheck()
        {
            try
            {
                ProcDoneHandler += ManagerDB_ProcDoneHandler;
                ResultWaiter();

                using (OracleCommand cmd = new OracleCommand("reg_upload.finish_check", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from FinishCheck()" + ex.Message);
            }
        }
        #endregion

        #region ToArchive
        public static void ToArchive()
        {
            try
            {
                ProcDoneHandler += ManagerDB_ProcDoneHandler;
                ResultWaiter();

                using (OracleCommand cmd = new OracleCommand("reg_upload.move_arc_and_lpd", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from ToArchive()" + ex.Message);
            }
        }
        #endregion


        #region StatusR
        public static void StatusR()
        {
            try
            {
                ProcDoneHandler += ManagerDB_ProcDoneHandler;
                ResultWaiter();

                using (OracleCommand cmd = new OracleCommand("reg_upload.set_r_status_loop", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from StatusR()" + ex.Message);
            }
        }
        #endregion


        private static string UpdateResultReport()
        {
            return Environment.NewLine + "   " + _report.Replace(Environment.NewLine, Environment.NewLine + "   ");
        }
    }
}
