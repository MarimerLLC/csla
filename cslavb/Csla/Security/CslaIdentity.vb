Option Infer On
Option Strict Off
Imports System
Imports System.Linq.Expressions
Imports System.Security.Principal
Imports Csla.Serialization
Imports System.Collections.Generic
Imports Csla.Core.FieldManager
Imports System.Runtime.Serialization
Imports System.Reflection
Imports Csla.Reflection
Imports Csla.Core

Namespace Security
  <Serializable()> _
  Partial Public MustInherit Class CslaIdentity
    Inherits ReadOnlyBase(Of CslaIdentity)
    Implements ICheckRoles, IIdentity

#Region " RegisterProperty "

    ''' <summary>
    ''' Indicates that the specified property belongs
    ''' to the business object type.
    ''' </summary>
    ''' <typeparam name="T">Type of object to which the property belongs.</typeparam>
    ''' <typeparam name="P">Type of property</typeparam>
    ''' <param name="propertyLambdaExpression">Property Expression</param>
    ''' <returns>The provided IPropertyInfo object.</returns>
    Protected Overloads Shared Function RegisterProperty(Of T, P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object))) As PropertyInfo(Of P)
      Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
      Return RegisterProperty(GetType(T), New PropertyInfo(Of P)(reflectedPropertyInfo.Name))
    End Function

    ''' <summary>
    ''' Indicates that the specified property belongs
    ''' to the business object type.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="P">Type of property</typeparam>
    ''' <param name="propertyLambdaExpression">Property Expression</param>
    ''' <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    ''' <returns>The provided IPropertyInfo object.</returns>
    Protected Overloads Shared Function RegisterProperty(Of T, P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object)), ByVal friendlyName As String) As PropertyInfo(Of P)
      Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
      Return RegisterProperty(GetType(T), New PropertyInfo(Of P)(reflectedPropertyInfo.Name, friendlyName))
    End Function

    ''' <summary>
    ''' Indicates that the specified property belongs
    ''' to the business object type.
    ''' </summary>
    ''' <typeparam name="T">Type of Target</typeparam>
    ''' <typeparam name="P">Type of property</typeparam>
    ''' <param name="propertyLambdaExpression">Property Expression</param>
    ''' <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    ''' <param name="defaultValue">Default Value for the property</param>
    ''' <returns></returns>
    Protected Overloads Shared Function RegisterProperty(Of T, P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object)), ByVal friendlyName As String, ByVal defaultValue As P) As PropertyInfo(Of P)
      Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
      Return RegisterProperty(GetType(T), New PropertyInfo(Of P)(reflectedPropertyInfo.Name, friendlyName, defaultValue))
    End Function

#End Region

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
      Dim roles = GetProperty(Of MobileList(Of String))(RolesProperty)

      If roles IsNot Nothing Then
        Return roles.Contains(role)
      Else
        Return False
      End If

    End Function

#End Region

#Region "IIdentity"

    ''' <remarks>These methods implements IIdentity's methods. 
    ''' VB does not allow us to implement a method and change its signature, 
    ''' i.e. from ReadOnly to ReadOnly with Protected Set Accessor.</remarks>
#Region "IIdentity Helper"

    Private ReadOnly Property IIdentity_AuthenticationType() As String Implements IIdentity.AuthenticationType
      Get
        Return AuthenticationType
      End Get
    End Property

    Private ReadOnly Property IIdentity_IsAuthenticated() As Boolean Implements IIdentity.IsAuthenticated
      Get
        Return IsAuthenticated
      End Get
    End Property

    Private ReadOnly Property IIdentity_Name() As String Implements IIdentity.Name
      Get
        Return Name
      End Get
    End Property

#End Region

    Private Shared ReadOnly AuthenticationTypeProperty As PropertyInfo(Of String) = _
      RegisterProperty(Of String)(New PropertyInfo(Of String)("AuthenticationType", "Authentication type", "Csla"))

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

