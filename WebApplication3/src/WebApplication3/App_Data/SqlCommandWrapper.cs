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

        public SqlCommandWrapper()
        {
            cmd = new SqlCommand();
        }

        public SqlCommandWrapper(string command, SqlConnection conn)
        {
            cmd = new SqlCommand(command, conn);
        }

        public void addParameter(string parameterName, object value)
        {
            cmd.Parameters.AddWithValue(parameterName, value);
        }

        public void execute()
        {
            cmd.ExecuteNonQuery();
        }

        public SqlDataReader executeReader()
        {
            return cmd.ExecuteReader();
        }

        public object executeScalar()
        {
            return cmd.ExecuteScalar();
        }

        public void reinitialize(string newCommand, SqlConnection con)
        {
            cmd = new SqlCommand(newCommand, con);
        }

        public void isStoredProcedure()
        {
            cmd.CommandType = CommandType.StoredProcedure;
        }
    }
}
