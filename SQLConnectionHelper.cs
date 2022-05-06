using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulsenicsV3
{
	internal class SQLConnectionHelper
	{
		public SqlConnection GetSqlConnection
        {

        string connectionString;
        SqlConnection cnn;

        connectionString = @"Server=LAPTOP-IRJMG6J3\OFFICER;Database=pulsenics;Trusted_Connection=True; User ID=sa;Password=pulsenics";

        cnn = new SqlConnection(connectionString);
        return cnn
    }

	}
}
