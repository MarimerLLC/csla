Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Configuration

''' <summary>
''' This is a base class from which readonly business classes
''' can be derived.
''' </summary>
''' <remarks>
''' This base class only supports data retrieve, not updating or
''' deleting. Any business classes derived from this base class
''' should only implement readonly properties.
''' </remarks>
<Serializable()> _
Public MustInherit Class ReadOnlyBase

  Implements ICloneable

#Region " Clone "

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>A new object containing the exact data of the original object.</returns>
  Public Function Clone() As Object Implements ICloneable.Clone
    Dim buffer As New MemoryStream()
    Dim formatter As New BinaryFormatter()

    formatter.Serialize(buffer, Me)
    buffer.Position = 0
    Return formatter.Deserialize(buffer)
  End Function

#End Region

#Region " Data Access "

  Private Sub DataPortal_Create(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - create not allowed")
  End Sub

  ''' <summary>
  ''' Override this method to allow retrieval of an existing business
  ''' object based on data in the database.
  ''' </summary>
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
  Protected Overridable Sub DataPortal_Fetch(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - fetch not allowed")
  End Sub

  Private Sub DataPortal_Update()
    Throw New NotSupportedException("Invalid operation - update not allowed")
  End Sub

  Private Sub DataPortal_Delete(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - delete not allowed")
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
