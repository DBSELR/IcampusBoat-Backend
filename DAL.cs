using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace IcampusBoatBackend
{
    public class DAL
    {
        private static string sQLServer = "DBS22\\SQLEXPRESS2022";
        private static string sQLdb = "db_ERP_Lbrce_react";
        private static string sQLpass = "12345";  
        private static string sQLUserID = "sa";

        //private static string sQLServer = @"103.92.235.226";
        //private static string sQLdb = "DB_A06577_DBS";
        //private static string sQLpass = "qazplm@123";  //"qazplm@123";
        //private static string sQLUserID = "sa";


        private static string txtmsg;

        private static string sQLConnString = "Initial Catalog= " + SQLdb + "; server= " + SQLServer + "; User Id = " + SQLUserID + "; Password=" + SQLPassWord + "; Connect Timeout= 999999;TrustServerCertificate =true;";


        public enum QueryType
        {
            SP,
            txt
        }

        public static string SQLServer
        {
            get
            {
                return sQLServer;
            }

            set
            {
                sQLServer = value;
            }
        }

        public static string SQLdb
        {
            get
            {
                return sQLdb;
            }

            set
            {
                sQLdb = value;
            }
        }

        public static string SQLPassWord
        {
            get
            {
                return sQLpass;
            }

            set
            {
                sQLpass = value;
            }
        }

        public static string SQLUserID
        {
            get
            {
                return sQLUserID;
            }

            set
            {
                sQLUserID = value;
            }
        }

        public static string SQLConnString
        {
            get
            {
                return sQLConnString;
            }

            set
            {
                sQLConnString = value;
            }
        }

        public static string TXTMSG
        {
            get
            {
                return txtmsg;
            }

            set
            {
                txtmsg = value;
            }
        }
        //public static DataTable GetData_FrmSP(SqlCommand sqlCmd, QueryType qryType)
        //{
        //    sqlCmd.CommandType = qryType == QueryType.SP ? CommandType.StoredProcedure : CommandType.Text;
        //    sqlCmd.CommandTimeout = 120;

        //    using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
        //    {
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);
        //        return dt;  


        //    }
        //}
        public static DataTable GetData_FrmSP(SqlCommand sqlCmd, QueryType qryType)
        {
            sqlCmd.CommandType = qryType == QueryType.SP
                ? CommandType.StoredProcedure
                : CommandType.Text;

            sqlCmd.CommandTimeout = 120;

            using (SqlConnection con = new SqlConnection(SQLConnString))
            {
                sqlCmd.Connection = con;

                con.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        public static DataTable GetData_FrmTxt(string query)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(query, connection))
                {
                    return GetData_FrmSP(sqlCmd, QueryType.txt);
                }
            }
        }
        public static DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            List<Dictionary<string, object>> list = new();

            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> item = new();

                foreach (DataColumn col in dt.Columns)
                {
                    item[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }

                list.Add(item);
            }

            return list;
        }

        internal static int ExecuteNonQuery(SqlCommand sqlcmd, QueryType sP)
        {
            throw new NotImplementedException();
        }

        internal static void ExecuteQuery(SqlCommand cmd, QueryType sP)
        {
            throw new NotImplementedException();
        }
    }
}
