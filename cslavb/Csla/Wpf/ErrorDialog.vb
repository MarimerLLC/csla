Imports System
Imports System.Windows
Imports System.Windows.Controls

Namespace Wpf

  ''' <summary>
  ''' Displays an error dialog for any exceptions
  ''' that occur in a CslaDataProvider.
  ''' </summary>
  Public Class ErrorDialog
    Inherits Control

    ''' <summary>
    ''' Creates a new instance of the control.
    ''' </summary>
    Public Sub New()
      Me.DialogIcon = MessageBoxImage.Exclamation
      AddHandler DataContextChanged, AddressOf ErrorDialog_DataContextChanged

    End Sub

    Public Sub ErrorDialog_DataContextChanged(ByVal sender As Object, ByVal e As DependencyPropertyChangedEventArgs)
      DetachSource(e.OldValue)
      AttachSource(e.NewValue)
    End Sub

    ''' <summary>
    ''' Gets or sets the title of the error
    ''' dialog.
    ''' </summary>
    Public Shared ReadOnly DialogTitleProperty As DependencyProperty = DependencyProperty.Register("DialogTitle", _
                                                                                                            GetType(String), _
                                                                                                            GetType(ErrorDialog), _
                                                                                                            Nothing)

    ''' <summary>
    ''' Gets or sets the title of the error
    ''' dialog
    ''' </summary>
    Public Property DialogTitle() As String
      Get
        Return CType(GetValue(DialogTitleProperty), String)
      End Get
      Set(ByVal value As String)
        SetValue(DialogTitleProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the first line of text displayed
    ''' within the error dialog (before the
    ''' exception message).
    ''' </summary>
    Public Shared ReadOnly DialogFirstLineProperty As DependencyProperty = DependencyProperty.Register("DialogFirstLine", _
                                                                                                       GetType(String), _
                                                                                                       GetType(ErrorDialog), _
                                                                                                       Nothing)

    ''' <summary>
    ''' Gets or sets the first line of text displayed
    ''' within the error dialog (before the
    ''' exception message).
    ''' </summary>
    Public Property DialogFirstLine() As String
      Get
        Return CType(GetValue(DialogFirstLineProperty), String)
      End Get
      Set(ByVal value As String)
        SetValue(DialogFirstLineProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether
    ''' the dialog should include exception details
    ''' or just the exception summary message.
    ''' </summary>
    Public Shared ReadOnly ShowExceptionDetailProperty As DependencyProperty = DependencyProperty.Register("ShowExceptionDetail", _
                                                                                                       GetType(Boolean), _
                                                                                                       GetType(ErrorDialog), _
                                                                                                       Nothing)

    ''' <summary>
    ''' Gets or sets the first line of text displayed
    ''' within the error dialog (before the
    ''' exception message).
    ''' </summary>
    Public Property ShowExceptionDetail() As Boolean
      Get
        Return CType(GetValue(ShowExceptionDetailProperty), Boolean)
      End Get
      Set(ByVal value As Boolean)
        SetValue(ShowExceptionDetailProperty, value)
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether
    ''' the dialog should include exception details
    ''' or just the exception summary message.
    ''' </summary>
    Public Shared ReadOnly DialogIconProperty As DependencyProperty = DependencyProperty.Register("DialogIcon", _
                                                                                                       GetType(MessageBoxImage), _
                                                                                                       GetType(ErrorDialog), _
                                                                                                       Nothing)

    ''' <summary>
    ''' Gets or sets the icon displayed in
    ''' the dialog.
    ''' </summary>
    Public Property DialogIcon() As MessageBoxImage
      Get
        Return CType(GetValue(DialogIconProperty), MessageBoxImage)
      End Get
      Set(ByVal value As MessageBoxImage)
        SetValue(DialogIconProperty, value)
      End Set
    End Property

    Friend Sub Register(ByVal source As Object)
      AttachSource(source)
    End Sub

    Private Sub AttachSource(ByVal source As Object)

      Dim dp As System.Windows.Data.DataSourceProvider = TryCast(source, System.Windows.Data.DataSourceProvider)

      If dp IsNot Nothing Then
        AddHandler dp.DataChanged, AddressOf source_DataChanged
      End If
    End Sub

    Private Sub DetachSource(ByVal source As Object)

      Dim dp As System.Windows.Data.DataSourceProvider = TryCast(source, System.Windows.Data.DataSourceProvider)

      If dp IsNot Nothing Then
        RemoveHandler dp.DataChanged, AddressOf source_DataChanged
      End If
    End Sub

    Private Sub source_DataChanged(ByVal sender As Object, ByVal e As EventArgs)
      Dim dp As System.Windows.Data.DataSourceProvider = TryCast(sender, System.Windows.Data.DataSourceProvider)

      If dp IsNot Nothing AndAlso dp.Error IsNot Nothing Then
        Dim [error] As String
        If Me.ShowExceptionDetail Then
          [error] = dp.Error.ToString()
        Else
          [error] = dp.Error.Message
        End If

        Dim output As String
        If String.IsNullOrEmpty(Me.DialogFirstLine) Then
          output = [error]
        Else
          output = String.Format("{0}{1}{2}", Me.DialogFirstLine, Environment.NewLine, [error])
        End If

        MessageBox.Show(output, Me.DialogTitle, MessageBoxButton.OK, Me.DialogIcon)
      End If

    End Sub


  End Class

End Namespace

