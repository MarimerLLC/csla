Imports System.Configuration

''' <summary>
''' This is the base class from which command 
''' objects will be derived.
''' </summary>
''' <remarks>
''' <para>
''' Command objects allow the execution of arbitrary server-side
''' functionality. Most often, this involves the invocation of
''' a stored procedure in the database, but can involve any other
''' type of stateless, atomic call to the server instead.
''' </para><para>
''' To implement a command object, inherit from CommandBase and
''' override the DataPortal_Update method. In this method you can
''' implement any server-side code as required.
''' </para><para>
''' To pass data to/from the server, use instance variables within
''' the command object itself. The command object is instantiated on
''' the client, and is passed by value to the server where the 
''' DataPortal_Update method is invoked. The command object is then
''' returned to the client by value.
''' </para>
''' </remarks>
<Serializable()> _
Public Class CommandBase

#Region " Data access "

  Private Sub DataPortal_Create(ByVal Criteria As Object)
    Throw New NotSupportedException(GetResourceString("CreateNotSupportedException"))
  End Sub

  Private Sub DataPortal_Fetch(ByVal Criteria As Object)
    Throw New NotSupportedException(GetResourceString("FetchNotSupportedException"))
  End Sub

  ''' <summary>
  ''' Override this method to allow insert, update or deletion of a business
  ''' object.
  ''' </summary>
  Protected Overridable Sub DataPortal_Update()
    Throw New NotSupportedException(GetResourceString("UpdateNotSupportedException"))
  End Sub

  Private Sub DataPortal_Delete(ByVal Criteria As Object)
    Throw New NotSupportedException(GetResourceString("DeleteNotSupportedException"))
  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal prior to calling the 
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  Protected Overridable Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  Protected Overridable Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Returns the specified database connection string from the application
  ''' configuration file.
  ''' </summary>
  ''' <remarks>
  ''' The database connection string must be in the <c>appSettings</c> section
  ''' of the application configuration file. The database name should be
  ''' prefixed with 'DB:'. For instance, <c>DB:mydatabase</c>.
  ''' </remarks>
  ''' <param name="DatabaseName">Name of the database.</param>
  ''' <returns>A database connection string.</returns>
  Protected Function DB(ByVal DatabaseName As String) As String
    Return ConfigurationSettings.AppSettings("DB:" & DatabaseName)
  End Function

#End Region

End Class
