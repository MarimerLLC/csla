Imports System.Security.Principal
Imports System.Threading

''' <summary>
''' 
''' </summary>
Namespace Security

  ''' <summary>
  ''' Implements a custom Principal class that is used by
  ''' CSLA .NET for table-based security.
  ''' </summary>
  <Serializable()> _
  Public Class BusinessPrincipal
    Implements IPrincipal

    Private mIdentity As BusinessIdentity

#Region " IPrincipal "

    ''' <summary>
    ''' Implements the Identity property defined by IPrincipal.
    ''' </summary>
    Public ReadOnly Property Identity() As IIdentity _
        Implements IPrincipal.Identity
      Get
        Return mIdentity
      End Get
    End Property

    ''' <summary>
    ''' Implements the IsInRole property defined by IPrincipal.
    ''' </summary>
    Public Function IsInRole(ByVal Role As String) As Boolean _
        Implements IPrincipal.IsInRole

      Return mIdentity.IsInRole(Role)

    End Function

#End Region

#Region " Login process "

    ''' <summary>
    ''' Initiates a login process using custom CSLA .NET security.
    ''' </summary>
    ''' <remarks>
    ''' As described in the book, this invokes a login process using
    ''' a table-based authentication scheme and a list of roles in
    ''' the database tables. By replacing the code in
    ''' <see cref="T:CSLA.Security.BusinessIdentity" /> you can easily
    ''' adapt this scheme to authenticate the user against any database
    ''' or other scheme.
    ''' </remarks>
    ''' <param name="Username">The user's username.</param>
    ''' <param name="Password">The user's password.</param>
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
