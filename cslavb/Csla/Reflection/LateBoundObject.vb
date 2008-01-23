Namespace Reflection

  ''' <summary>
  ''' Enables simple invocation of methods
  ''' against the contained object using 
  ''' late binding.
  ''' </summary>
  Public Class LateBoundObject

    ''' <summary>
    ''' Object instance managed by LateBoundObject.
    ''' </summary>
    Private privateInstance As Object
    Public Property Instance() As Object
      Get
        Return privateInstance
      End Get
      Set(ByVal value As Object)
        privateInstance = value
      End Set
    End Property

    ''' <summary>
    ''' Creates an instance of the specified
    ''' type and contains it within a new
    ''' LateBoundObject.
    ''' </summary>
    ''' <param name="objectType">
    ''' Type of object to create.
    ''' </param>
    ''' <remarks>
    ''' The specified type must implement a
    ''' default constructor.
    ''' </remarks>
    Public Sub New(ByVal objectType As Type)
      Me.New(MethodCaller.CreateInstance(objectType))
    End Sub

    ''' <summary>
    ''' Contains the provided object within
    ''' a new LateBoundObject.
    ''' </summary>
    ''' <param name="instance">
    ''' Object to contain.
    ''' </param>
    Public Sub New(ByVal instance As Object)
      Me.Instance = instance
    End Sub

    ''' <summary>
    ''' Uses reflection to dynamically invoke a method
    ''' if that method is implemented on the target object.
    ''' </summary>
    ''' <param name="method">
    ''' Name of the method.
    ''' </param>
    ''' <param name="parameters">
    ''' Parameters to pass to method.
    ''' </param>
    Public Function CallMethodIfImplemented(ByVal method As String, ByVal ParamArray parameters() As Object) As Object
      Return MethodCaller.CallMethodIfImplemented(Me.Instance, method, parameters)
    End Function

    ''' <summary>
    ''' Uses reflection to dynamically invoke a method,
    ''' throwing an exception if it is not
    ''' implemented on the target object.
    ''' </summary>
    ''' <param name="method">
    ''' Name of the method.
    ''' </param>
    Public Function CallMethod(ByVal method As String) As Object
      Return MethodCaller.CallMethod(Me.Instance, method)
    End Function

    ''' <summary>
    ''' Uses reflection to dynamically invoke a method,
    ''' throwing an exception if it is not
    ''' implemented on the target object.
    ''' </summary>
    ''' <param name="method">
    ''' Name of the method.
    ''' </param>
    ''' <param name="parameters">
    ''' Parameters to pass to method.
    ''' </param>
    Public Function CallMethod(ByVal method As String, ByVal ParamArray parameters() As Object) As Object
      Return MethodCaller.CallMethod(Me.Instance, method, parameters)
    End Function

  End Class

End Namespace