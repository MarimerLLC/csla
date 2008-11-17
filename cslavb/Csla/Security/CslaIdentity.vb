Option Infer On
Option Strict Off
Imports System
Imports System.Security.Principal
Imports Csla.Serialization
Imports System.Collections.Generic
Imports Csla.Core.FieldManager
Imports System.Runtime.Serialization
Imports Csla.Core

Namespace Security
    Partial Public MustInherit Class CslaIdentity
        Inherits ReadOnlyBase(Of CslaIdentity)
        Implements IIdentity, ICheckRoles

#Region "UnauthenticatedIdentity"

        ''' <summary>
        ''' Creates an instance of the class.
        ''' </summary>
        Public Shared Function UnauthenticatedIdentity() As CslaIdentity
            Return New Security.UnauthenticatedIdentity()
        End Function

#End Region

#Region "IsInRole"

        Private Shared ReadOnly RolesProperty As PropertyInfo(Of MobileList(Of String)) = RegisterProperty(New PropertyInfo(Of MobileList(Of String))("Roles"))

        ''' <summary>
        ''' Gets or sets the list of roles for this user.
        ''' </summary>
        Protected Property Roles() As MobileList(Of String)
            Get
                Return GetProperty(RolesProperty)
            End Get
            Set(ByVal value As MobileList(Of String))
                LoadProperty(Of MobileList(Of String))(RolesProperty, value)
            End Set
        End Property

        Public Function IsInRole(ByVal role As String) As Boolean Implements ICheckRoles.IsInRole
            Dim roles = GetProperty(MobileList(Of String))(RolesProperty)

            If roles IsNot Nothing Then
                Return roles.contains(role)
            Else
                Return False
            End If

        End Function

#End Region

#Region "IIdentity"

        Private Shared ReadOnly AuthenticationTypeProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("AuthenticationType", "Authentication type", "Csla"))

        ''' <summary>
        ''' Gets the authentication type for this identity.
        ''' </summary>
        Public Property AuthenticationType() As String
            Get
                Return GetProperty(Of String)(AuthenticationTypeProperty)
            End Get
            Protected Set(ByVal value As String)
                LoadProperty(Of String)(AuthenticationTypeProperty, value)
            End Set
        End Property

        Private Shared ReadOnly IsAuthenticatedProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(New PropertyInfo(Of Boolean)("IsAuthenticated"))

        ''' <summary>
        ''' Gets a value indicating whether this identity represents
        ''' an authenticated user.
        ''' </summary>
        Public Property IsAuthenticated() As Boolean
            Get
                Return GetProperty(Of Boolean)(IsAuthenticatedProperty)
            End Get
            Protected Set(ByVal value As Boolean)
                LoadProperty(Of Boolean)(IsAuthenticatedProperty, value)
            End Set
        End Property

        Private Shared ReadOnly NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("Name"))

        Public Property Name() As String
            Get
                Return GetProperty(Of String)(NameProperty)
            End Get
            Protected Set(ByVal value As String)
                LoadProperty(Of String)(NameProperty, value)
            End Set
        End Property

#End Region

    End Class
End Namespace

