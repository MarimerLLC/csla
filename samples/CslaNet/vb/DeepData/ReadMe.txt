This demo app is designed to illustrate various ways you can load data into 
CSLA .NET business objects.

To try different types of data access, change the app.config file in the 
DeepData project. Uncomment one of the entries in the <appSettings> block to try 
each of the DAL plug-ins. If all are commented out you are using standard ADO.NET 
data access like you'd find in my Expert VB/C# 2005 Business Objects books.

====================================================================================

The solution was built using Visual Studio 2008 Beta 2.

====================================================================================

The solution requires CSLA .NET 3.0.1, which you can get 
from www.lhotka.net/cslanet/download.aspx. It may work with later versions of
CSLA .NET 3.0 as well.

====================================================================================

The solution expects that the cslavb solution is in a relative path:

...\cslavb
...\DeepData

If your relative paths are the same, the solution should build without change. 
If they are NOT the same, you'll need to rereference Csla.dll in all the 
projects to use your new location.

Make sure to build the cslavb solution before attempting to build DeepData.

====================================================================================

YOU CAN USE THIS PROJECT IN VISUAL STUDIO 2005. To do so:

1. Open DeepData.sln in a text editor
2. Change the Format Version to 9.00 (first line of the file)
      "Microsoft Visual Studio Solution File, Format Version 9.00"
3. Open the solution in VS 2005
4. Remove the DeepData.DAL.DLinq project from the solution
5. Build the solution