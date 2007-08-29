Imports System.Security.Principal

Namespace Security

  <Serializable()> _
  Public Class PTPrincipal
    Inherits Csla.Security.BusinessPrincipalBase

    Private Sub New(ByVal identity As IIdentity)
      MyBase.New(identity)
    End Sub

    Public Shared Function VerifyCredentials(ByVal username As String, ByVal password As String) As Boolean

      Return UsernamePasswordValidator.Validate(username, password)

    End Function

    Public Shared Function Login(ByVal username As String, ByVal password As String) As Boolean

      Return SetPrincipal(PTIdentity.GetIdentity(username, password))

    End Function

    Public Shared Sub LoadPrincipal(ByVal username As String)

      SetPrincipal(PTIdentity.GetIdentity(username))

    End Sub

    Private Shared Function SetPrincipal(ByVal identity As PTIdentity) As Boolean

      If identity.IsAuthenticated Then
        Dim principal As PTPrincipal = New PTPrincipal(identity)
        Csla.ApplicationContext.User = principal
      End If
      Return identity.IsAuthenticated

    End Function

    Public Shared Sub Logout()

      Dim identity As PTIdentity = PTIdentity.UnauthenticatedIdentity()
      Dim principal As PTPrincipal = New PTPrincipal(identity)
      Csla.ApplicationContext.User = principal

    End Sub

    Public Overrides Function IsInRole(ByVal role As String) As Boolean

      Dim identity As PTIdentity = CType(Me.Identity, PTIdentity)
      Return identity.IsInRole(role)

    End Function

#Region "UsernamePasswordValidator"

    <Serializable()> _
    Private Class UsernamePasswordValidator
      Inherits ReadOnlyBase(Of UsernamePasswordValidator)

#Region "Business Methods"

      Private _validUser As Boolean
      Public ReadOnly Property ValidUser() As Boolean
        Get
          Return _validUser
        End Get
      End Property

      Private _id As Guid = Guid.NewGuid()
      Protected Overrides Function GetIdValue() As Object
        Return _id
      End Function

#End Region

#Region "Factory Methods"

      Public Shared Function Validate(ByVal username As String, ByVal password As String) As Boolean

        Dim obj As UsernamePasswordValidator = DataPortal.Fetch(Of UsernamePasswordValidator)(New Criteria(username, password))
        Return obj.ValidUser

      End Function

      Private Sub New()
        ' require use of factory methods 
      End Sub

#End Region

#Region "Data Access "

      <Serializable()> _
      Private Class Criteria
        Private _username As String
        Public ReadOnly Property Username() As String
          Get
            Return _username
          End Get
        End Property

        Private _password As String
        Public ReadOnly Property Password() As String
          Get
            Return _password
          End Get
        End Property

        Public Sub New(ByVal username As String, ByVal password As String)
          _username = username
          _password = password
        End Sub
      End Class

      Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

        _validUser = False
        Using cn As SqlConnection = New SqlConnection(Database.SecurityConnection)
          cn.Open()
          Using cm As SqlCommand = cn.CreateCommand()
            cm.CommandText = "VerifyCredentials"
            cm.CommandType = CommandType.StoredProcedure
            cm.Parameters.AddWithValue("@user", criteria.Username)
            cm.Parameters.AddWithValue("@pw", criteria.Password)
            Using dr As SqlDataReader = cm.ExecuteReader()
              If dr.Read() Then
                _validUser = True
              End If
            End Using
          End Using
        End Using

      End Sub

#End Region

    End Class

#End Region

  End Class

End Namespace
