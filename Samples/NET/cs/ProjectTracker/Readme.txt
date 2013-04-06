This solution includes numerous projects, some with dependencies.

The solution requires Visual Studio 2010 SP1 and .NET 4.

The solution requires CSLA 4 version 4.1 (required assemblies are in the
Samples\Dependencies folder).

The WcfAppServer project is configured to run in IIS Express. You must have
IIS Express installed, or reconfigure the project to run in full IIS.

The ProjectTracker.Library.Sl project requires the Silverlight 4 SDK and tools.

The ProjectTracker.Library.Wp project requires the WP7 SDK and tools.

The Mvc3UI project requires ASP.NET MVC 3, and a version of Csla.Web.Mvc.dll
compiled against MVC 3 (required assemblies are in the Samples\Dependencies 
folder).

Use of the ProjectTracker.Dal.DalEf data access implementation requires
SQL Server 2008 R2 - at least Express or Developer Edition.

Pre-defined user accounts:
User:        admin
Password:    admin
Description: User in the Admin role

User:        manager
Password:    manager
Description: User in the ProjectManager role
