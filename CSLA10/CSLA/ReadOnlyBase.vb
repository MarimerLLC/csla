Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Configuration
Imports System.Reflection

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
  Implements Serialization.ISerializationNotification

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

    ' now cascade the call to all child objects/collections
    Dim fields() As FieldInfo
    Dim field As FieldInfo

    ' get the list of fields in this type
    fields = Me.GetType.GetFields( _
                            BindingFlags.NonPublic Or _
                            BindingFlags.Instance Or _
                            BindingFlags.Public)

    For Each field In fields
      If Not field.FieldType.IsValueType AndAlso _
            Not Attribute.IsDefined(field, GetType(NotUndoableAttribute)) Then
        ' it's a ref type, so check for ISerializationNotification
        Dim value As Object = field.GetValue(Me)
        If TypeOf value Is Serialization.ISerializationNotification Then
          DirectCast(value, Serialization.ISerializationNotification).Deserialized()
        End If
      End If
    Next

  End Sub

  ''' <summary>
  ''' This method is called on the original instance of the
  ''' object after it has been serialized.
  ''' </summary>
  Protected Overridable Sub Serialized() _
    Implements CSLA.Serialization.ISerializationNotification.Serialized

    ' cascade the call to all child objects/collections
    Dim fields() As FieldInfo
    Dim field As FieldInfo

    ' get the list of fields in this type
    fields = Me.GetType.GetFields( _
                            BindingFlags.NonPublic Or _
                            BindingFlags.Instance Or _
                            BindingFlags.Public)

    For Each field In fields
      If Not field.FieldType.IsValueType AndAlso _
            Not Attribute.IsDefined(field, GetType(NotUndoableAttribute)) Then
        ' it's a ref type, so check for ISerializationNotification
        Dim value As Object = field.GetValue(Me)
        If TypeOf value Is Serialization.ISerializationNotification Then
          DirectCast(value, Serialization.ISerializationNotification).Serialized()
        End If
      End If
    Next

  End Sub

  ''' <summary>
  ''' This method is called before an object is serialized.
  ''' </summary>
  Protected Overridable Sub Serializing() _
    Implements CSLA.Serialization.ISerializationNotification.Serializing

    ' cascade the call to all child objects/collections
    Dim fields() As FieldInfo
    Dim field As FieldInfo

    ' get the list of fields in this type
    fields = Me.GetType.GetFields( _
                            BindingFlags.NonPublic Or _
                            BindingFlags.Instance Or _
                            BindingFlags.Public)

    For Each field In fields
      If Not field.FieldType.IsValueType AndAlso _
            Not Attribute.IsDefined(field, GetType(NotUndoableAttribute)) Then
        ' it's a ref type, so check for ISerializationNotification
        Dim value As Object = field.GetValue(Me)
        If TypeOf value Is Serialization.ISerializationNotification Then
          DirectCast(value, Serialization.ISerializationNotification).Serializing()
        End If
      End If
    Next

  End Sub

#End Region

End Class
