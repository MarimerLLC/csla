Imports System.Windows
Imports System.Windows.Controls

''' <summary>
''' Base class for edit forms that integrate
''' with the MainForm navigation code.
''' </summary>
Public Class EditForm
  Inherits UserControl

  Implements IRefresh

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  Public Sub New()
    AddHandler Loaded, AddressOf EditForm_Loaded
  End Sub

  Private Sub EditForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
    ApplyAuthorization()
  End Sub

  Private Sub IRefresh_Refresh() Implements IRefresh.Refresh
    ApplyAuthorization()
  End Sub

  ''' <summary>
  ''' Override this method and use to apply
  ''' authorization rules as the form loads
  ''' or the current user changes.
  ''' </summary>
  Protected Overridable Sub ApplyAuthorization()
  End Sub

#Region "Title property"

  Public Event TitleChanged As EventHandler

  ''' <summary>
  ''' Gets or sets the title of this
  ''' edit form.
  ''' </summary>
  Public Property Title() As String
    Get
      Return CStr(GetValue(TitleProperty))
    End Get
    Set(ByVal value As String)
      SetValue(TitleProperty, value)
      If Not TitleChangedEvent Is Nothing Then
        RaiseEvent TitleChanged(Me, EventArgs.Empty)
      End If
    End Set
  End Property

  ' Using a DependencyProperty as the backing store for _title.  This enables animation, styling, binding, etc...
  Public Shared ReadOnly TitleProperty As DependencyProperty = DependencyProperty.Register("Title", GetType(String), GetType(EditForm), Nothing)

#End Region

  Protected Overridable Sub DataChanged(ByVal sender As Object, ByVal e As EventArgs)

    Dim dp As Csla.Wpf.CslaDataProvider = TryCast(sender, Csla.Wpf.CslaDataProvider)
    If Not dp.Error Is Nothing Then
      MessageBox.Show(dp.Error.ToString(), "Data error", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    End If

  End Sub

End Class
