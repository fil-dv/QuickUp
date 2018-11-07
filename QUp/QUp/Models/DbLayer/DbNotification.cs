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
    public class DbNotification : QManagerBase
    {
        #region ResultWaiter      
        
        static System.Windows.Threading.DispatcherTimer _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public static event Action ProcDoneHandler;

        public static void ResultWaiter()
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
                OracleDataReader reader = ManagerDB.GetReader(query);
                while (reader.Read())
                {
                    if (Convert.ToInt32(reader[0]) > 0)
                    {
                        _dispatcherTimer.Stop();
                        ProcDoneHandler?.Invoke();
                    }
                }                    
            }
            catch (Exception ex)
            {
                ExceptionHandler("DispatcherTimer_Tick()", ex.Message);
            }
        }       

        public static string GetResultFromDb()
        {
            string res = "";
            try
            {
                string query = "select t.comments from Q_REPORT t where t.done = 1";

                using (OracleDataReader reader = ManagerDB.GetReader(query))
                {
                    while (reader.Read())
                    {
                        res = reader[0].ToString();
                    }
                }               
            }
            catch (Exception ex)
            {
                ExceptionHandler("GetResultFromDb()", ex.Message);
            }
            return res;
        }
        #endregion
    }
}
