Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Reflection

Namespace Reflection
  Friend Class DynamicMemberHandle

    Private _memberName As String

    Public Property MemberName() As String
      Get
        Return _memberName
      End Get
      Private Set(ByVal value As String)
        _memberName = value
      End Set
    End Property

    Private _memberType As Type
    Public Property MemberType() As Type
      Get
        Return _memberType
      End Get
      Private Set(ByVal value As Type)
        _memberType = value
      End Set
    End Property

    Private _dynamicMemberGet As DynamicMemberGetDelegate

    Public Property DynamicMemberGet() As DynamicMemberGetDelegate
      Get
        Return _dynamicMemberGet
      End Get
      Private Set(ByVal value As DynamicMemberGetDelegate)
        _dynamicMemberGet = value
      End Set
    End Property

    Private _dynamicMemberSet As DynamicMemberSetDelegate

    Public Property DynamicMemberSet() As DynamicMemberSetDelegate
      Get
        Return _dynamicMemberSet
      End Get
      Private Set(ByVal value As DynamicMemberSetDelegate)
        _dynamicMemberSet = value
      End Set
    End Property

    Public Sub New(ByVal memberName As String, ByVal memberType As Type, ByVal dynamicMemberGet As DynamicMemberGetDelegate, ByVal dynamicMemberSet As DynamicMemberSetDelegate)
      Me.MemberName = memberName
      Me.MemberType = memberType
      Me.DynamicMemberGet = dynamicMemberGet
      Me.DynamicMemberSet = dynamicMemberSet
    End Sub

    Public Sub New(ByVal info As PropertyInfo)
      Me.New(info.Name, info.PropertyType, DynamicMethodHandlerFactory.CreatePropertyGetter(info), DynamicMethodHandlerFactory.CreatePropertySetter(info))
    End Sub
    Public Sub New(ByVal info As FieldInfo)
      Me.New(info.Name, info.FieldType, DynamicMethodHandlerFactory.CreateFieldGetter(info), DynamicMethodHandlerFactory.CreateFieldSetter(info))
    End Sub
  End Class
End Namespace