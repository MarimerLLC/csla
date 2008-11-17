#If CLIENTONLY <> True Then
Imports System
Imports System.Web.Security
Imports Csla.Core

Namespace Security

    ''' <summary>
    ''' IdentityFactory is an object in charge of retrieving <see cref="MembershipIdentity.Criteria"/> 
    ''' on the web server and transferring it back to the client.
    ''' </summary>
    <Serializable()> _
    Public NotInheritable Class IdentityFactory
        Inherits MembershipIdentity

        ''' <summary>
        ''' Fetches MembershipIdentity from the server
        ''' </summary>
        ''' <param name="criteria"><see cref="MembershipIdentity.Criteria"/></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FetchMembershipIdentity(ByVal criteria As MembershipIdentity.Criteria) As MembershipIdentity
            If criteria.IsRunOnWebServer Then
                Return GetIdentityOnServer(criteria)
            Else
                Dim serverIdentity As IdentityFactory = DataPortal.Fetch(Of IdentityFactory)(criteria)
                Dim identityType As Type = Type.GetType(criteria.MembershipIdentityType)
                Return GetIdentity(identityType, serverIdentity)
            End If
        End Function

        Friend Function GetIdentityOnServer(ByVal criteria As MembershipIdentity.Criteria) As MembershipIdentity

            Dim identityType As Type = Type.GetType(criteria.MembershipIdentityType)

            Dim returnValue = CType(Activator.CreateInstance(identityType, True), MembershipIdentity)

            If Membership.ValidateUser(criteria.Name, criteria.Password) Then
                returnValue.IsAuthenticated = True
                returnValue.Name = criteria.Name
                returnValue.Roles = New MobileList(Of String)(System.Web.Security.Roles.GetRolesForUser(criteria.Name))
                returnValue.AuthenticationType = "Csla"
                returnValue.LoadCustomData()
            Else
                returnValue.IsAuthenticated = False
                returnValue.Name = String.Empty
                returnValue.Roles = New MobileList(Of String)()
                returnValue.AuthenticationType = ""
            End If

            Return returnValue

        End Function

        Private Function GetIdentity(ByVal identityType As Type, ByVal identity As IdentityFactory) As MembershipIdentity
            Dim returnValue As MembershipIdentity = CType(Activator.CreateInstance(identityType), MembershipIdentity)
            returnValue.IsAuthenticated = identity.IsAuthenticated
            returnValue.Name = identity.Name
            returnValue.Roles = identity.Roles
            returnValue.AuthenticationType = "Csla"
            Return returnValue
        End Function

        Private Overloads Sub DataPortal_Fetch(ByVal criteria As MembershipIdentity.Criteria)
            Dim target As MembershipIdentity = GetIdentityOnServer(criteria)
            IsAuthenticated = target.IsAuthenticated
            Name = target.Name
            Roles = target.Roles
            AuthenticationType = "Csla"
        End Sub

    End Class

End Namespace
#End If

