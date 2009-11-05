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

  Public Sub MarkForUpdate()

    MarkOld()
    MarkDirty()

  End Sub

#End Region

#Region " Business Rules "

  Protected Overrides Sub AddBusinessRules()

    ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, "Name")

  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function NewClient(ByVal clientId As Integer) As ClientEdit

    Return New ClientEdit(clientId)

  End Function

  Public Shared Sub DeleteClient(ByVal clientId As Integer)

    DataPortal.Delete(New Criteria(clientId))

  End Sub

  Private Sub New()

  End Sub

  Private Sub New(ByVal clientId As Integer)

    _id = clientId

  End Sub

#End Region

#Region " Data Access "

  Protected Overrides Sub DataPortal_Insert()

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
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
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

  Protected Overrides Sub DataPortal_Update()

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "UPDATE Client SET name=@name WHERE id=@id"
        cm.Parameters.AddWithValue("@id", _id)
        cm.Parameters.AddWithValue("@name", _name)
        cm.ExecuteNonQuery()
      End Using
      MarkOld()
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

  Protected Overrides Sub DataPortal_DeleteSelf()

    DataPortal_Delete(New Criteria(_id))
    MarkNew()

  End Sub


  <Serializable()> _
  Private Class Criteria

    Private _clientId As Integer
    Public ReadOnly Property ClientId() As Integer
      Get
        Return _clientId
      End Get
    End Property

    Public Sub New(ByVal clientId As Integer)
      _clientId = ClientId
    End Sub

  End Class

  Private Overloads Sub DataPortal_Delete(ByVal criteria As criteria)

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      RemoveProjects(criteria.ClientId)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "DELETE Client WHERE id=@id"
        cm.Parameters.AddWithValue("@id", criteria.ClientId)
        cm.ExecuteNonQuery()
      End Using
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

  Private Sub RemoveProjects(ByVal clientId As Integer)

    Dim cn As SqlConnection = CType(ApplicationContext.LocalContext("cn"), SqlConnection)
    Using cm As SqlCommand = cn.CreateCommand
      cm.CommandType = System.Data.CommandType.Text
      cm.CommandText = "DELETE Project WHERE clientId=@id"
      cm.Parameters.AddWithValue("@id", clientId)
      cm.ExecuteNonQuery()
    End Using

  End Sub

#End Region

End Class
