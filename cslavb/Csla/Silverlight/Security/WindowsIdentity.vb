Imports System
Imports Csla.Serialization
Imports System.Collections.Generic
Imports Csla.Silverlight
Imports Csla.Core

Namespace Silverlight.Security

  '[MobileFactory("Csla.Security.IdentityFactory,Csla", "FetchWindowsIdentity")]
  ''' <summary>
  ''' Base class to simplify the retrieval of Windows identity
  ''' information from a Windows server to a 
  ''' CSLA .NET for Silverlight client.
  ''' </summary>
  <Serializable()> _
  Public MustInherit Class WindowsIdentity
    Inherits ReadOnlyBase(Of WindowsIdentity)

    Implements System.Security.Principal.IIdentity, Csla.Security.ICheckRoles

#Region "Constructor, Helper Setter"
#If SILVERLIGHT Then
    ''' <summary>
    ''' Creates an instance of the class.
    ''' </summary>
    Public Sub new()
#Else

    Private Shared _forceInit As Integer

    ''' <summary>
    ''' Creates an instance of the class.
    ''' </summary>
    Public Sub New()
      _forceInit = _forceInit + 0
    End Sub

    ''' <summary>
    ''' Method invoked when the object is deserialized.
    ''' </summary>
    ''' <param name="context">Serialization context.</param>
    Protected Overrides Sub OnDeserialized(ByVal context As System.Runtime.Serialization.StreamingContext)
      _forceInit = 0
      MyBase.OnDeserialized(context)
    End Sub

#End If

    Private Sub SetWindowsIdentity(ByVal roles As MobileList(Of String), ByVal isAuthenticated As Boolean, ByVal name As String)
      Me.LoadProperty(RolesProperty, roles)
      Me.LoadProperty(IsAuthenticatedProperty, isAuthenticated)
      Me.LoadProperty(NameProperty, name)
    End Sub

#End Region

#Region "Identity and roles population"
#If Not Silverlight Then

    ''' <summary>
    ''' Retrieves identity and role information from the currently
    ''' logged in Windows user.
    ''' </summary>
    Protected Sub PopulateWindowsIdentity()
      Dim DomainDelimiter As String = "\\"
      Dim groups As System.Security.Principal.IdentityReferenceCollection = System.Security.Principal.WindowsIdentity.GetCurrent().Groups
      Dim roles As MobileList(Of String) = New MobileList(Of String)

      For Each item As System.Security.Principal.IdentityReference In groups
        Dim account As System.Security.Principal.NTAccount = CType(item.Translate(GetType(System.Security.Principal.NTAccount)), System.Security.Principal.NTAccount)
        If account.Value.Contains(DomainDelimiter) Then
          roles.Add(account.Value.Substring(account.Value.LastIndexOf(DomainDelimiter) + 1))
        Else
          roles.Add(account.Value)
        End If
      Next

      Dim identityName As String = System.Security.Principal.WindowsIdentity.GetCurrent().Name
      If identityName.Contains(DomainDelimiter) Then
        identityName = identityName.Substring(identityName.LastIndexOf(DomainDelimiter) + 1)
      End If

      Me.LoadProperty(RolesProperty, roles)
      Me.LoadProperty(IsAuthenticatedProperty, True)
      Me.LoadProperty(NameProperty, identityName)
    End Sub
#End If
#End Region

#Region "IsInRole"
    Private Shared ReadOnly RolesProperty As PropertyInfo(Of MobileList(Of String)) = RegisterProperty(Of MobileList(Of String))(New PropertyInfo(Of MobileList(Of String))("Roles"))

    ''' <summary>
    ''' Returns a value indicating whether the current user
    ''' is in the specified role.
    ''' </summary>
    ''' <param name="role">Role to check.</param>
    ''' <returns></returns>
    Public Function IsInRole(ByVal role As String) As Boolean Implements Csla.Security.ICheckRoles.IsInRole
      Return GetProperty(Of MobileList(Of String))(RolesProperty).Contains(role)
    End Function

#End Region

#Region "IIdentity"

    ''' <summary>
    ''' Returns the authentication type for this identity.
    ''' Always returns Windows.
    ''' </summary>
    Public ReadOnly Property AuthenticationType() As String Implements System.Security.Principal.IIdentity.AuthenticationType
      Get
        Return "Windows"
      End Get
    End Property

    Private Shared ReadOnly IsAuthenticatedProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(New PropertyInfo(Of Boolean)("IsAuthenticated"))

    ''' <summary>
    ''' Returns a value indicating whether this identity
    ''' represents an authenticated user.
    ''' </summary>
    Public ReadOnly Property IsAuthenticated() As Boolean Implements System.Security.Principal.IIdentity.IsAuthenticated
      Get
        Return GetProperty(Of Boolean)(IsAuthenticatedProperty)
      End Get
    End Property

    Private Shared ReadOnly NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("Name"))

    Public ReadOnly Property Name() As String Implements System.Security.Principal.IIdentity.Name
      Get
        Return GetProperty(Of String)(NameProperty)
      End Get
    End Property

#End Region

  End Class
End Namespace

