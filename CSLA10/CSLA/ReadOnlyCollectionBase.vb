Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Configuration

''' <summary>
''' This is the base class from which readonly collections
''' of readonly objects should be derived.
''' </summary>
<Serializable()> _
Public MustInherit Class ReadOnlyCollectionBase
  Inherits CSLA.Core.BindableCollectionBase

  Implements ICloneable

  Public Sub New()
    AllowEdit = False
    AllowNew = False
    AllowRemove = False
  End Sub

#Region " Remove, Clear, Set "

  ''' <summary>
  ''' Indicates that the collection is locked, so insert, remove
  ''' and change operations are disallowed.
  ''' </summary>
  Protected Locked As Boolean = True

  ''' <summary>
  ''' Prevents insertion of new items into the collection when the
  ''' collection is locked.
  ''' </summary>
  Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)
    If Locked Then
      Throw New NotSupportedException("Insert is invalid for a read-only collection")
    End If
  End Sub

  ''' <summary>
  ''' Prevents removal of items from the collection when the
  ''' collection is locked.
  ''' </summary>
  Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)
    If Locked Then
      Throw New NotSupportedException("Remove is invalid for a read-only collection")
    End If
  End Sub

  ''' <summary>
  ''' Prevents clearing the collection when the
  ''' collection is locked.
  ''' </summary>
  Protected Overrides Sub OnClear()
    If Locked Then
      Throw New NotSupportedException("Clear is invalid for a read-only collection")
    End If
  End Sub

  ''' <summary>
  ''' Prevents changing an item reference when the 
  ''' collection is locked.
  ''' </summary>
  Protected Overrides Sub OnSet(ByVal index As Integer, ByVal oldValue As Object, ByVal newValue As Object)
    If Locked Then
      Throw New NotSupportedException("Items can not be changed in a read-only collection")
    End If
  End Sub

#End Region

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
