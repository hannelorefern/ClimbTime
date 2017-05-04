using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.App_Data
{
    public class TestCommand : SqlCommandInterface
    {
        string commandString { get; set; }
        Dictionary<string, Object> parameters { get; set; }
        SqlConnection conn { get; set; }

        public TestCommand()
        {
            commandString = "";
            parameters = new Dictionary<string, Object>();
        }

        public TestCommand(string s)
        {
            commandString = s;
            parameters = new Dictionary<string, Object>();
        }

        public void addParameter(string parameterName, object value)
        {
            parameters.Add(parameterName, value);
        }

        public void execute()
        {
            Debug.WriteLine("EXECUTE " + commandString + " WITH PARAMETERS {");
            foreach (KeyValuePair<string, Object> kvp in parameters)
            {
                Debug.Write(kvp.Key + " = " + kvp.Value + "; ");
            }
            Debug.WriteLine("}\n");
        }

        public SqlDataReader executeReader()
        {
            SqlCommand com = new SqlCommand(commandString, conn);
            foreach (KeyValuePair<string, Object> kvp in parameters)
            {
                com.Parameters.AddWithValue(kvp.Key, kvp.Value);
            }
            Debug.WriteLine("EXECUTE " + commandString + " WITH PARAMETERS {");
            foreach (KeyValuePair<string, Object> kvp in parameters)
            {
                Debug.Write(kvp.Key + " = " + kvp.Value + "; ");
            }
            Debug.WriteLine("}\n");
            SqlDataReader reader = com.ExecuteReader();
            Debug.WriteLine("RESULTS: ");
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Debug.Write(reader[i] + " - ");
                }
            }
            return reader;
        }

        public object executeScalar()
        {
            SqlCommand com = new SqlCommand(commandString, conn);
            foreach (KeyValuePair<string, Object> kvp in parameters)
            {
                com.Parameters.AddWithValue(kvp.Key, kvp.Value);
            }
            object ret = null;
            Debug.WriteLine("EXECUTE " + commandString + " WITH PARAMETERS {");
            foreach (KeyValuePair<string, Object> kvp in parameters)
            {
                Debug.Write(kvp.Key + " = " + kvp.Value + "; ");
            }
            Debug.WriteLine("}\n");
            Debug.WriteLine("RESULT: " + com.ExecuteScalar());
            return ret;
        }

        public void reinitialize(string newCommand, SqlConnection con)
        {
            commandString = newCommand;
            parameters = new Dictionary<string, Object>();
            conn = con;
        }

        public void isStoredProcedure()
        {
            return;
        }
    }
}
