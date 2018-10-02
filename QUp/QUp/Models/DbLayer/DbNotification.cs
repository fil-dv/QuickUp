using Oracle.ManagedDataAccess.Client;
using QUp.Infrastr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace QUp.Models.DbLayer
{
    public static class DbNotification
    {
        #region ResultWaiter      
        
        static System.Windows.Threading.DispatcherTimer _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        static OracleConnection _con;
        public static event Action ProcDoneHandler;

        public static void ResultWaiter(OracleConnection con)
        {
            _con = con;
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

        public static string GetResultFromDb()
        {
            string res = "";
            try
            {
                string query = "select t.comments from Q_REPORT t where t.done = 1";              

                using (OracleCommand cmd = new OracleCommand(query, _con))
                {
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        res = reader[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {                
                MessageBox.Show("Exception from GetResultFromDb()" + ex.Message);
            }
            return res;
        }
        #endregion
    }
}
