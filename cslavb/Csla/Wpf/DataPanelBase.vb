#If Not NET20 Then
Imports System.Collections.Specialized
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.ComponentModel
Imports System.Reflection

Namespace Wpf

  ''' <summary>
  ''' Base class for creating WPF panel
  ''' controls that react when the DataContext,
  ''' data object and data property values
  ''' are changed.
  ''' </summary>
  Public Class DataPanelBase
    Inherits StackPanel

    Private mLoaded As Boolean
    Private mDataObject As Object

    ''' <summary>
    ''' Gets a reference to the current
    ''' data object.
    ''' </summary>
    ''' <remarks>
    ''' The DataContext may not be the data object. The
    ''' DataContext may be a DataSourceProvider control.
    ''' This property returns a reference to the actual
    ''' <b>data object</b>, not necessarily the DataContext
    ''' itself.
    ''' </remarks>
    Protected ReadOnly Property DataObject() As Object
      Get
        Return mDataObject
      End Get
    End Property

    ''' <summary>
    ''' This method is called when a property
    ''' of the data object to which the 
    ''' control is bound has changed.
    ''' </summary>
    Protected Overridable Sub DataPropertyChanged(ByVal e As PropertyChangedEventArgs)
      ' may be overridden by subclass
    End Sub

    ''' <summary>
    ''' This method is called if the data
    ''' object is an IBindingList, and the 
    ''' ListChanged event was raised by
    ''' the data object.
    ''' </summary>
    Protected Overridable Sub DataBindingListChanged(ByVal e As ListChangedEventArgs)
      ' may be overridden by subclass
    End Sub

    ''' <summary>
    ''' This method is called if the data
    ''' object is an INotifyCollectionChanged, 
    ''' and the CollectionChanged event was 
    ''' raised by the data object.
    ''' </summary>
    Protected Overridable Sub DataObservableCollectionChanged(ByVal e As NotifyCollectionChangedEventArgs)
      ' may be overridden by subclass
    End Sub

    ''' <summary>
    ''' This method is called when the data
    ''' object to which the control is bound
    ''' has changed.
    ''' </summary>
    Protected Overridable Sub DataObjectChanged()
      ' may be overridden by subclass
    End Sub

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()
      AddHandler DataContextChanged, AddressOf Panel_DataContextChanged
      AddHandler Loaded, AddressOf Panel_Loaded
    End Sub

    ''' <summary>
    ''' Handle case where the DataContext for the
    ''' control has changed.
    ''' </summary>
    Private Sub Panel_DataContextChanged(ByVal sender As Object, ByVal e As DependencyPropertyChangedEventArgs)
      UnHookDataContextEvents(e.OldValue)

      ' store a ref to the data object
      mDataObject = GetDataObject(e.NewValue)

      HookDataContextEvents(e.NewValue)

      If mLoaded Then
        DataObjectChanged()
      End If
    End Sub

    Private Function GetDataObject(ByVal dataContext As Object) As Object
      Dim provider As DataSourceProvider = TryCast(dataContext, DataSourceProvider)
      If Not provider Is Nothing Then
        Return provider.Data
      Else
        Return dataContext
      End If
    End Function

    ''' <summary>
    ''' Handle case where the Data property of the
    ''' DataContext (a DataSourceProvider) has changed.
    ''' </summary>
    Private Sub DataProvider_DataChanged(ByVal sender As Object, ByVal e As EventArgs)
      UnHookPropertyChanged(TryCast(mDataObject, INotifyPropertyChanged))

      mDataObject = TryCast((CType(sender, DataSourceProvider)).Data, IDataErrorInfo)

      HookPropertyChanged(TryCast(mDataObject, INotifyPropertyChanged))

      DataObjectChanged()
    End Sub

    Private Sub UnHookDataContextEvents(ByVal oldValue As Object)
      ' unhook any old event handling
      Dim oldContext As Object = Nothing

      Dim provider As DataSourceProvider = TryCast(oldValue, DataSourceProvider)
      If provider Is Nothing Then
        oldContext = oldValue
      Else
        RemoveHandler provider.DataChanged, AddressOf DataProvider_DataChanged
        oldContext = provider.Data
      End If
      UnHookPropertyChanged(TryCast(oldContext, INotifyPropertyChanged))
      UnHookBindingListChanged(TryCast(oldContext, IBindingList))
      UnHookObservableListChanged(TryCast(oldContext, INotifyCollectionChanged))
    End Sub

    Private Sub HookDataContextEvents(ByVal newValue As Object)
      ' hook any new event
      Dim newContext As Object = Nothing

      Dim provider As DataSourceProvider = TryCast(newValue, DataSourceProvider)
      If provider Is Nothing Then
        newContext = newValue
      Else
        AddHandler provider.DataChanged, AddressOf DataProvider_DataChanged
        newContext = provider.Data
      End If
      HookPropertyChanged(TryCast(newContext, INotifyPropertyChanged))
      HookBindingListChanged(TryCast(newContext, IBindingList))
      HookObservableListChanged(TryCast(newContext, INotifyCollectionChanged))
    End Sub

    Private Sub UnHookPropertyChanged(ByVal oldContext As INotifyPropertyChanged)
      If Not oldContext Is Nothing Then
        RemoveHandler oldContext.PropertyChanged, AddressOf DataObject_PropertyChanged
      End If
    End Sub

    Private Sub HookPropertyChanged(ByVal newContext As INotifyPropertyChanged)
      If Not newContext Is Nothing Then
        AddHandler newContext.PropertyChanged, AddressOf DataObject_PropertyChanged
      End If
    End Sub

    Private Sub UnHookBindingListChanged(ByVal oldContext As IBindingList)
      If Not oldContext Is Nothing Then
        RemoveHandler oldContext.ListChanged, AddressOf DataObject_ListChanged
      End If
    End Sub

    Private Sub HookBindingListChanged(ByVal newContext As IBindingList)
      If Not newContext Is Nothing Then
        AddHandler newContext.ListChanged, AddressOf DataObject_ListChanged
      End If
    End Sub

    Private Sub UnHookObservableListChanged(ByVal oldContext As INotifyCollectionChanged)
      If Not oldContext Is Nothing Then
        RemoveHandler oldContext.CollectionChanged, AddressOf DataObject_CollectionChanged
      End If
    End Sub

    Private Sub HookObservableListChanged(ByVal newContext As INotifyCollectionChanged)
      If Not newContext Is Nothing Then
        AddHandler newContext.CollectionChanged, AddressOf DataObject_CollectionChanged
      End If
    End Sub

    Private Sub Panel_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
      mLoaded = True
      If Not mDataObject Is Nothing Then
        DataObjectChanged()
      End If
    End Sub

    Private Sub DataObject_PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
      DataPropertyChanged(e)
    End Sub

    Private Sub DataObject_ListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
      DataBindingListChanged(e)
    End Sub

    Private Sub DataObject_CollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
      DataObservableCollectionChanged(e)
    End Sub

    ''' <summary>
    ''' Scans all child controls of this panel
    ''' for object bindings, and calls
    ''' <see cref="FoundBinding"/> for each
    ''' binding found.
    ''' </summary>
    Protected Sub FindChildBindings()
      FindBindings(Me)
    End Sub

    Private Sub FindBindings(ByVal visual As Visual)
      For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(visual) - 1
        Dim childVisual As Visual = CType(VisualTreeHelper.GetChild(visual, i), Visual)
        Dim sharedMembers As MemberInfo() = childVisual.GetType().GetMembers(BindingFlags.Static Or BindingFlags.Public Or BindingFlags.FlattenHierarchy)
        For Each member As MemberInfo In sharedMembers
          Dim prop As DependencyProperty = Nothing
          If member.MemberType = MemberTypes.Field Then
            prop = TryCast((CType(member, FieldInfo)).GetValue(childVisual), DependencyProperty)
          ElseIf member.MemberType = MemberTypes.Property Then
            prop = TryCast((CType(member, PropertyInfo)).GetValue(childVisual, Nothing), DependencyProperty)
          End If

          If Not prop Is Nothing Then
            Dim bnd As Binding = BindingOperations.GetBinding(childVisual, prop)
            If Not bnd Is Nothing AndAlso bnd.RelativeSource Is Nothing AndAlso Not bnd.Path Is Nothing AndAlso String.IsNullOrEmpty(bnd.ElementName) Then
              FoundBinding(bnd, CType(childVisual, FrameworkElement), prop)
            End If
          End If
        Next member
        FindBindings(childVisual)
      Next i
    End Sub

    ''' <summary>
    ''' Called by
    ''' <see cref="FindChildBindings"/> each
    ''' time an object binding is found.
    ''' </summary>
    ''' <param name="bnd">The Binding object.</param>
    ''' <param name="control">The control containing the binding.</param>
    ''' <param name="prop">The data bound DependencyProperty.</param>
    Protected Overridable Sub FoundBinding(ByVal bnd As Binding, ByVal control As FrameworkElement, ByVal prop As DependencyProperty)
      ' may be overridden by subclass
    End Sub

  End Class

End Namespace
#End If