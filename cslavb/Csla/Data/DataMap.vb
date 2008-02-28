Imports System.Reflection

Namespace Data

  ''' <summary>
  ''' Defines a mapping between two sets of
  ''' properties/fields for use by
  ''' DataMapper.
  ''' </summary>
  Public Class DataMap

#Region "MapElement"

    Friend Class MemberMapping
      Private _from As MemberInfo
      Public ReadOnly Property FromMember() As MemberInfo
        Get
          Return _from
        End Get
      End Property

      Private _to As MemberInfo
      Public ReadOnly Property ToMember() As MemberInfo
        Get
          Return _to
        End Get
      End Property

      Public Sub New(ByVal fromMember As MemberInfo, ByVal toMember As MemberInfo)
        _from = fromMember
        _to = toMember
      End Sub
    End Class

#End Region

    Private _sourceType As Type
    Private _targetType As Type
    Private _map As List(Of MemberMapping) = New List(Of MemberMapping)()
    Private _fieldFlags As BindingFlags = BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance
    Private _propertyFlags As BindingFlags = BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.FlattenHierarchy

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
      _map.Add(New MemberMapping(_sourceType.GetProperty(sourceProperty, _propertyFlags), _targetType.GetProperty(targetProperty, _propertyFlags)))
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
      _map.Add(New MemberMapping(_sourceType.GetField(sourceField, _fieldFlags), _targetType.GetField(targetField, _fieldFlags)))
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
      _map.Add(New MemberMapping(_sourceType.GetField(sourceField, _fieldFlags), _targetType.GetProperty(targetProperty, _propertyFlags)))
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
      _map.Add(New MemberMapping(_sourceType.GetProperty(sourceProperty, _propertyFlags), _targetType.GetField(targetField, _fieldFlags)))
    End Sub

  End Class

End Namespace