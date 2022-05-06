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
using static PulsenicsV3.SQL_Engine;

namespace PulsenicsV3
{
    public partial class Form1 : Form
    {
        SQL_Engine init = new SQL_Engine();
        ListBox filesList = new ListBox();
        private TextBox searchBox;
        private TextBox NameBox;
        private TextBox EmailBox;
        private TextBox PhoneBox;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            /*Watches for changes in the files and runs OnFileChanged if a file does */
            var watcher = new FileSystemWatcher(@"C:\Users\natha\Desktop\pulsenics");
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnFileChanged;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            /*The below method will grab the files from the current directory and insert them to the db if they don't exist, 
             * then it will also check if the files have been modified since the app last initialized
             if they have been modified since then it will update the db with the new modified date*/
            string[] files = init.Initialize_App();


            
            filesList.Size = new System.Drawing.Size(300, 200);
            filesList.Location = new System.Drawing.Point(100, 100);
            this.Controls.Add(filesList);
            filesList.BeginUpdate();
            foreach (string name in files)
            {
                filesList.Items.Add(name);
            }
            filesList.EndUpdate();


            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchBox.AcceptsReturn = true;
            this.searchBox.AcceptsTab = true;
            this.searchBox.Location = new System.Drawing.Point(400, 100);
            this.Controls.Add(this.searchBox);
            searchBox.TextChanged += new EventHandler(searchBox_TextChanged);

            this.NameBox = new System.Windows.Forms.TextBox();
            this.NameBox.AcceptsReturn = true;
            this.NameBox.AcceptsTab = true;
            this.NameBox.Location = new System.Drawing.Point(600, 200);
            this.Controls.Add(this.NameBox);
            /*      NameBox.TextChanged += new EventHandler(NameBox_TextChanged);*/

            this.EmailBox = new System.Windows.Forms.TextBox();
            this.EmailBox.AcceptsReturn = true;
            this.EmailBox.AcceptsTab = true;
            this.EmailBox.Location = new System.Drawing.Point(600, 250);
            this.Controls.Add(this.EmailBox);
     /*       EmailBox.TextChanged += new EventHandler(EmailBox_TextChanged);*/

            this.PhoneBox = new System.Windows.Forms.TextBox();
            this.PhoneBox.AcceptsReturn = true;
            this.PhoneBox.AcceptsTab = true;
            this.PhoneBox.Location = new System.Drawing.Point(600, 300);
            this.Controls.Add(this.PhoneBox);
           /* PhoneBox.TextChanged += new EventHandler(PhoneBox_TextChanged);*/


        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            string[] oldFilesList = System.IO.Directory.GetFiles(@"C:\Users\natha\Desktop\pulsenics");
            filesList.Items.Clear();
            String searchTerm = (sender as TextBox).Text.ToLower();
            filesList.BeginUpdate();
            foreach (string name in oldFilesList)
            {
                bool included = name.ToLower().Contains(searchTerm);
                if (included)
                {
                  filesList.Items.Add(name);
                }

                
            }
            filesList.EndUpdate();


        }

   

        private void SubmitButton_Click(object sender, EventArgs e)
        {
       /*     String res = init.Submit_User(this.NameBox.Text, this.EmailBox.Text, this.PhoneBox.Text);
            if (res == "Good")
            {
                this.NameBox.Text = "";
                this.EmailBox.Text = "";
                this.PhoneBox.Text = "";
            }*/
        }

        private void assignbutton_Click(object sender, EventArgs e)
        {

            String user = usertoassignbox.Text;
            if(filesList.SelectedItem != null & user.Length > 0)
            {
                String file = filesList.SelectedItem.ToString();
                init.Assign_File(user, file);
            }
            else
            {
                MessageBox.Show("Please ensure that you have selected a file and input a user");
            };
            
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
            string[] files = init.Initialize_App();
           
        }


    }
}
