Namespace Core

  Friend Interface IManageProperties
    ReadOnly Property HasManagedProperties() As Boolean
    Function GetManagedProperties() As List(Of IPropertyInfo)
    Function GetProperty(ByVal propertyInfo As IPropertyInfo) As Object
    Function ReadProperty(ByVal propertyInfo As IPropertyInfo) As Object
    Sub SetProperty(ByVal propertyInfo As IPropertyInfo, ByVal newValue As Object)
    Sub LoadProperty(ByVal propertyInfo As IPropertyInfo, ByVal newValue As Object)
  End Interface

End Namespace
