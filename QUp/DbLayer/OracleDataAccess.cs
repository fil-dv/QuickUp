using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.DataAccess;
using Oracle.DataAccess.Client;


namespace DbLayer
{
    public class OracleDataAccess
    {
        

        public void SetRStatus()
        {
            using (OracleConnection cn = new OracleConnection("User ID=import_user;password=sT7hk9Lm;Data Source=CD_WORK"))
            {
                //OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = cn;
                cmd.InitialLONGFetchSize = 1000;
                cmd.CommandText = "reg_upload.set_r_status_loop";
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

                
                //da.SelectCommand = cmd;               
            }
        }

        



    }
}
