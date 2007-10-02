Imports System.Security.Principal
Imports System.IdentityModel.Selectors
Imports System.IdentityModel.Policy
Imports System.IdentityModel.Claims
Imports ProjectTracker.Library.Security

''' <summary>
''' Custom authorization policy for WCF.
''' </summary>
''' <remarks>
''' This policy sets the current principal and identity
''' to the values provided earlier in the process
''' by the CredentialValidator class.
''' </remarks>
Public Class PrincipalPolicy
  Implements IAuthorizationPolicy

  Private id_Renamed As String = Guid.NewGuid().ToString()

  Public ReadOnly Property Id() As String Implements IdentityModel.Policy.IAuthorizationPolicy.Id
    Get
      Return Me.id_Renamed
    End Get
  End Property

  Public ReadOnly Property Issuer() As ClaimSet Implements IdentityModel.Policy.IAuthorizationPolicy.Issuer
    Get
      Return ClaimSet.System
    End Get
  End Property

  Public Function Evaluate(ByVal context As EvaluationContext, ByRef state As Object) As Boolean _
    Implements IdentityModel.Policy.IAuthorizationPolicy.Evaluate

    ' get the identities list from the context
    Dim obj As Object = Nothing
    If (Not context.Properties.TryGetValue("Identities", obj)) Then
      Return False
    End If
    Dim identities As IList(Of IIdentity) = TryCast(obj, IList(Of IIdentity))

    ' make sure there is already a default identity
    If identities Is Nothing OrElse identities.Count <= 0 Then
      Return False
    End If

    ' try to get principal from rolling cache
    Dim username As String = identities(0).Name
    Dim principal As IPrincipal = Csla.Security.PrincipalCache.GetPrincipal(username)

    If principal Is Nothing Then
      PTPrincipal.Logout()
      If username <> "anonymous" Then
        ' load prinicpal based on username authenticated in CredentialValidator
        PTPrincipal.LoadPrincipal(username)
        ' add current principal to rolling cache
        Csla.Security.PrincipalCache.AddPrincipal(Csla.ApplicationContext.User)
      End If
      principal = Csla.ApplicationContext.User
    End If

    ' tell WCF to use the custom principal 
    context.Properties("Principal") = principal

    ' tell WCF to use the custom identity 
    identities(0) = principal.Identity

    Return True

  End Function

End Class
