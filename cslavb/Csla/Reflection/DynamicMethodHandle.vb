Imports System.Reflection

Namespace Reflection

  Friend Class DynamicMethodHandle

    Private privateMethodName As String
    Public Property MethodName() As String
      Get
        Return privateMethodName
      End Get
      Private Set(ByVal value As String)
        privateMethodName = value
      End Set
    End Property

    Private privateDynamicMethod As DynamicMethodDelegate
    Public Property DynamicMethod() As DynamicMethodDelegate
      Get
        Return privateDynamicMethod
      End Get
      Private Set(ByVal value As DynamicMethodDelegate)
        privateDynamicMethod = value
      End Set
    End Property

    Private privateHasFinalArrayParam As Boolean
    Public Property HasFinalArrayParam() As Boolean
      Get
        Return privateHasFinalArrayParam
      End Get
      Private Set(ByVal value As Boolean)
        privateHasFinalArrayParam = value
      End Set
    End Property

    Private privateMethodParamsLength As Integer
    Public Property MethodParamsLength() As Integer
      Get
        Return privateMethodParamsLength
      End Get
      Private Set(ByVal value As Integer)
        privateMethodParamsLength = value
      End Set
    End Property

    Private privateFinalArrayElementType As Type
    Public Property FinalArrayElementType() As Type
      Get
        Return privateFinalArrayElementType
      End Get
      Private Set(ByVal value As Type)
        privateFinalArrayElementType = value
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