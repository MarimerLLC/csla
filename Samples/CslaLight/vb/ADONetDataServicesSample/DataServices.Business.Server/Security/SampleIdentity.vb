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
Imports System.Data

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

  Private Sub EndQuery(ByVal e As IAsyncResult)
    Try
      Dim queryUsers = CType(e.AsyncState, Services.Client.DataServiceQuery(Of CompanyServiceReference.Users))
      Dim users = queryUsers.EndExecute(e).ToList()

      Dim query = (From oneUser In users _
                    Where oneUser.UserName.ToUpper = _criteria.UserName.ToUpper And oneUser.Password = _criteria.Password _
                    Select oneUser).FirstOrDefault
      If query IsNot Nothing Then
        Me.Roles = New MobileList(Of String)(New String() {query.Role})
        Me.AuthenticationType = "ADO"
        Me.Name = query.UserName
        Me.IsAuthenticated = True
      End If

      If IsAuthenticated Then
        _handler(Me, Nothing)
      Else
        _handler(Nothing, New ArgumentException("Invalid user/passaword"))
      End If
    Catch ex As Exception
      _handler(Nothing, ex)
    End Try

  End Sub

  Private _handler As LocalProxy(Of SampleIdentity).CompletedHandler
  Private _criteria As CredentialsCriteria

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Sub DataPortal_Fetch(ByVal criteria As CredentialsCriteria, ByVal handler As LocalProxy(Of SampleIdentity).CompletedHandler)
    Try
      _handler = handler
      _criteria = criteria
      Dim context = Csla.Data.DataServiceContextManager(Of CompanyServiceReference.CompanyEntities).GetManager(Company.GetServiceUri).DataServiceContext
      context.Users.BeginExecute(AddressOf EndQuery, context.Users)

    Catch ex As Exception
      _handler = Nothing
      handler(Nothing, ex)
    End Try
  End Sub


#Else



#End If

End Class
