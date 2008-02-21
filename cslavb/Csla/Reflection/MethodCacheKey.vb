Namespace Reflection

  Friend Class MethodCacheKey

    Private _typeName As String
    Public Property TypeName() As String
      Get
        Return _typeName
      End Get
      Private Set(ByVal value As String)
        _typeName = value
      End Set
    End Property

    Private _methodName As String
    Public Property MethodName() As String
      Get
        Return _methodName
      End Get
      Private Set(ByVal value As String)
        _methodName = value
      End Set
    End Property

    Private _paramTypes As Type()
    Public Property ParamTypes() As Type()
      Get
        Return _paramTypes
      End Get
      Private Set(ByVal value As Type())
        _paramTypes = value
      End Set
    End Property

    Private _hashKey As Integer

    Public Sub New(ByVal typeName As String, ByVal methodName As String, ByVal paramTypes() As Type)

      Me.TypeName = typeName
      Me.MethodName = methodName
      Me.ParamTypes = paramTypes

      _hashKey = typeName.GetHashCode()
      _hashKey = _hashKey Xor methodName.GetHashCode()
      For Each item As Type In paramTypes
        _hashKey = _hashKey Xor item.Name.GetHashCode()
      Next item

    End Sub

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean

      Dim key As MethodCacheKey = TryCast(obj, MethodCacheKey)
      If key IsNot Nothing AndAlso key.TypeName = Me.TypeName AndAlso key.MethodName = Me.MethodName AndAlso ArrayEquals(key.ParamTypes, Me.ParamTypes) Then
        Return True
      End If

      Return False

    End Function

    Private Function ArrayEquals(ByVal a1() As Type, ByVal a2() As Type) As Boolean

      If a1.Length <> a2.Length Then
        Return False
      End If

      For pos As Integer = 0 To a1.Length - 1
        If a1(pos) IsNot a2(pos) Then
          Return False
        End If
      Next pos
      Return True

    End Function

    Public Overrides Function GetHashCode() As Integer

      Return _hashKey

    End Function

  End Class

End Namespace