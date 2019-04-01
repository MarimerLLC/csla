Database engine
---------------
The database file is attached to LocalDB - the new dedicated version of 
SQL Express for developers. If you aren't running VS2012 or higher, chances are 
you need to download it at

LocalDB SQL 2012 32 bits (x86)
http://download.microsoft.com/download/8/D/D/8DD7BDBA-CEF7-4D8E-8C16-D9F69527F909/ENU/x86/SqlLocaLDB.MSI

LocalDB SQL 2012 64 bits (x64)
http://download.microsoft.com/download/8/D/D/8DD7BDBA-CEF7-4D8E-8C16-D9F69527F909/ENU/x64/SqlLocalDB.MSI

LocalDB SQL 2014 32 bits (x86)
https://download.microsoft.com/download/E/A/E/EAE6F7FC-767A-4038-A954-49B8B05D04EB/LocalDB%2032BIT/SqlLocalDB.msi

LocalDB SQL 2014 64 bits (x64)
https://download.microsoft.com/download/E/A/E/EAE6F7FC-767A-4038-A954-49B8B05D04EB/LocalDB%2064BIT/SqlLocalDB.msi

Connection string
-----------------
In order to have a path independent connection string, the database file is 
copied to the build folder. The side efect is that any changes you made to 
database will go away when you clean the solution or the project.
