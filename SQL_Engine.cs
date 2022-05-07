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
using System.Windows.Forms;

namespace PulsenicsV3
{
    internal class SQL_Engine
    {   
        /*First we declare all our needed SQL Variables in the class*/
        SqlConnection cnn;
        SqlCommand command;
        SqlDataReader dataReader;
        SqlDataAdapter adapter = new SqlDataAdapter();
        
        /*Function to establish a Sqlconnection to the database*/
        public SqlConnection GetDatabase()
        {
            string connectionString;
            connectionString = @"Server=LAPTOP-IRJMG6J3\OFFICER;Database=pulsenics;Trusted_Connection=True; User ID=sa;Password=pulsenics; MultipleActiveResultSets=true";
            cnn = new SqlConnection(connectionString);
            return cnn;
        }

        /*Initialize the app, read the chosen directory and add any new files to the db, also will check if the modified date in db matches modified date in the directory and update the db as necessary*/
        public string[] Initialize_App()
        {
            cnn = GetDatabase();
            string[] currentFilesList = System.IO.Directory.GetFiles(@"C:\Users\natha\Desktop\pulsenics");
            cnn.Open();
            foreach (String file in currentFilesList)
            {
                String creation = File.GetCreationTime(file).ToString();
                String modified = File.GetLastWriteTime(file).ToString();
                String getFileSql = "";
                getFileSql = $"Select * from files where name = @file";
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@file", file);
                command = new SqlCommand(getFileSql, cnn);
                command.Parameters.Add(param[0]);
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        
                        String dbModified = dataReader.GetValue(3).ToString();
                        if(dbModified != modified)
                        {
                            Console.WriteLine("Modified date out of sync with database, updating...");
                            String updateSql = $"Update files set modified='{modified}' where name=@file";
                            command = new SqlCommand(updateSql, cnn);
                            param[1] = new SqlParameter("@file", file);
                            command.Parameters.Add(param[1]);
                            adapter.UpdateCommand = command;
                            adapter.UpdateCommand.ExecuteNonQuery();
                            Console.WriteLine("Updated Database");
                        }
                    }
                    dataReader.Close();
                }       
                else
                {  
                    String insertFileSql = "";
                    insertFileSql = $"Insert into files (name,created,modified) VALUES (@file,'{creation}','{modified}')";
                    command = new SqlCommand(insertFileSql, cnn);
                    param[2] = new SqlParameter("@file", file);
                    command.Parameters.Add(param[2]);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                    command.Dispose();
                    dataReader.Close();
                }
                
            /*It's not in the requirements but another nice feature would be if there is a file in the db which no longer exists in the directory this file is either flagged or removed from the database*/

            };
            cnn.Close();
            return currentFilesList;
        }

        /*Assign a file to a specified user based on user text input*/
        public void Assign_File(String user, String file)
        {
            cnn.Open();
            String currentAssignedUsers = "";
            String getCurrentAssignedUsersSQl = $"Select users from files where name = '{file}'";
            command = new SqlCommand(getCurrentAssignedUsersSQl, cnn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                currentAssignedUsers = currentAssignedUsers + dataReader.GetValue(0).ToString();
            }
            dataReader.Close();
            String newAssignedUsers = currentAssignedUsers + user +",";
            
            String updateSql = $"Update files set users=@newAssignedUsers where name='{file}'";
            command = new SqlCommand(updateSql, cnn);
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@newAssignedUsers", newAssignedUsers);
            command.Parameters.Add(param[0]);
            adapter.UpdateCommand = command;
            adapter.UpdateCommand.ExecuteNonQuery();
            Console.WriteLine("Updated assigned users");
            cnn.Close();
            MessageBox.Show($"{file} assigned to {user}");

        }

        /*Saves a submitted user to the database*/
        public String Submit_User(String name, String email, String phone)
        {
            if (name.Length > 0 & email.Length > 0 & phone.Length > 0)
            {
                cnn.Open();
                String insertUserSql = "";
                insertUserSql = $"Insert into users (name,email,phone) VALUES (@name,@email,@phone)";
                command = new SqlCommand(insertUserSql, cnn);
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@name", name);
                param[1] = new SqlParameter("@email", email);
                param[2] = new SqlParameter("@phone", phone);
                command.Parameters.Add(param[0]);
                command.Parameters.Add(param[1]);
                command.Parameters.Add(param[2]);
                adapter.InsertCommand = command;
                try
                {
                    adapter.InsertCommand.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    MessageBox.Show(e.ToString());
                    return "Bad";
                }
                
                command.Dispose();
                cnn.Close();
                MessageBox.Show($"{name} added to database");
                return "Good";
            }
            else
            {
                MessageBox.Show($"Error adding {name} to database, please ensure Name, Email and Phone all have values");
                return "Bad";
            }
        }

        /*Gets all the users assigned to a particular file*/
        public string[] Get_Assigned_Users(String file)
        {
            cnn.Open();
            String getAssignedUsersSql = $"Select users from files where name = '{file}'";
            String assignedUsers = "";
            command = new SqlCommand(getAssignedUsersSql, cnn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                assignedUsers = assignedUsers + dataReader.GetValue(0).ToString();
            }
            dataReader.Close();
            command.Dispose();
            string[] assignedUsersList = assignedUsers.Split(',');
            cnn.Close();
            return assignedUsersList;
            

        }
    }
}