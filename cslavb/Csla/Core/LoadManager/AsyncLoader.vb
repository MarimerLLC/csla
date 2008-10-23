Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Reflection
Imports System.Reflection.Emit

Namespace Core.LoadManager
  Friend Class AsyncLoader
    Private _property As IPropertyInfo
    Private _loadProperty As Action(Of IPropertyInfo, Object)
    Private _propertyChanged As Action(Of String)
    Private _factory As [Delegate]
    Private _parameters As Object()

    Public ReadOnly Property [Property]() As IPropertyInfo
      Get
        Return _property
      End Get
    End Property

    Public Sub New(ByVal [property] As IPropertyInfo, ByVal factory As [Delegate], ByVal loadProperty As Action(Of IPropertyInfo, Object), ByVal propertyChanged As Action(Of String), ByVal ParamArray parameters As Object())
      _property = [property]
      _loadProperty = loadProperty
      _propertyChanged = propertyChanged
      _factory = factory
      _parameters = parameters
    End Sub

    Public Event Complete As EventHandler(Of ErrorEventArgs)
    Protected Sub OnComplete(ByVal [error] As Exception)
      RaiseEvent Complete(Me, New ErrorEventArgs(Me, [error]))
    End Sub

    Friend Sub Load(ByVal handler As [Delegate])
      Dim parameters As New List(Of Object)(_parameters)
      parameters.Insert(0, handler)

      _factory.DynamicInvoke(parameters.ToArray())
    End Sub

    Public Sub LoadComplete(Of R)(ByVal sender As Object, ByVal result As DataPortalResult(Of R))
      Dim obj As R = result.[Object]

      _loadProperty(_property, obj)
      _propertyChanged(_property.Name)
      OnComplete(result.[Error])
    End Sub
  End Class
End Namespace