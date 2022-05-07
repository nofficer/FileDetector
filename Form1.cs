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
        private delegate void SafeCallDelegate(ListBox filesList, string[] files);
        SQL_Engine init = new SQL_Engine();
        ListBox filesList = new ListBox();
        private TextBox searchBox;
        private TextBox NameBox;
        private TextBox EmailBox;
        private TextBox PhoneBox;
        private Label searchLabel;
        ListBox assignedUsersList = new ListBox();
        private Label assignedUsersLabel;
        public Form1()
        {
            InitializeComponent();
        }

        private void generateFilesList(ListBox filesList, string[] files)
        {
            filesList.Size = new System.Drawing.Size(300, 200);
            filesList.Location = new System.Drawing.Point(100, 100);
            this.Controls.Add(filesList);
            filesList.Items.Clear();
            filesList.BeginUpdate();
            foreach (string name in files)
            {
                filesList.Items.Add(name);
            }
            filesList.EndUpdate();
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
            generateFilesList(filesList, files);
            filesList.SelectedIndexChanged += new EventHandler(selectedFileChanged);
            generateLabel(433, 70, searchLabel, "Search");
            searchBox = generateTextBox(405, 100, searchBox);
            searchBox.TextChanged += new EventHandler(searchBox_TextChanged);
            NameBox = generateTextBox(600, 200, NameBox);
            EmailBox = generateTextBox(600, 250, EmailBox);
            PhoneBox = generateTextBox(600, 300, PhoneBox);
        }

        private TextBox generateTextBox(int xcoord, int ycoord, TextBox boxname)
        {
            boxname = new System.Windows.Forms.TextBox();
            boxname.AcceptsReturn = true;
            boxname.AcceptsTab = true;
            boxname.Location = new System.Drawing.Point(xcoord, ycoord);
            this.Controls.Add(boxname);
            return boxname;
        }

        private Label generateLabel(int xcoord, int ycoord, Label labelname, String text)
        {
            labelname = new System.Windows.Forms.Label();
            labelname.Location = new System.Drawing.Point(xcoord, ycoord);
            labelname.Text = text;
            this.Controls.Add(labelname);
            return labelname;
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
            String res = init.Submit_User(NameBox.Text, EmailBox.Text, PhoneBox.Text);
            if (res == "Good")
            {
                this.NameBox.Text = "";
                this.EmailBox.Text = "";
                this.PhoneBox.Text = "";
            }
        }
        private void assignbutton_Click(object sender, EventArgs e)
        {

            String user = usertoassignbox.Text;
            if(filesList.SelectedItem != null & user.Length > 0)
            {
                String file = filesList.SelectedItem.ToString();
                init.Assign_File(user, file);
                usertoassignbox.Text = "";
                String selectedItem = filesList.SelectedItem.ToString();
                updateAssignedUsersList(selectedItem);
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
            /*This stuff happens in another thread so I have to call it safely via a delegate*/
            if (filesList.InvokeRequired)
            {
                var d = new SafeCallDelegate(generateFilesList);
                filesList.Invoke(d, new object[] { filesList, files });
            }

        }

        private void selectedFileChanged(object sender, System.EventArgs e)
        {
            this.Controls.Remove(assignedUsersList);
            this.Controls.Remove(assignedUsersLabel);
            this.assignedUsersList.Items.Clear();
            if(filesList.SelectedItem == null)
            {
                return;
            }
            String selectedItem = filesList.SelectedItem.ToString();
            updateAssignedUsersList(selectedItem);

        }

        
        private void updateAssignedUsersList(String selectedItem)
        {
            string[] res = init.Get_Assigned_Users(selectedItem);
            assignedUsersList.Size = new System.Drawing.Size(100, 100);
            assignedUsersList.Location = new System.Drawing.Point(405, 200);
            this.Controls.Add(assignedUsersList);
            this.assignedUsersLabel = new System.Windows.Forms.Label();
            this.assignedUsersLabel.Text = "Assigned Users";
            this.assignedUsersLabel.Location = new System.Drawing.Point(413, 170);
            this.Controls.Add(assignedUsersLabel);
            assignedUsersList.BeginUpdate();
            assignedUsersList.Items.Clear();
            foreach (String name in res)
            {
                assignedUsersList.Items.Add(name);
            }
            assignedUsersList.EndUpdate();

        }
    }
}
