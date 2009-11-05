Imports System.Data.SqlClient

<Serializable()> _
Public Class ClientProjectEdit
  Inherits BusinessBase(Of ClientProjectEdit)

#Region " Business Methods "

  Private _id As Integer
  Public ReadOnly Property Id() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _id
    End Get
  End Property

  Private _clientId As Integer
  Public Property ClientId() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _clientId
    End Get
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Set(ByVal value As Integer)
      CanWriteProperty(True)
      If Not _clientId.Equals(value) Then
        _clientId = value
        PropertyHasChanged()
      End If
    End Set
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

  Private _description As String = ""
  Public Property Description() As String
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _description
    End Get
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Set(ByVal value As String)
      CanWriteProperty(True)
      If Not _description.Equals(value) Then
        _description = value
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

  Public Shared Function NewProject(ByVal projectId As Integer) As ClientProjectEdit

    Return New ClientProjectEdit(projectId)

  End Function

  Public Shared Sub DeleteProject(ByVal projectId As Integer)

    DataPortal.Delete(New DeleteCriteria(projectId))

  End Sub

  Private Sub New()

  End Sub

  Private Sub New(ByVal projectId As Integer)

    _id = projectId

  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class DeleteCriteria

    Private _projectId As Integer
    Public ReadOnly Property ProjectId() As Integer
      Get
        Return _projectId
      End Get
    End Property

    Public Sub New(ByVal projectId As Integer)
      _projectId = projectId
    End Sub

  End Class

  Protected Overrides Sub DataPortal_Insert()

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "INSERT INTO Project (clientId,name,description) VALUES (@clientId,@name,@description);SELECT @newId=id FROM Project WHERE id=SCOPE_IDENTITY()"
        Dim idParam As New SqlParameter("@newId", System.Data.SqlDbType.Int)
        idParam.Direction = System.Data.ParameterDirection.Output
        cm.Parameters.Add(idParam)
        cm.Parameters.AddWithValue("@clientId", _clientId)
        cm.Parameters.AddWithValue("@name", _name)
        cm.Parameters.AddWithValue("@description", _description)

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
        cm.CommandText = "UPDATE Project SET clientid=@clientId,name=@name,description=@description WHERE id=@id"
        cm.Parameters.AddWithValue("@id", _id)
        cm.Parameters.AddWithValue("@clientId", _clientId)
        cm.Parameters.AddWithValue("@name", _name)
        cm.Parameters.AddWithValue("@description", _description)
        cm.ExecuteNonQuery()
      End Using
      MarkOld()
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

  Protected Overrides Sub DataPortal_DeleteSelf()

    DataPortal_Delete(New DeleteCriteria(_id))
    MarkNew()

  End Sub

  Private Overloads Sub DataPortal_Delete(ByVal criteria As DeleteCriteria)

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "DELETE Project WHERE id=@id"
        cm.Parameters.AddWithValue("@id", criteria.ProjectId)
        cm.ExecuteNonQuery()
      End Using
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

#End Region

End Class
