Imports System.Security.Principal
Imports System.Threading

Namespace Security

  <Serializable()> _
  Public Class BusinessPrincipal
    Implements IPrincipal

    Private mIdentity As BusinessIdentity

#Region " IPrincipal "

    Public ReadOnly Property Identity() As IIdentity _
        Implements IPrincipal.Identity
      Get
        Return mIdentity
      End Get
    End Property

    Public Function IsInRole(ByVal Role As String) As Boolean _
        Implements IPrincipal.IsInRole

      Return mIdentity.IsInRole(Role)

    End Function

#End Region

#Region " Login process "

    Public Shared Sub Login(ByVal Username As String, ByVal Password As String)
      Dim p As New BusinessPrincipal(Username, Password)
    End Sub

    Private Sub New(ByVal Username As String, ByVal Password As String)
      Dim currentdomain As AppDomain = Thread.GetDomain

      currentdomain.SetPrincipalPolicy(PrincipalPolicy.UnauthenticatedPrincipal)


      Dim OldPrincipal As IPrincipal = Thread.CurrentPrincipal


      Thread.CurrentPrincipal = Me

      Try
        If Not TypeOf OldPrincipal Is BusinessPrincipal Then
          currentdomain.SetThreadPrincipal(Me)
        End If

      Catch
        ' failed, but we don't care because there's nothing
        ' we can do in this case
      End Try

      ' load the underlying identity object that tells whether
      ' we are really logged in, and if so will contain the 
      ' list of roles we belong to
      mIdentity = BusinessIdentity.LoadIdentity(Username, Password)

    End Sub

#End Region

  End Class

End Namespace
