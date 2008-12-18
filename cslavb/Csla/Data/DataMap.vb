Imports System
Imports System.Collections.Generic
Imports Csla.Reflection


Namespace Data

  ''' <summary>
  ''' Defines a mapping between two sets of
  ''' properties/fields for use by
  ''' DataMapper.
  ''' </summary>
  Public Class DataMap

#Region "MapElement"

    Friend Class MemberMapping
      Private _from As DynamicMemberHandle
      Public Property FromMember() As DynamicMemberHandle
        Get
          Return _from
        End Get
        Private Set(ByVal value As DynamicMemberHandle)
          _from = value
        End Set
      End Property

      Private _to As DynamicMemberHandle
      Public Property ToMember() As DynamicMemberHandle
        Get
          Return _to
        End Get
        Private Set(ByVal value As DynamicMemberHandle)
          _to = value
        End Set
      End Property

      Public Sub New(ByVal fromMember As DynamicMemberHandle, ByVal toMember As DynamicMemberHandle)
        Me.FromMember = fromMember
        Me.ToMember = toMember
      End Sub
    End Class

#End Region

    Private _sourceType As Type
    Private _targetType As Type
    Private _map As List(Of MemberMapping) = New List(Of MemberMapping)()

    ''' <summary>
    ''' Initializes an instance of the type.
    ''' </summary>
    ''' <param name="sourceType">
    ''' Type of source object.
    ''' </param>
    ''' <param name="targetType">
    ''' Type of target object.
    ''' </param>
    Public Sub New(ByVal sourceType As Type, ByVal targetType As Type)
      _sourceType = sourceType
      _targetType = targetType
    End Sub

    Friend Function GetMap() As List(Of MemberMapping)
      Return _map
    End Function

    ''' <summary>
    ''' Adds a property to property
    ''' mapping.
    ''' </summary>
    ''' <param name="sourceProperty">
    ''' Name of source property.
    ''' </param>
    ''' <param name="targetProperty">
    ''' Name of target property.
    ''' </param>
    Public Sub AddPropertyMapping(ByVal sourceProperty As String, ByVal targetProperty As String)
      _map.Add(New MemberMapping( _
               MethodCaller.GetCachedProperty(_sourceType, sourceProperty), _
               MethodCaller.GetCachedProperty(_targetType, targetProperty)))
    End Sub

    ''' <summary>
    ''' Adds a field to field mapping.
    ''' </summary>
    ''' <param name="sourceField">
    ''' Name of source field.
    ''' </param>
    ''' <param name="targetField">
    ''' Name of target field.
    ''' </param>
    Public Sub AddFieldMapping(ByVal sourceField As String, ByVal targetField As String)
      _map.Add(New MemberMapping( _
               MethodCaller.GetCachedField(_sourceType, sourceField), _
               MethodCaller.GetCachedField(_targetType, targetField)))
    End Sub

    ''' <summary>
    ''' Adds a field to property mapping.
    ''' </summary>
    ''' <param name="sourceField">
    ''' Name of source field.
    ''' </param>
    ''' <param name="targetProperty">
    ''' Name of target property.
    ''' </param>
    Public Sub AddFieldToPropertyMapping(ByVal sourceField As String, ByVal targetProperty As String)
      _map.Add(New MemberMapping( _
               MethodCaller.GetCachedField(_sourceType, sourceField), _
               MethodCaller.GetCachedProperty(_targetType, targetProperty)))
    End Sub

    ''' <summary>
    ''' Adds a property to field mapping.
    ''' </summary>
    ''' <param name="sourceProperty">
    ''' Name of source property.
    ''' </param>
    ''' <param name="targetField">
    ''' Name of target field.
    ''' </param>
    Public Sub AddPropertyToFieldMapping(ByVal sourceProperty As String, ByVal targetField As String)
      _map.Add(New MemberMapping( _
               MethodCaller.GetCachedProperty(_sourceType, sourceProperty), _
               MethodCaller.GetCachedField(_targetType, targetField)))
    End Sub

  End Class

End Namespace