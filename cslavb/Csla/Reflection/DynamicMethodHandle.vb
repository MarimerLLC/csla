Imports System.Reflection

Namespace Reflection

  Friend Class DynamicMethodHandle

    Private _methodName As String
    Public Property MethodName() As String
      Get
        Return _methodName
      End Get
      Private Set(ByVal value As String)
        _methodName = value
      End Set
    End Property

    Private _dynamicMethod As DynamicMethodDelegate
    Public Property DynamicMethod() As DynamicMethodDelegate
      Get
        Return _dynamicMethod
      End Get
      Private Set(ByVal value As DynamicMethodDelegate)
        _dynamicMethod = value
      End Set
    End Property

    Private _hasFinalArrayParam As Boolean
    Public Property HasFinalArrayParam() As Boolean
      Get
        Return _hasFinalArrayParam
      End Get
      Private Set(ByVal value As Boolean)
        _hasFinalArrayParam = value
      End Set
    End Property

    Private _methodParamsLength As Integer
    Public Property MethodParamsLength() As Integer
      Get
        Return _methodParamsLength
      End Get
      Private Set(ByVal value As Integer)
        _methodParamsLength = value
      End Set
    End Property

    Private _finalArrayElementType As Type
    Public Property FinalArrayElementType() As Type
      Get
        Return _finalArrayElementType
      End Get
      Private Set(ByVal value As Type)
        _finalArrayElementType = value
      End Set
    End Property

    Public Sub New(ByVal info As MethodInfo, ByVal ParamArray parameters() As Object)

      If info Is Nothing Then
        Me.DynamicMethod = Nothing

      Else
        Me.MethodName = info.Name
        Dim infoParams = info.GetParameters()
        Dim inParams() As Object = Nothing
        If parameters Is Nothing Then
          inParams = New Object() {Nothing}

        Else
          inParams = parameters
        End If
        Dim pCount = infoParams.Length
        If pCount > 0 AndAlso ((pCount = 1 AndAlso infoParams(0).ParameterType.IsArray) OrElse (infoParams(pCount - 1).GetCustomAttributes(GetType(ParamArrayAttribute), True).Length > 0)) Then
          Me.HasFinalArrayParam = True
          Me.MethodParamsLength = pCount
          Me.FinalArrayElementType = infoParams(pCount - 1).ParameterType
        End If
        Me.DynamicMethod = DynamicMethodHandlerFactory.CreateMethod(info)
      End If

    End Sub

  End Class

End Namespace