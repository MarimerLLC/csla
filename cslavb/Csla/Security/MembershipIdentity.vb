Imports System
Imports System.Security.Principal
Imports Csla.Serialization
Imports System.Collections.Generic
Imports Csla.Core.FieldManager
Imports System.Runtime.Serialization
Imports Csla.DataPortalClient
Imports Csla.Silverlight
Imports Csla.Core

Namespace Security

  ''' <summary>
  ''' Implements a .NET identity object that automatically
  ''' authenticates against the ASP.NET membership provider.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  <MobileFactory("Csla.Security.IdentityFactory,Csla", "FetchMembershipIdentity")> _
  Public Class MembershipIdentity
    Inherits ReadOnlyBase(Of MembershipIdentity)
    Implements IIdentity, ICheckRoles

#Region "Constructor, Helper Setter"

    Private Shared _forceInit As Integer = 0

    ''' <summary>
    ''' Creates an instance of the class.
    ''' </summary>
    Protected Sub New()
      _forceInit = _forceInit + 0
    End Sub

#End Region

#Region "IsInRole"

    Private Shared ReadOnly RolesProperty As PropertyInfo(Of MobileList(Of String)) = RegisterProperty(New PropertyInfo(Of MobileList(Of String))("Roles"))

    ''' <summary>
    ''' Gets or sets a list of roles for this user.
    ''' </summary>
    Protected Friend Property Roles() As MobileList(Of String)
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

    Private Shared ReadOnly AuthenticationTypeProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("AuthenticationType"))

    ''' <summary>
    ''' Gets the authentication type for this identity.
    ''' </summary>
    Public Property AuthenticationType() As String
      Get
        'string authenticationType = GetProperty<string>(AuthenticationTypeProperty);
        Dim strAuthenticationType As String = GetProperty(Of String)(AuthenticationTypeProperty)

        If strAuthenticationType IsNot Nothing Then
          strAuthenticationType = "Csla"
          LoadProperty(Of String)(AuthenticationTypeProperty, strAuthenticationType)
        End If

        Return strAuthenticationType
      End Get
      Protected Friend Set(ByVal value As String)
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
      Protected Friend Set(ByVal value As Boolean)
        LoadProperty(Of Boolean)(IsAuthenticatedProperty, value)
      End Set
    End Property

    Private Shared ReadOnly NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("Name"))

    Public Property Name() As String
      Get
        Return GetProperty(Of String)(NameProperty)
      End Get
      Protected Friend Set(ByVal value As String)
        LoadProperty(Of String)(NameProperty, value)
      End Set
    End Property

#End Region

#Region "Custom Data"

    ''' <summary>
    ''' Override this method in a subclass to load custom
    ''' data beyond the automatically loaded values from
    ''' the membership and role providers.
    ''' </summary>
    Protected Friend Overridable Sub LoadCustomData()

    End Sub

#End Region

#Region "Criteria"

    ''' <summary>
    ''' Criteria object containing the user credentials
    ''' to be authenticated.
    ''' </summary>
    <Serializable()> _
    Public Class Criteria
      Inherits Core.MobileObject

      Private _name As String
      ''' <summary>
      ''' Gets or sets the username.
      ''' </summary>
      Public Property Name() As String
        Get
          Return _name
        End Get
        Set(ByVal value As String)
          _name = value
        End Set
      End Property

      Private _password As String
      ''' <summary>
      ''' Gets or sets the password.
      ''' </summary>
      Public Property Password() As String
        Get
          Return _password
        End Get
        Set(ByVal value As String)
          _password = value
        End Set
      End Property

      Private _membershipIdentityType As String
      ''' <summary>
      ''' Gets or sets the membership identity type.
      ''' </summary>
      Public Property MembershipIdentityType() As String
        Get
          Return _membershipIdentityType
        End Get
        Set(ByVal value As String)
          _membershipIdentityType = value
        End Set
      End Property

      Private _isRunOnWebServer As Boolean
      ''' <summary>
      ''' Gets or sets whether the membership provider
      ''' should be access on the client (true) or application
      ''' server (false).
      ''' </summary>
      Public Property IsRunOnWebServer() As Boolean
        Get
          Return _isRunOnWebServer
        End Get
        Set(ByVal value As Boolean)
          _isRunOnWebServer = value
        End Set
      End Property

      Public Sub New(ByVal name As String, ByVal password As String, ByVal membershipIdentityType As Type, ByVal isRunOnWebServer As Boolean)
        Me.Name = name
        Me.Password = password
        Me.MembershipIdentityType = membershipIdentityType.AssemblyQualifiedName
        Me.IsRunOnWebServer = isRunOnWebServer
      End Sub

      ''' <summary>
      ''' Override this method to get custom field values
      ''' from the serialization stream.
      ''' </summary>
      ''' <param name="info">Serialization info.</param>
      ''' <param name="mode">Serialization mode.</param>
      Protected Overrides Sub OnGetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As Core.StateMode)
        info.AddValue("MembershipIdentity.Criteria.Name", Name)
        info.AddValue("MembershipIdentity.Criteria.Password", Password)
        info.AddValue("MembershipIdentity.Criteria.MembershipIdentityType", MembershipIdentityType)
        info.AddValue("MembershipIdentity.Criteria.IsRunOnWebServer", IsRunOnWebServer)
        MyBase.OnGetState(info, mode)
      End Sub

      ''' <summary>
      ''' Override this method to set custom field values
      ''' into the serialization stream.
      ''' </summary>
      ''' <param name="info">Serialization info.</param>
      ''' <param name="mode">Serialization mode.</param>
      Protected Overrides Sub OnSetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As Core.StateMode)
        MyBase.OnSetState(info, mode)
        Name = info.GetValue(Of String)("MembershipIdentity.Criteria.Name")
        Password = info.GetValue(Of String)("MembershipIdentity.Criteria.Password")
        MembershipIdentityType = info.GetValue(Of String)("MembershipIdentity.Criteria.MembershipIdentityType")
        IsRunOnWebServer = info.GetValue(Of Boolean)("MembershipIdentity.Criteria.IsRunOnWebServer")
      End Sub

    End Class

#End Region

  End Class
End Namespace

