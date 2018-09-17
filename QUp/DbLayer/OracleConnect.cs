using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLayer
{
    public class OracleConnect
    {
        OracleConnection _con;
        OracleCommand _cmd;
        public OracleConnect(string connectionString)
        {
            _con = new OracleConnection();
            _con.ConnectionString = connectionString;
        }
        public void OpenConnect()
        {
            try
            {
                _con.Open();
                //OracleGlobalization info = _con.GetSessionInfo();
                //System.Globalization.CultureInfo lCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                ////var ri = new System.Globalization.RegionInfo(lCultureInfo.LCID);
                ////info.Calendar = lCultureInfo.Calendar.GetType().Name.Replace("Calendar", String.Empty);
                ////info.Currency = ri.CurrencySymbol;
                ////info.DualCurrency = ri.CurrencySymbol;
                ////info.ISOCurrency = ri.ISOCurrencySymbol;
                ////info.DateFormat = lCultureInfo.DateTimeFormat.ShortDatePattern + " " + lCultureInfo.DateTimeFormat.ShortTimePattern.Replace("HH", "HH24").Replace("mm", "mi");
                ////info.DateLanguage = System.Text.RegularExpressions.Regex.Replace(lCultureInfo.EnglishName, @" \(.+\)", String.Empty);
                //string str = lCultureInfo.NumberFormat.NumberDecimalSeparator;
                //string str1 = lCultureInfo.NumberFormat.NumberGroupSeparator;
                //info.NumericCharacters = lCultureInfo.NumberFormat.NumberDecimalSeparator;// + lCultureInfo.NumberFormat.NumberGroupSeparator;
                ////info.TimeZone = String.Format("{0}:{1}", TimeZoneInfo.Local.BaseUtcOffset.Hours, TimeZoneInfo.Local.BaseUtcOffset.Minutes);
                //_con.SetSessionInfo(info);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public void CloseConnect()
        {
            _con.Close();
            _con.Dispose();
        }

        public void ExecCommand(string query)
        {
           // int res = -1;
            _cmd = new OracleCommand(query, _con);
            //res = _cmd.ExecuteNonQuery();            
            _cmd.ExecuteNonQuery();
            _cmd.Dispose();
           // return res;
        }

        public OracleDataReader GetReader(string query)
        {
            _cmd = new OracleCommand(query, _con);
            OracleDataReader reader = _cmd.ExecuteReader();
            _cmd.Dispose();
            return reader;
        }

    }
}
