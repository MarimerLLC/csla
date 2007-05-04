Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Text
Imports System.Windows.Data
Imports System.Reflection

Namespace Wpf

  ''' <summary>
  ''' Wraps and creates a CSLA .NET-style object 
  ''' that you can use as a binding source.
  ''' </summary>
  Public Class CslaDataProvider
    Inherits DataSourceProvider

    Private _objectType As Type = Nothing
    Private _factoryMethod As String = String.Empty
    Private _factoryParameters As ObservableCollection(Of Object)
    Private _isAsynchronous As Boolean

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()
      _factoryParameters = New ObservableCollection(Of Object)()
      AddHandler _factoryParameters.CollectionChanged, AddressOf _factoryParameters_CollectionChanged
    End Sub

    Private Sub _factoryParameters_CollectionChanged(ByVal sender As Object, ByVal e As System.Collections.Specialized.NotifyCollectionChangedEventArgs)
      BeginQuery()
    End Sub

    ''' <summary>
    ''' Gets or sets the type of object 
    ''' to create an instance of.
    ''' </summary>
    Public Property ObjectType() As Type
      Get
        Return _objectType
      End Get
      Set(ByVal value As Type)
        _objectType = value
        OnPropertyChanged(New PropertyChangedEventArgs("TypeName"))
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the name of the static
    ''' (Shared in Visual Basic) factory method
    ''' that should be called to create the
    ''' object instance.
    ''' </summary>
    Public Property FactoryMethod() As String
      Get
        Return _factoryMethod
      End Get
      Set(ByVal value As String)
        _factoryMethod = value
        OnPropertyChanged(New PropertyChangedEventArgs("GetFactoryMethod"))
      End Set
    End Property

    ''' <summary>
    ''' Get the list of parameters to pass
    ''' to the factory method.
    ''' </summary>
    Public ReadOnly Property FactoryParameters() As IList
      Get
        Return _factoryParameters
      End Get
    End Property

    ''' <summary>
    ''' Gets or sets a value that indicates 
    ''' whether to perform object creation in 
    ''' a worker thread or in the active context.
    ''' </summary>
    Public Property IsAsynchronous() As Boolean
      Get
        Return _isAsynchronous
      End Get
      Set(ByVal value As Boolean)
        _isAsynchronous = value
      End Set
    End Property

    ''' <summary>
    ''' Overridden. Starts to create the requested object, 
    ''' either immediately or on a background thread, 
    ''' based on the value of the IsAsynchronous property.
    ''' </summary>
    Protected Overrides Sub BeginQuery()
      If Me.IsRefreshDeferred Then
        Return
      End If
      Dim request As QueryRequest = New QueryRequest()
      request.ObjectType = _objectType
      request.FactoryMethod = _factoryMethod
      request.FactoryParameters = _factoryParameters

      If IsAsynchronous Then
        System.Threading.ThreadPool.QueueUserWorkItem(AddressOf DoQuery, request)
      Else
        DoQuery(request)
      End If
    End Sub

    Private Sub DoQuery(ByVal state As Object)
      Dim request As QueryRequest = CType(state, QueryRequest)
      Dim result As Object = Nothing
      Dim exceptionResult As Exception = Nothing
      Dim parameters As Object() = New List(Of Object)(request.FactoryParameters).ToArray()

      Try
        ' get factory method info
        Dim flags As BindingFlags = BindingFlags.Static Or BindingFlags.Public Or BindingFlags.FlattenHierarchy
        Dim factory As MethodInfo = request.ObjectType.GetMethod(request.FactoryMethod, flags, Nothing, MethodCaller.GetParameterTypes(parameters), Nothing)

        If factory Is Nothing Then
          ' strongly typed factory couldn't be found
          ' so find one with the correct number of
          ' parameters 
          Dim parameterCount As Integer = parameters.Length
          Dim methods As MethodInfo() = request.ObjectType.GetMethods(flags)
          For Each method As MethodInfo In methods
            If method.Name = request.FactoryMethod AndAlso method.GetParameters().Length = parameterCount Then
              factory = method
              Exit For
            End If
          Next method
        End If

        If factory Is Nothing Then
          ' no matching factory could be found
          ' so throw exception
          Throw New InvalidOperationException(String.Format(My.Resources.NoSuchFactoryMethod, request.FactoryMethod))
        End If

        ' invoke factory method
        Try
          result = factory.Invoke(Nothing, parameters)
        Catch ex As Csla.DataPortalException
          exceptionResult = ex.BusinessException
        Catch ex As Exception
          exceptionResult = ex
        End Try
      Catch ex As Exception
        exceptionResult = ex
      End Try

      ' return result to base class
      MyBase.OnQueryFinished(result, exceptionResult, Nothing, Nothing)
    End Sub

#Region "QueryRequest Class"

    Private Class QueryRequest
      Private _objectType As Type

      Public Property ObjectType() As Type
        Get
          Return _objectType
        End Get
        Set(ByVal value As Type)
          _objectType = value
        End Set
      End Property

      Private _factoryMethod As String

      Public Property FactoryMethod() As String
        Get
          Return _factoryMethod
        End Get
        Set(ByVal value As String)
          _factoryMethod = value
        End Set
      End Property

      Private _factoryParameters As ObservableCollection(Of Object)

      Public Property FactoryParameters() As ObservableCollection(Of Object)
        Get
          Return _factoryParameters
        End Get
        Set(ByVal value As ObservableCollection(Of Object))
          _factoryParameters = New ObservableCollection(Of Object)(New List(Of Object)(value))
        End Set
      End Property
    End Class

#End Region

  End Class

End Namespace
