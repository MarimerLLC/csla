Imports Microsoft.VisualBasic
Imports ProjectTracker.Library.Security

Public Class Security

  Public Shared Sub UseAnonymous()

    ProjectTracker.Library.Security.PTPrincipal.Logout()

  End Sub

End Class
