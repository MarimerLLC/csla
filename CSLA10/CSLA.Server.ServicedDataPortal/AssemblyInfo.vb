Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.EnterpriseServices

' General Information about an assembly is controlled through the following 
' set of attributes. Change these attribute values to modify the information
' associated with an assembly.

' Review the values of the assembly attributes

<Assembly: AssemblyTitle("")> 
<Assembly: AssemblyDescription("")> 
<Assembly: AssemblyCompany("")> 
<Assembly: AssemblyProduct("")> 
<Assembly: AssemblyCopyright("")> 
<Assembly: AssemblyTrademark("")> 
<Assembly: CLSCompliant(True)> 

'The following GUID is for the ID of the typelib if this project is exposed to COM
<Assembly: Guid("A0547DE0-0EA3-4B98-AD4C-AD3759C14A96")> 

' Version information for an assembly consists of the following four values:
'
'      Major Version
'      Minor Version 
'      Build Number
'      Revision
'
' You can specify all the values or you can default the Build and Revision Numbers 
' by using the '*' as shown below:

<Assembly: AssemblyVersion("1.0.*")> 

' strong name
<Assembly: AssemblyKeyFile("h:\rdl\mykey.snk")> 

' EnterpriseServices settings
<Assembly: ApplicationActivation(ActivationOption.Library)> 
<Assembly: ApplicationName("CSLA DataPortal")> 
<Assembly: Description("CSLA .NET data portal")> 
<Assembly: ApplicationAccessControl(True)> 
