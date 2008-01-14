Imports System.Reflection

Namespace Data

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

    Public Sub New(ByVal sourceType As Type, ByVal targetType As Type)
      _sourceType = sourceType
      _targetType = targetType
    End Sub

    Friend ReadOnly Property SourceType() As Type
      Get
        Return _sourceType
      End Get
    End Property

    Friend Function GetMap() As List(Of MemberMapping)
      Return _map
    End Function

    Public Sub AddPropertyMapping(ByVal sourceProperty As String, ByVal targetProperty As String)
      _map.Add(New MemberMapping(_sourceType.GetProperty(sourceProperty, _propertyFlags), _targetType.GetProperty(targetProperty, _propertyFlags)))
    End Sub

    Public Sub AddFieldMapping(ByVal sourceField As String, ByVal targetField As String)
      _map.Add(New MemberMapping(_sourceType.GetField(sourceField, _fieldFlags), _targetType.GetField(targetField, _fieldFlags)))
    End Sub

    Public Sub AddFieldToPropertyMapping(ByVal sourceField As String, ByVal targetProperty As String)
      _map.Add(New MemberMapping(_sourceType.GetField(sourceField, _fieldFlags), _targetType.GetProperty(targetProperty, _propertyFlags)))
    End Sub

    Public Sub AddPropertyToFieldMapping(ByVal sourceProperty As String, ByVal targetField As String)
      _map.Add(New MemberMapping(_sourceType.GetProperty(sourceProperty, _propertyFlags), _targetType.GetField(targetField, _fieldFlags)))
    End Sub

  End Class

End Namespace