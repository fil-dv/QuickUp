using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
                OracleGlobalization info = _con.GetSessionInfo();
                info.DateFormat = "dd.mm.yyyy";
                _con.SetSessionInfo(info);
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
        #endregion

        #region RegInit
        public static void RegInit(string regName, string startDate, string stopDate)
        {
            try
            {
                //ProcDoneHandler += ManagerDB_ProcDoneHandler;
                ResultWaiter();

                DateTime start = DateTime.Parse(startDate);              

                 using (OracleCommand cmd = new OracleCommand("reg_upload.initReg", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("regLongName", OracleDbType.Varchar2).Value = regName;
                    cmd.Parameters.Add("payStart", OracleDbType.Date).Value = start;
                    if (!string.IsNullOrEmpty(stopDate))
                    {
                        DateTime stop = DateTime.Parse(stopDate);
                        cmd.Parameters.Add("payStop", OracleDbType.Date).Value = stop;
                    }
                    else
                    {
                        cmd.Parameters.Add("payStop", OracleDbType.Date).Value = null;
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
                //ProcDoneHandler += ManagerDB_ProcDoneHandler;
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
                //ProcDoneHandler += ManagerDB_ProcDoneHandler;
                ResultWaiter();

                using (OracleCommand cmd = new OracleCommand("reg_upload.step_by_step", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
                //ProcDoneHandler += ManagerDB_ProcDoneHandler;
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
                //ProcDoneHandler += ManagerDB_ProcDoneHandler;
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
                //ProcDoneHandler += ManagerDB_ProcDoneHandler;
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

        #region CreateBackUp
        public static string GetBackUpName()
        {
            string name = "";
            try
            {
                string regNumbers = GetRegNumbers();

                if (QMediator.PathToRegDest.Contains("Факторинг"))
                {
                    name = "ice_f_" + regNumbers;
                }
                else
                {
                    string stopDate = GetStopDate();
                    string updStopDate = UpdateStopDate(stopDate);
                    string alias = GetAlias();
                    name = "ice_" + alias + "_" + updStopDate + "_" + regNumbers;
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from GetBackUpName()" + ex.Message); 
            }
            return name;
        }

        private static string GetRegNumbers()
        {
            string regNums = "";
            try
            {
                string str = Path.GetFileName(QMediator.PathToRegDest).Replace(' ', '_').Replace('-', '_');
                if (str.Contains("__")) str = str.Replace("__", "_");
                regNums = str;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from GetRegNumbers()" + ex.Message); 
            }
            return regNums;
        }

        private static string UpdateStopDate(string date)
        {
            string res = "";
            try
            {
                string str = date.Substring(0, date.IndexOf(' '));
                str.IndexOf("/20", 2);
                string year = str.Substring((str.IndexOf("/20") + 3), 2);
                string month = str.Substring(0, 2);
                res = year + month;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from UpdateStopDate()" + ex.Message);
            }
            return res;
        }

        private static string GetStopDate()
        {
            string stopDate = "";
            try
            {
                using (OracleCommand cmd = new OracleCommand("reg_upload.get_stop_date", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("g_v_reg_stop_date", OracleDbType.Date);
                    cmd.Parameters["g_v_reg_stop_date"].Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();

                    stopDate = Convert.ToString(cmd.Parameters["g_v_reg_stop_date"].Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from GetStopDate()" + ex.Message);
            }
            return stopDate;
        }

        private static string GetAlias()
        {
            string alias = "";
            try
            {
                using (OracleCommand cmd = new OracleCommand("select reg_upload.get_alias from dual", _con))
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        alias = Convert.ToString(reader[0]);
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from GetAlias()" + ex.Message);
            }
            return alias;
        }


        //private static string GetAlias()
        //{
        //    string alias = "";
        //    try
        //    {
        //        using (OracleCommand cmd = new OracleCommand("reg_upload.get_alias", _con))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("v_alias", OracleDbType.Varchar2);
        //            cmd.Parameters["v_alias"].Direction = ParameterDirection.ReturnValue;
        //            cmd.ExecuteNonQuery();

        //            alias = Convert.ToString(cmd.Parameters["v_alias"].Value);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Exception from GetAlias()" + ex.Message);
        //    }
        //    return alias;
        //}

        public static string CreateBackUp(string tableName)
        {
            string recordCount = "";

            try
            {
                using (OracleCommand cmd = new OracleCommand("create  table " + tableName + " as select* from import_clnt_example", _con))
                {
                    cmd.ExecuteNonQuery();
                }
                using (OracleCommand cmd = new OracleCommand("select count(*) from " + tableName, _con))
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        recordCount = Convert.ToString(reader[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from CreateBackUp()" + ex.Message);
            }

            return recordCount;
        }
        #endregion

        #region ChangeCurrency
        public static void ChangeCurrency()
        {
            try
            {
                ResultWaiter();

                using (OracleCommand cmd = new OracleCommand("reg_upload.ccy_update", _con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception from ChangeCurrency()" + ex.Message);
            }
        }
        #endregion

        private static string UpdateResultReport()
        {
            return Environment.NewLine + "   " + _report.Replace(Environment.NewLine, Environment.NewLine + "   ");
        }
    }
}
