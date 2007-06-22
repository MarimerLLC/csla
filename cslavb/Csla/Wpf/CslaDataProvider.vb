#If Not NET20 Then
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

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()
      mCommandManager = New CslaDataProviderCommandManager(Me)
      mFactoryParameters = New ObservableCollection(Of Object)()
      AddHandler mFactoryParameters.CollectionChanged, AddressOf _factoryParameters_CollectionChanged
    End Sub

    Private Sub _factoryParameters_CollectionChanged(ByVal sender As Object, ByVal e As System.Collections.Specialized.NotifyCollectionChangedEventArgs)
      BeginQuery()
    End Sub

#Region " Properties "

    Private mObjectType As Type = Nothing
    Private mManageLifetime As Boolean
    Private mFactoryMethod As String = String.Empty
    Private mFactoryParameters As ObservableCollection(Of Object)
    Private mIsAsynchronous As Boolean
    Private mCommandManager As CslaDataProviderCommandManager

    ''' <summary>
    ''' Gets an object that can be used to execute
    ''' Save and Undo commands on this CslaDataProvider 
    ''' through XAML command bindings.
    ''' </summary>
    Public ReadOnly Property CommandManager() As CslaDataProviderCommandManager
      Get
        Return mCommandManager
      End Get
    End Property

    ''' <summary>
    ''' Gets or sets the type of object 
    ''' to create an instance of.
    ''' </summary>
    Public Property ObjectType() As Type
      Get
        Return mObjectType
      End Get
      Set(ByVal value As Type)
        mObjectType = value
        OnPropertyChanged(New PropertyChangedEventArgs("TypeName"))
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether the
    ''' data control should manage the lifetime of
    ''' the business object, including using n-level
    ''' undo.
    ''' </summary>
    Public Property ManageObjectLifetime() As Boolean
      Get
        Return mManageLifetime
      End Get
      Set(ByVal value As Boolean)
        mManageLifetime = value
        OnPropertyChanged(New PropertyChangedEventArgs("ManageObjectLifetime"))
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
        Return mFactoryMethod
      End Get
      Set(ByVal value As String)
        mFactoryMethod = value
        OnPropertyChanged(New PropertyChangedEventArgs("GetFactoryMethod"))
      End Set
    End Property

    ''' <summary>
    ''' Get the list of parameters to pass
    ''' to the factory method.
    ''' </summary>
    Public ReadOnly Property FactoryParameters() As IList
      Get
        Return mFactoryParameters
      End Get
    End Property

    ''' <summary>
    ''' Gets or sets a value that indicates 
    ''' whether to perform object creation in 
    ''' a worker thread or in the active context.
    ''' </summary>
    Public Property IsAsynchronous() As Boolean
      Get
        Return mIsAsynchronous
      End Get
      Set(ByVal value As Boolean)
        mIsAsynchronous = value
      End Set
    End Property

#End Region

#Region " Query "

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
      request.ObjectType = mObjectType
      request.FactoryMethod = mFactoryMethod
      request.FactoryParameters = mFactoryParameters

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

#End Region

#Region "Cancel/Update/New"

    ''' <summary>
    ''' Cancels changes to the business object, returning
    ''' it to its previous state.
    ''' </summary>
    ''' <remarks>
    ''' This metod does nothing unless ManageLifetime is
    ''' set to true and the object supports n-level undo.
    ''' </remarks>
    Public Sub Cancel()

      Dim undo As Csla.Core.ISupportUndo = TryCast(Me.Data, Csla.Core.ISupportUndo)
      If Not undo Is Nothing AndAlso mManageLifetime Then
        undo.CancelEdit()
        undo.BeginEdit()
      End If

    End Sub

    ''' <summary>
    ''' Accepts changes to the business object, and
    ''' commits them by calling the object's Save()
    ''' method.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' This method does nothing unless the object
    ''' implements Csla.Core.ISavable.
    ''' </para><para>
    ''' If the object implements IClonable, it
    ''' will be cloned, and the clone will be
    ''' saved.
    ''' </para><para>
    ''' If the object supports n-level undo and
    ''' ManageLifetime is true, then this method
    ''' will automatically call ApplyEdit() and
    ''' BeginEdit() appropriately.
    ''' </para>
    ''' </remarks>
    Public Sub Save()

      ' only do something if the object implements
      ' ISavable
      Dim savable As Csla.Core.ISavable = TryCast(Me.Data, Csla.Core.ISavable)
      If Not savable Is Nothing Then
        Dim result As Object = savable
        Dim exceptionResult As Exception = Nothing
        Try
          ' apply edits in memory
          Dim undo As Csla.Core.ISupportUndo = TryCast(savable, Csla.Core.ISupportUndo)
          If Not undo Is Nothing AndAlso mManageLifetime Then
            undo.ApplyEdit()
          End If

          ' clone the object if possible
          Dim clonable As ICloneable = TryCast(savable, ICloneable)
          If Not clonable Is Nothing Then
            savable = CType(clonable.Clone(), Csla.Core.ISavable)
          End If

          ' save the clone
          result = savable.Save()

          ' start editing the resulting object
          undo = TryCast(result, Csla.Core.ISupportUndo)
          If Not undo Is Nothing AndAlso mManageLifetime Then
            undo.BeginEdit()
          End If

        Catch ex As Exception
          exceptionResult = ex
        End Try

        ' clear previous object
        MyBase.OnQueryFinished(Nothing, Nothing, Nothing, Nothing)
        ' return result to base class
        MyBase.OnQueryFinished(result, exceptionResult, Nothing, Nothing)
      End If

    End Sub

    ''' <summary>
    ''' Adds a new item to the object if the object
    ''' implements IBindingList and AllowNew is true.
    ''' </summary>
    Public Sub AddNew()

      ' only do something if the object implements
      ' IBindingList
      Dim list As IBindingList = TryCast(Me.Data, IBindingList)
      If list IsNot Nothing AndAlso list.AllowNew Then
        list.AddNew()
      End If

    End Sub

#End Region

  End Class

End Namespace
#End If