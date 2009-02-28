Imports System
Imports System.Collections.Generic
Imports Csla.Core

Namespace Validation

  ''' <summary>
  ''' Arguments provided to an async 
  ''' validation rule method.
  ''' </summary>
  Public Class AsyncRuleArgs

    Private _Properties() As IPropertyInfo
    ''' <summary>
    ''' List of the property values to be made available
    ''' to this validation rule.
    ''' </summary>
    Public Property Properties() As Core.IPropertyInfo()
      Get
        Return _Properties
      End Get
      Protected Set(ByVal value As Core.IPropertyInfo())
        _Properties = value
      End Set
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="primaryProperty">
    ''' The primary property to be validated by this
    ''' rule method.
    ''' </param>
    ''' <param name="additionalProperties">
    ''' A list of additional property values to be
    ''' provided to the validationr rule.
    ''' </param>
    ''' <remarks></remarks>
    Public Sub New(ByVal primaryProperty As IPropertyInfo, ByVal ParamArray additionalProperties() As IPropertyInfo)

      If primaryProperty Is Nothing Then
        Throw New ArgumentNullException("primaryProperty")
      End If

      Dim length As Integer = 0
      If additionalProperties IsNot Nothing Then
        length += additionalProperties.Length
      End If

      _Properties = New IPropertyInfo(length) {}
      Properties(0) = primaryProperty

      If additionalProperties IsNot Nothing Then
        Array.Copy(additionalProperties, 0, Properties, 1, additionalProperties.Length)
      End If

    End Sub

  End Class

End Namespace
