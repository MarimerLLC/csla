Imports System
Imports System.Security.Principal
Imports Csla.Serialization
Imports Csla.Serialization.Mobile

Namespace Security

  ''' <summary>
  ''' Base class from which custom principal
  ''' objects should inherit to operate
  ''' properly with the data portal.
  ''' </summary>
  <Serializable()> _
  Public Class BusinessPrincipalBase
    Inherits Core.MobileObject
    Implements IPrincipal

    Private _identity As IIdentity

    Protected Sub New()
      _identity = New UnauthenticatedIdentity()      
    End Sub

    ''' <summary>
    ''' Returns the user's identity object.
    ''' </summary>
    Public Overridable ReadOnly Property Identity() As IIdentity _
        Implements IPrincipal.Identity
      Get
        Return _identity
      End Get
    End Property

    ''' <summary>
    ''' Returns a value indicating whether the
    ''' user is in a given role.
    ''' </summary>
    ''' <param name="role">Name of the role.</param>
    Public Overridable Function IsInRole(ByVal role As String) As Boolean _
      Implements IPrincipal.IsInRole

      Dim check = CType(_identity, ICheckRoles)

      If check IsNot Nothing Then
        Return check.IsInRole(role)
      Else
        Return False
      End If

    End Function

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="identity">Identity object for the user.</param>
    Protected Sub New(ByVal identity As IIdentity)

      _identity = identity

    End Sub

    ''' <summary>
    ''' Override this method to get custom field values
    ''' from the serialization stream.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    ''' <param name="mode">Serialization mode.</param>
    Protected Overrides Sub OnGetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As Core.StateMode)
      info.AddValue("BusinessPrincipalBase.Identity", MobileFormatter.Serialize(_identity))
      MyBase.OnGetState(info, mode)
    End Sub

    Protected Overrides Sub OnSetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As Core.StateMode)
      MyBase.OnSetState(info, mode)
      _identity = CType(MobileFormatter.Deserialize(info.GetValue(Of Byte())("BusinessPrincipalBase.Identity")), IIdentity)
    End Sub

  End Class

End Namespace
