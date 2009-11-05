Namespace Library

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

    Private Shared _clientList As ClientNVList

    Public Shared ReadOnly Property ClientList() As ClientNVList
      Get
        If _clientList Is Nothing Then
          _clientList = ClientNVList.GetList
        End If
        Return _clientList
      End Get
    End Property

#End Region

#Region " Business Rules "

    Protected Overrides Sub AddBusinessRules()

      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, "Name")
      ValidationRules.AddRule(Of ClientProjectEdit)(AddressOf VerifyClient, "ClientId")

    End Sub

    Private Shared Function VerifyClient(Of T As ClientProjectEdit)(ByVal target As T, ByVal e As Csla.Validation.RuleArgs) As Boolean

      If Not ClientList.ContainsKey(target._clientId) Then
        e.Description = "Invalid client id"
        Return False

      Else
        Return True
      End If

    End Function

#End Region

#Region " Validation Rules "

#End Region

#Region " Factory Methods "

    Friend Shared Function NewProject() As ClientProjectEdit
      Return DataPortal.Create(Of ClientProjectEdit)()
    End Function

    Friend Shared Function GetProject(ByVal dr As Csla.Data.SafeDataReader) As ClientProjectEdit
      Return New ClientProjectEdit(dr)
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
      _clientId = ClientList.DefaultValue
      ValidationRules.CheckRules()

    End Sub

    Private Sub Fetch(ByVal dr As Csla.Data.SafeDataReader)

      _id = dr.GetInt32("id")
      _clientId = dr.GetInt32("clientId")
      _name = dr.GetString("name")
      _description = dr.GetString("description")
      MarkOld()

    End Sub

#End Region

  End Class

End Namespace
