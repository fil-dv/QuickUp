using Oracle.ManagedDataAccess.Client;
using QUp.Infrastr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QUp.Models.DbLayer
{
    public static class DbNotification
    {
        public static bool IsNotified = false;

        public static void Start()
        {
            // To Run this sample, make sure that the change notification privilege is granted to you user.
            OracleConnection _con = null;
            OracleDependency _dep = null;

            try
            {
                _con = new OracleConnection(QSettings.ConnentionString);
                OracleCommand cmd = new OracleCommand("select * from q_report", _con);
                _con.Open();

                // Set the port number for the listener to listen for the notification request
                OracleDependency.Port = 1521;// 1005;

                // Create an OracleDependency instance and bind it to an OracleCommand instance.
                // When an OracleDependency instance is bound to an OracleCommand instance, an OracleNotificationRequest is created and is set in the
                // OracleCommand's Notification property. This indicates subsequent execution of command will register the notification.
                // By default, the notification request is using the Database Change Notification.
                _dep = new OracleDependency(cmd);

                // Add the event handler to handle the notification. The 
                // Dep_OnChange method will be invoked when a notification message
                // is received from the database
                _dep.OnChange += Dep_OnChange;

                // The notification registration is created and the query result sets 
                // associated with the command can be invalidated when there is a change.  When the first notification registration occurs, the 
                // notification listener is started and the listener port number will be 1005.
                cmd.ExecuteNonQuery();

                // Updating emp table so that a notification can be received when
                // the emp table is updated.
                // Start a transaction to update emp table
                OracleTransaction txn = _con.BeginTransaction();
                // Create a new command which will update emp table
                string updateCmdText = "update Q_REPORT t set t.done = 1";
                OracleCommand updateCmd = new OracleCommand(updateCmdText, _con);
                // Update the emp table
                updateCmd.ExecuteNonQuery();
                //When the transaction is committed, a notification will be sent from
                //the database
                txn.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            _con.Close();
            
            // Loop while waiting for notification
            while (DbNotification.IsNotified == false)
            {
                Thread.Sleep(1000);
            }
        }

        private static void Dep_OnChange(object sender, OracleNotificationEventArgs eventArgs)
        {
            Console.WriteLine("Notification Received");
            DataTable changeDetails = eventArgs.Details;
            Console.WriteLine("Data has changed in {0}", changeDetails.Rows[0]["ResourceName"]);
            DbNotification.IsNotified = true;
        }
    }
}
