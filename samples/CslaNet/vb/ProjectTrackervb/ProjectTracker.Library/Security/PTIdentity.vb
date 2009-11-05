Imports System.Security.Principal

Namespace Security

  <Serializable()> _
  Public Class PTIdentity
    Inherits ReadOnlyBase(Of PTIdentity)

    Implements IIdentity

#Region " Business Methods "

    Protected Overrides Function GetIdValue() As Object

      Return _name

    End Function

#Region " IsInRole "

    Private _roles As New List(Of String)

    Friend Function IsInRole(ByVal role As String) As Boolean

      Return _roles.Contains(role)

    End Function

#End Region

#Region " IIdentity "

    Private _isAuthenticated As Boolean
    Private _name As String = ""

    Public ReadOnly Property AuthenticationType() As String _
      Implements System.Security.Principal.IIdentity.AuthenticationType
      Get
        Return "Csla"
      End Get
    End Property

    Public ReadOnly Property IsAuthenticated() As Boolean _
      Implements System.Security.Principal.IIdentity.IsAuthenticated
      Get
        Return _isAuthenticated
      End Get
    End Property

    Public ReadOnly Property Name() As String _
      Implements System.Security.Principal.IIdentity.Name
      Get
        Return _name
      End Get
    End Property

#End Region

#End Region

#Region " Factory Methods "

    Friend Shared Function UnauthenticatedIdentity() As PTIdentity

      Return New PTIdentity

    End Function

    Friend Shared Function GetIdentity( _
      ByVal username As String, ByVal password As String) As PTIdentity

      Return DataPortal.Fetch(Of PTIdentity)(New CredentialsCriteria(username, password))

    End Function

    Friend Shared Function GetIdentity( _
      ByVal username As String) As PTIdentity

      Return DataPortal.Fetch(Of PTIdentity)(New LoadOnlyCriteria(username))

    End Function

    Private Sub New()
      ' require use of factory methods
    End Sub

#End Region

#Region " Data Access "

    <Serializable()> _
    Private Class CredentialsCriteria

      Private _username As String
      Private _password As String

      Public ReadOnly Property Username() As String
        Get
          Return _username
        End Get
      End Property

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

    <Serializable()> _
    Private Class LoadOnlyCriteria

      Private _username As String

      Public ReadOnly Property Username() As String
        Get
          Return _username
        End Get
      End Property

      Public Sub New(ByVal username As String)
        _username = username
      End Sub
    End Class

    Private Overloads Sub DataPortal_Fetch(ByVal criteria As CredentialsCriteria)

      Using ctx = ContextManager(Of ProjectTracker.DalLinq.Security.SecurityDataContext).GetManager(ProjectTracker.DalLinq.Database.Security)
        Dim data = From u In ctx.DataContext.Users Where u.Username = criteria.Username AndAlso u.Password = criteria.Password Select u
        If data.Count > 0 Then
          Fetch(data.Single)

        Else
          Fetch(Nothing)
        End If
      End Using

    End Sub

    Private Overloads Sub DataPortal_Fetch(ByVal criteria As LoadOnlyCriteria)

      Using ctx = ContextManager(Of ProjectTracker.DalLinq.Security.SecurityDataContext).GetManager(ProjectTracker.DalLinq.Database.Security)
        Dim data = From u In ctx.DataContext.Users Where u.Username = criteria.Username Select u
        If data.Count > 0 Then
          Fetch(data.Single)

        Else
          Fetch(Nothing)
        End If
      End Using

    End Sub

    Private Sub Fetch(ByVal user As DalLinq.Security.User)

      If user IsNot Nothing Then
        _name = user.Username
        _isAuthenticated = True
        Dim roles = From r In user.Roles
        For Each role In roles
          _roles.Add(role.Role)
        Next

      Else
        _name = ""
        _isAuthenticated = False
        _roles.Clear()
      End If

    End Sub

#End Region

  End Class

End Namespace
