Imports System.Data.SqlClient
Imports System.Collections.Specialized
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Configuration

''' <summary>
''' This is a base class from which readonly name/value 
''' business classes can be quickly and easily created.
''' </summary>
''' <remarks>
''' As discussed in Chapter 5, inherit from this class to
''' quickly and easily create name/value list objects for
''' population of ListBox or ComboBox controls and for
''' validation of list-based data items in your business
''' objects.
''' </remarks>
<Serializable()> _
Public MustInherit Class NameValueList
  Inherits NameObjectCollectionBase

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

#Region " Collection methods "

  ''' <summary>
  ''' Returns a value from the list.
  ''' </summary>
  ''' <param name="index">The positional index of the value in the collection.</param>
  ''' <returns>The specified value.</returns>
  Default Public ReadOnly Property Item(ByVal Index As Integer) As String
    Get
      Return CStr(MyBase.BaseGet(Index))
    End Get
  End Property

  ''' <summary>
  ''' Returns a value from the list.
  ''' </summary>
  ''' <param name="Name">The name of the value.</param>
  ''' <returns>The specified value.</returns>
  Default Public ReadOnly Property Item(ByVal Name As String) As String
    Get
      Return CStr(MyBase.BaseGet(Name))
    End Get
  End Property

  ''' <summary>
  ''' Adds a name/value pair to the list.
  ''' </summary>
  ''' <param name="Name">The name of the item.</param>
  ''' <param name="Value">The value to be added.</param>
  Protected Sub Add(ByVal Name As String, ByVal Value As String)
    MyBase.BaseAdd(Name, Value)
  End Sub

  ''' <summary>
  ''' Returns the first name found in the list with the specified
  ''' value.
  ''' </summary>
  ''' <remarks>
  ''' This method throws an exception if no matching value is found
  ''' in the list.
  ''' </remarks>
  ''' <param name="Item">The value to search for in the list.</param>
  ''' <returns>The name of the item found.</returns>
  Public ReadOnly Property Key(ByVal Item As String) As String
    Get
      Dim keyName As String
      For Each keyName In Me
        If Me.Item(keyName) = Item Then
          Return keyName
        End If
      Next
      ' we didn't find a match - throw an exception
      Throw New ApplicationException(GetResourceString("NoMatchException"))
    End Get
  End Property

#End Region

#Region " Create and Load "

  ''' <summary>
  ''' Creates a new NameValueList.
  ''' </summary>
  Protected Sub New()
    ' prevent public creation
  End Sub

  ''' <summary>
  ''' Creates a new NameValueList.
  ''' </summary>
  Protected Sub New(ByVal info As SerializationInfo, _
      ByVal context As StreamingContext)
    MyBase.New(info, context)
  End Sub

#End Region

#Region " Data Access "

  Private Sub DataPortal_Create(ByVal Criteria As Object)
    Throw New NotSupportedException(GetResourceString("CreateNotSupportedException"))
  End Sub

  ''' <summary>
  ''' Override this method to allow retrieval of an existing business
  ''' object based on data in the database.
  ''' </summary>
  ''' <remarks>
  ''' In many cases you can call the SimpleFetch method to
  ''' retrieve simple name/value data from a single table in
  ''' a database. In more complex cases you may need to implement
  ''' your own data retrieval code using ADO.NET.
  ''' </remarks>
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

  ''' <summary>
  ''' Provides default/simple loading for most lists. 
  ''' It is called to load data from the database
  ''' </summary>
  ''' <param name="DataBaseName">
  ''' The name of the database to read. This database name
  ''' must correspond to a database entry in the application
  ''' configuration file.
  ''' </param>
  ''' <param name="TableName">The name of the table to read.</param>
  ''' <param name="NameColumn">The name of the column containing name or key values.</param>
  ''' <param name="ValueColumn">The name of the column containing data values.</param>
  Protected Sub SimpleFetch(ByVal DataBaseName As String, ByVal TableName As String, ByVal NameColumn As String, ByVal ValueColumn As String)

    Dim cn As New SqlConnection(DB(DataBaseName))
    Dim cm As New SqlCommand

    cn.Open()
    Try
      With cm
        .Connection = cn
        .CommandText = _
          "SELECT " & NameColumn & "," & ValueColumn & " FROM " & TableName
        Dim dr As New Data.SafeDataReader(.ExecuteReader)
        Try
          While dr.Read()
            If dr.IsDBNull(1) Then
              Add(CStr(dr.GetValue(0)), "")

            Else
              Add(CStr(dr.GetValue(0)), CStr(dr.GetValue(1)))
            End If
          End While

        Finally
          dr.Close()
        End Try

      End With

    Finally
      cn.Close()
    End Try

  End Sub

#End Region

End Class
