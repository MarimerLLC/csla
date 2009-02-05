Imports System
Imports System.Collections.Generic

Namespace Core.FieldManager
  Friend Class PropertyInfoList
    Inherits List(Of IPropertyInfo)

    Private _IsLocked As Boolean
    Public Property IsLocked() As Boolean
      Get
        Return _IsLocked
      End Get
      Set(ByVal value As Boolean)
        _IsLocked = value
      End Set
    End Property

    Public Sub New()

    End Sub

    Public Sub New(ByVal list As List(Of IPropertyInfo))
      MyBase.New(list)
    End Sub

  End Class
End Namespace

