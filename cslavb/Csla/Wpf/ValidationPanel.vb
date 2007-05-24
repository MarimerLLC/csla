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
  ''' Container for other UI controls that adds
  ''' the ability for the contained controls
  ''' to change appearance based on the error
  ''' information provided by the data binding
  ''' context.
  ''' </summary>
  Public Class ValidationPanel
    Inherits DataPanelBase

    Private mHaveRecentChange As Boolean

    ''' <summary>
    ''' Creates a new instance of the object.
    ''' </summary>
    Public Sub New()
      AddHandler Loaded, AddressOf ValidationPanel_Loaded
    End Sub

    ''' <summary>
    ''' Force the panel to refresh all validation
    ''' error status information for all controls
    ''' it contains.
    ''' </summary>
    Public Sub Refresh()
      mHaveRecentChange = True
      ErrorScan()
    End Sub

    ''' <summary>
    ''' Reload all the binding information for the 
    ''' controls contained within the
    ''' ErrorDisplayContainer, and refresh
    ''' the validation status.
    ''' </summary>
    Public Sub ReloadBindings()
      ReloadBindings(True)
    End Sub

    Private Sub ReloadBindings(ByVal refreshAfter As Boolean)
      mBindings.Clear()
      MyBase.FindChildBindings()
      If refreshAfter Then
        Refresh()
      End If
    End Sub

    ''' <summary>
    ''' This method is called when a property
    ''' of the data object to which the 
    ''' control is bound has changed.
    ''' </summary>
    Protected Overrides Sub DataPropertyChanged(ByVal e As PropertyChangedEventArgs)
      ' note that there's been a change, so the 
      ' next scan will perform validation
      mHaveRecentChange = True
    End Sub

    ''' <summary>
    ''' This method is called if the data
    ''' object is an IBindingList, and the 
    ''' ListChanged event was raised by
    ''' the data object.
    ''' </summary>
    Protected Overrides Sub DataBindingListChanged(ByVal e As ListChangedEventArgs)
      Refresh()
    End Sub

    ''' <summary>
    ''' This method is called if the data
    ''' object is an INotifyCollectionChanged, 
    ''' and the CollectionChanged event was 
    ''' raised by the data object.
    ''' </summary>
    Protected Overrides Sub DataObservableCollectionChanged(ByVal e As System.Collections.Specialized.NotifyCollectionChangedEventArgs)
      Refresh()
    End Sub

    ''' <summary>
    ''' This method is called when the data
    ''' object to which the control is bound
    ''' has changed.
    ''' </summary>
    Protected Overrides Sub DataObjectChanged()
      ReloadBindings()
    End Sub

#Region "Trigger Validation"

    Private Sub ValidationPanel_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
      AddHandler (CType(Me, FrameworkElement)).LostFocus, AddressOf ValidationPanel_LostFocus
      mHaveRecentChange = True
      'ErrorScan();
    End Sub

    Private Sub ValidationPanel_LostFocus(ByVal sender As Object, ByVal e As RoutedEventArgs)
      ErrorScan()
    End Sub

    Private Sub ValidationPanel_GotFocus(ByVal sender As Object, ByVal e As RoutedEventArgs)
      ErrorScan()
    End Sub

#End Region

#Region "Validation implementation"

    Private Sub ErrorScan()
      Dim source As IDataErrorInfo = TryCast(DataObject, IDataErrorInfo)
      If mHaveRecentChange AndAlso Not source Is Nothing Then
        mHaveRecentChange = False
        If mBindings.Count = 0 Then
          ReloadBindings(False)
        End If

        If Not source Is Nothing AndAlso mBindings.Count > 0 Then
          For Each item As BindingInfo In mBindings
            Dim expression As BindingExpression = item.Element.GetBindingExpression(item.Property)
            If Not expression Is Nothing Then
              Dim text As String = source(item.BindingObject.Path.Path)
              If String.IsNullOrEmpty(text) Then
                System.Windows.Controls.Validation.ClearInvalid(expression)
              Else
                Dim [error] As ValidationError = New ValidationError(New ExceptionValidationRule(), expression, text, Nothing)
                System.Windows.Controls.Validation.MarkInvalid(expression, [error])
              End If
            End If
          Next item
        End If
      End If
    End Sub

    Private mBindings As List(Of BindingInfo) = New List(Of BindingInfo)()

    ''' <summary>
    ''' Store the binding for use in
    ''' validation processing.
    ''' </summary>
    ''' <param name="bnd">The Binding object.</param>
    ''' <param name="control">The control containing the binding.</param>
    ''' <param name="prop">The data bound DependencyProperty.</param>
    Protected Overrides Sub FoundBinding(ByVal bnd As Binding, ByVal control As FrameworkElement, ByVal prop As DependencyProperty)
      mBindings.Add(New BindingInfo(bnd, control, prop))
      AddHandler control.GotFocus, AddressOf ValidationPanel_GotFocus
    End Sub

#Region "BindingInfo Class"

    ''' <summary>
    ''' Contains details about each binding that
    ''' are required to handle the validation
    ''' processing.
    ''' </summary>
    Private Class BindingInfo
      Private mBindingObject As Binding

      Public Property BindingObject() As Binding
        Get
          Return mBindingObject
        End Get
        Set(ByVal value As Binding)
          mBindingObject = value
        End Set
      End Property

      Private mElement As FrameworkElement

      Public Property Element() As FrameworkElement
        Get
          Return mElement
        End Get
        Set(ByVal value As FrameworkElement)
          mElement = value
        End Set
      End Property

      Private mProperty As DependencyProperty

      Public Property [Property]() As DependencyProperty
        Get
          Return mProperty
        End Get
        Set(ByVal value As DependencyProperty)
          mProperty = value
        End Set
      End Property

      Public Sub New(ByVal binding As Binding, ByVal element As FrameworkElement, ByVal [property] As DependencyProperty)
        mBindingObject = binding
        mElement = element
        mProperty = [property]
      End Sub
    End Class

#End Region

#End Region

  End Class

End Namespace
