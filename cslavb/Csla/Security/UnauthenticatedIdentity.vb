Imports System
Imports System.Security.Principal
Imports Csla.Serialization
Imports System.Collections.Generic
Imports Csla.Core.FieldManager
Imports System.Runtime.Serialization
Imports Csla.Core

Namespace Security

    ''' <summary>
    ''' Implementation of a .NET identity object representing
    ''' an unauthenticated user. Used by the
    ''' UnauthenticatedPrincipal class.
    ''' </summary>
    <Serializable()> _
    Public NotInheritable Class UnauthenticatedIdentity
        Inherits CslaIdentity

        ''' <summary>
        ''' Creates an instance of the object.
        ''' </summary>
        Public Sub New()
            IsAuthenticated = False
            Name = String.Empty
            AuthenticationType = String.Empty
            Roles = New MobileList(Of String)()
        End Sub

    End Class
End Namespace

