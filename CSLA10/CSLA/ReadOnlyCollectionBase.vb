Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Configuration

''' <summary>
''' This is the base class from which readonly collections
''' of readonly objects should be derived.
''' </summary>
<Serializable()> _
Public MustInherit Class ReadOnlyCollectionBase
  Inherits CSLA.Core.SortableCollectionBase

  Implements ICloneable
  Implements Serialization.ISerializationNotification

  ''' <summary>
  ''' Creates a new ReadOnlyCollectionBase object.
  ''' </summary>
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
    If Not ActivelySorting AndAlso Locked Then
      Throw New NotSupportedException(GetResourceString("NoInsertReadOnlyException"))
    End If
  End Sub

  ''' <summary>
  ''' Prevents removal of items from the collection when the
  ''' collection is locked.
  ''' </summary>
  Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)
    If Not ActivelySorting AndAlso Locked Then
      Throw New NotSupportedException(GetResourceString("NoRemoveReadOnlyException"))
    End If
  End Sub

  ''' <summary>
  ''' Prevents clearing the collection when the
  ''' collection is locked.
  ''' </summary>
  Protected Overrides Sub OnClear()
    If Not ActivelySorting AndAlso Locked Then
      Throw New NotSupportedException(GetResourceString("NoClearReadOnlyException"))
    End If
  End Sub

  ''' <summary>
  ''' Prevents changing an item reference when the 
  ''' collection is locked.
  ''' </summary>
  Protected Overrides Sub OnSet(ByVal index As Integer, ByVal oldValue As Object, ByVal newValue As Object)
    If Not ActivelySorting AndAlso Locked Then
      Throw New NotSupportedException(GetResourceString("NoChangeReadOnlyException"))
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

    Serialization.SerializationNotification.OnSerializing(Me)
    formatter.Serialize(buffer, Me)
    Serialization.SerializationNotification.OnSerialized(Me)
    buffer.Position = 0
    Dim temp As Object = formatter.Deserialize(buffer)
    Serialization.SerializationNotification.OnDeserialized(temp)
    Return temp

  End Function

#End Region

#Region " Data Access "

  Private Sub DataPortal_Create(ByVal Criteria As Object)
    Throw New NotSupportedException(GetResourceString("CreateNotSupportedException"))
  End Sub

  ''' <summary>
  ''' Override this method to allow retrieval of an existing business
  ''' object based on data in the database.
  ''' </summary>
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
  Protected Overridable Sub DataPortal_Fetch(ByVal Criteria As Object)
    Throw New NotSupportedException(GetResourceString("FetchNotSupportedException"))
  End Sub

  Private Sub DataPortal_Update()
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

#Region " ISerializationNotification "

  ''' <summary>
  ''' This method is called on a newly deserialized object
  ''' after deserialization is complete.
  ''' </summary>
  Protected Overridable Sub Deserialized() _
    Implements CSLA.Serialization.ISerializationNotification.Deserialized

    Dim child As Object
    For Each child In list
      If TypeOf child Is Serialization.ISerializationNotification Then
        DirectCast(child, Serialization.ISerializationNotification).Deserialized()
      End If
    Next
  End Sub

  ''' <summary>
  ''' This method is called on the original instance of the
  ''' object after it has been serialized.
  ''' </summary>
  Protected Overridable Sub Serialized() _
    Implements CSLA.Serialization.ISerializationNotification.Serialized

    Dim child As Object
    For Each child In list
      If TypeOf child Is Serialization.ISerializationNotification Then
        DirectCast(child, Serialization.ISerializationNotification).Serialized()
      End If
    Next
  End Sub

  ''' <summary>
  ''' This method is called before an object is serialized.
  ''' </summary>
  Protected Overridable Sub Serializing() _
    Implements CSLA.Serialization.ISerializationNotification.Serializing

    Dim child As Object
    For Each child In list
      If TypeOf child Is Serialization.ISerializationNotification Then
        DirectCast(child, Serialization.ISerializationNotification).Serializing()
      End If
    Next
  End Sub

#End Region

End Class
