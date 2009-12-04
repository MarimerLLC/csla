Imports System.Data.SqlClient

<Serializable()> _
  Public Class ClientEdit
  Inherits BusinessBase(Of ClientEdit)

#Region " Business Methods "

  Private _id As Integer
  Public ReadOnly Property Id() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _id
    End Get
  End Property

  Private _name As String = ""
  Public Property Name() As String
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _name
    End Get
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Set(ByVal value As String)
      CanWriteProperty(True)
      If Not _name.Equals(value) Then
        _name = value
        PropertyHasChanged()
      End If
    End Set
  End Property

  Protected Overrides Function GetIdValue() As Object

    Return _id

  End Function

#End Region

#Region " Business Rules "

  Protected Overrides Sub AddBusinessRules()

    ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, "Name")

  End Sub

#End Region

#Region " Factory Methods "

  Friend Shared Function NewClient() As ClientEdit

    Return DataPortal.Create(Of ClientEdit)()

  End Function

  Friend Shared Function GetClient(ByVal dr As Csla.Data.SafeDataReader) As ClientEdit

    Return New ClientEdit(dr)

  End Function

  Private Sub New()

    MarkAsChild()

  End Sub

  Private Sub New(ByVal dr As Csla.Data.SafeDataReader)
    Me.New()
    Fetch(dr)
  End Sub

#End Region

#Region " Data Access "

  Private Shared _lastId As Integer

  <RunLocal()> _
  Private Overloads Sub DataPortal_Create()

    _id = System.Threading.Interlocked.Decrement(_lastId)
    ValidationRules.CheckRules()

  End Sub

  Private Sub Fetch(ByVal dr As Csla.Data.SafeDataReader)

    _id = dr.GetInt32("id")
    _name = dr.GetString("name")
    MarkOld()

  End Sub

  Friend Sub Insert(ByVal parent As ClientList)

    Dim cn As SqlConnection = CType(ApplicationContext.LocalContext("cn"), SqlConnection)
    Using cm As SqlCommand = cn.CreateCommand
      cm.CommandType = System.Data.CommandType.Text
      cm.CommandText = "INSERT INTO Client (name) VALUES (@name);SELECT @newId=id FROM Client WHERE id=SCOPE_IDENTITY()"
      Dim idParam As New SqlParameter("@newId", System.Data.SqlDbType.Int)
      idParam.Direction = System.Data.ParameterDirection.Output
      cm.Parameters.Add(idParam)
      cm.Parameters.AddWithValue("@name", _name)

      cm.ExecuteNonQuery()

      _id = CInt(cm.Parameters("@newId").Value)
    End Using
    MarkOld()

  End Sub

  Friend Sub Update(ByVal parent As ClientList)

    Dim cn As SqlConnection = CType(ApplicationContext.LocalContext("cn"), SqlConnection)
    Using cm As SqlCommand = cn.CreateCommand
      cm.CommandType = System.Data.CommandType.Text
      cm.CommandText = "UPDATE Client SET name=@name WHERE id=@id"
      cm.Parameters.AddWithValue("@id", _id)
      cm.Parameters.AddWithValue("@name", _name)
      cm.ExecuteNonQuery()
    End Using
    MarkOld()

  End Sub

  Friend Sub DeleteSelf(ByVal parent As ClientList)

    RemoveProjects()
    Dim cn As SqlConnection = CType(ApplicationContext.LocalContext("cn"), SqlConnection)
    Using cm As SqlCommand = cn.CreateCommand
      cm.CommandType = System.Data.CommandType.Text
      cm.CommandText = "DELETE Client WHERE id=@id"
      cm.Parameters.AddWithValue("@id", _id)
      cm.ExecuteNonQuery()
    End Using
    MarkNew()

  End Sub

  Private Sub RemoveProjects()

    Dim cn As SqlConnection = CType(ApplicationContext.LocalContext("cn"), SqlConnection)
    Using cm As SqlCommand = cn.CreateCommand
      cm.CommandType = System.Data.CommandType.Text
      cm.CommandText = "DELETE Project WHERE clientId=@id"
      cm.Parameters.AddWithValue("@id", _id)
      cm.ExecuteNonQuery()
    End Using

  End Sub

#End Region

End Class
