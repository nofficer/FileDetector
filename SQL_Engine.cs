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
    internal class SQL_Engine
    {
        SqlConnection cnn;

        public SqlConnection GetDatabase()
        {
            string connectionString;
            

            connectionString = @"Server=LAPTOP-IRJMG6J3\OFFICER;Database=pulsenics;Trusted_Connection=True; User ID=sa;Password=pulsenics";

            cnn = new SqlConnection(connectionString);
            return cnn;
        }

        public string[] Initialize_App()
        {
            cnn = GetDatabase();
            SqlCommand command;
            SqlDataReader dataReader;
            String getFilesSql = "";
            getFilesSql = $"Select * from files";
            command = new SqlCommand(getFilesSql, cnn);
            dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {

            }
        }
    }
}