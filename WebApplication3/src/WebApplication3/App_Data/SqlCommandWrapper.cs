using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.App_Data
{
    public class SqlCommandWrapper : SqlCommandInterface
    {
        SqlCommand cmd { get; set; }
        SqlConnection con;

        public SqlCommandWrapper()
        {
            cmd = new SqlCommand();
            con = null;
        }

        public SqlCommandWrapper(string command, SqlConnection conn)
        {
            cmd = new SqlCommand(command, conn);
            con = conn;
        }

        public void addParameter(string parameterName, object value)
        {
            if (value == null)
                value = DBNull.Value;
            cmd.Parameters.AddWithValue(parameterName, value);
        }

        public void execute()
        {
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public SqlDataReader executeReader()
        {
            SqlDataReader ret = cmd.ExecuteReader();
            return ret;
        }

        public object executeScalar()
        {
            con.Open();
            object ret = cmd.ExecuteScalar();
            con.Close();
            return ret;
        }

        public void reinitialize(string newCommand, SqlConnection con)
        {
            cmd = new SqlCommand(newCommand, con);
            this.con = con;
        }

        public void isStoredProcedure()
        {
            cmd.CommandType = CommandType.StoredProcedure;
        }
    }
}
