To get the solution working:
1.  Restore database back up from DatabaseBackup folder onto your SQL Server 2008 instance.
2.  Setup security for the DB if necessary
3.  Update connection string in web.config in Rolodex.Silverlight.Web project to match your server setup
4.  To run WPF project, set Rolodex.WPF as start up project
5.  To run Silverlight portion set Rolodex.Silverlight.Web as start up project.  You may also need to setup Default.aspx as your start up page.

Notes:
1.  All classes with exception of XAML views are shared between WPF and SL projects
2.  All business objects are also shared between the two.
3.  There are two separate end points in Rolodex.Silverlight.Web - one for WPF, one for SL.
4.  Projects use Composite Application Guidance for WPF and Silverlight (http://compositewpf.codeplex.com/)
5.  Projects use WPF and SL Toolkits (http://wpf.codeplex.com/ and http://silverlight.codeplex.com/)