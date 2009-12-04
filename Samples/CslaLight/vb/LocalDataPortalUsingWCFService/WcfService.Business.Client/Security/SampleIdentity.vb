Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla
Imports Csla.Security
Imports Csla.Core
Imports Csla.Serialization
Imports Csla.DataPortalClient
Imports System.ComponentModel

#If Not SILVERLIGHT = 1 Then
Imports System.Data.SqlClient
#End If


<Serializable()> _
Public Class SampleIdentity
  Inherits CslaIdentity

#If Silverlight = 1 Then

  Public Shared Sub GetIdentity(ByVal username As String, ByVal password As String, ByVal completed As EventHandler(Of DataPortalResult(Of SampleIdentity)))
    GetCslaIdentity(Of SampleIdentity)(completed, New CredentialsCriteria(username, password))
  End Sub

  Private Sub EndQuery(ByVal sender As Object, ByVal e As CompanyServiceReference.GetUserCompletedEventArgs)
    Try
      If e.Error Is Nothing Then
        Dim user As CompanyServiceReference.UserInfo = e.Result

        If user.IsAuthenticated Then
          Me.Roles = New MobileList(Of String)(New String() {user.Role})
          Me.AuthenticationType = "WCF"
          Me.Name = user.UserName
          Me.IsAuthenticated = True
        End If

        If IsAuthenticated Then
          _handler(Me, Nothing)
        Else
          _handler(Nothing, New ArgumentException("Invalid user/passaword"))
        End If
      Else
        _handler(Nothing, e.Error)
      End If

    Catch ex As Exception
      _handler(Nothing, ex)
    Finally
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      RemoveHandler client.GetUserCompleted, AddressOf EndQuery
    End Try

  End Sub

  Private _handler As LocalProxy(Of SampleIdentity).CompletedHandler

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Sub DataPortal_Fetch(ByVal criteria As CredentialsCriteria, ByVal handler As LocalProxy(Of SampleIdentity).CompletedHandler)
    Try
      _handler = handler
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      AddHandler client.GetUserCompleted, AddressOf EndQuery
      client.GetUserAsync(criteria.UserName, criteria.Password)


    Catch ex As Exception
      _handler = Nothing
      handler(Nothing, ex)
    End Try
  End Sub

#End If

End Class
