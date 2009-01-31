Imports System
Imports System.Collections.Generic

Namespace Core

  Friend Interface IManageProperties
    ReadOnly Property HasManagedProperties() As Boolean
    Function GetManagedProperties() As List(Of IPropertyInfo)
    Function GetProperty(ByVal propertyInfo As IPropertyInfo) As Object
    Function ReadProperty(ByVal propertyInfo As IPropertyInfo) As Object
    Sub SetProperty(ByVal propertyInfo As IPropertyInfo, ByVal newValue As Object)
    Sub LoadProperty(ByVal propertyInfo As IPropertyInfo, ByVal newValue As Object)
    Sub LoadProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P)
  End Interface

End Namespace
