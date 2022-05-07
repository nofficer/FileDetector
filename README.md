# PulsenicsV3
Windows Form C# Application which reads files from specified directory and saves their information into SQL DB. Watches for file changes and updates the DB according. Can create users and assign users to files.

To test for yourself you may simply update the directory variable in Form1.cs line 18 to reflect the directory you wish to act upon. You will also have to spin up a SQL database and update your connection string on line 27 of the SQL_Engine.cs . The db has two tables:

Files:
ID
name
created
modified

Users:
ID
name
email
phone
