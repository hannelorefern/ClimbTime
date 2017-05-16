using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.App_Data
{
    interface SqlCommandInterface
    {
        void reinitialize(string newCommand, SqlConnection con);

        void addParameter(string parameterName, object value);

        void execute();

        Object executeScalar();

        SqlDataReader executeReader();

        void isStoredProcedure();
    }
}
