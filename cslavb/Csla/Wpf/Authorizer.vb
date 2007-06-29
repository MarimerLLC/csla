#If Not NET20 Then
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
Imports Csla.Security

Namespace Wpf

  ''' <summary>
  ''' Container for other UI controls that adds
  ''' the ability for the contained controls
  ''' to change state based on the authorization
  ''' information provided by the data binding
  ''' context.
  ''' </summary>
  Public Class Authorizer
    Inherits DataDecoratorBase

#Region "NotVisibleMode property"

    ' Define DependencyProperty
    Private Shared ReadOnly NotVisibleModeProperty As DependencyProperty = _
      DependencyProperty.Register("NotVisibleMode", GetType(VisibilityMode), _
      GetType(Authorizer), New FrameworkPropertyMetadata(VisibilityMode.Hidden), AddressOf IsValidVisibilityMode)

    ' Define method to validate the value
    Private Shared Function IsValidVisibilityMode(ByVal o As Object) As Boolean
      Return (TypeOf o Is VisibilityMode)
    End Function

    ''' <summary>
    ''' Gets or sets the value controlling how controls
    ''' bound to non-readable properties will be rendered.
    ''' </summary>
    Public Property NotVisibleMode() As VisibilityMode
      Get
        Return CType(MyBase.GetValue(NotVisibleModeProperty), VisibilityMode)
      End Get
      Set(ByVal value As VisibilityMode)
        MyBase.SetValue(NotVisibleModeProperty, value)
      End Set
    End Property

#End Region

#Region " ControlVisibility property "

    Private Shared ControlVisibilityProperty As DependencyProperty = _
      DependencyProperty.RegisterAttached( _
      "ControlVisibility", GetType(VisibilityMode), GetType(Authorizer))

    ''' <summary>
    ''' Gets a value specifying the visibility mode
    ''' of the specified control. This value is
    ''' used to override the NotVisibleMode property
    ''' for a specific control.
    ''' </summary>
    Public Shared Function GetControlVisibility(ByVal obj As DependencyObject) As VisibilityMode

      Return CType(obj.GetValue(ControlVisibilityProperty), VisibilityMode)

    End Function

    ''' <summary>
    ''' Sets a value specifying the visibility mode
    ''' of the specified control. This value is
    ''' used to override the NotVisibleMode property
    ''' for a specific control.
    ''' </summary>
    Public Shared Sub SetControlVisibility(ByVal obj As DependencyObject, ByVal visibility As VisibilityMode)

      obj.SetValue(ControlVisibilityProperty, visibility)

    End Sub

#End Region

    Private mSource As IAuthorizeReadWrite

    ''' <summary>
    ''' This method is called when the data
    ''' object to which the control is bound
    ''' has changed.
    ''' </summary>
    Protected Overrides Sub DataObjectChanged()
      Refresh()
    End Sub

    ''' <summary>
    ''' Refresh authorization and update
    ''' all controls.
    ''' </summary>
    Public Sub Refresh()
      mSource = TryCast(DataObject, IAuthorizeReadWrite)
      If Not mSource Is Nothing Then
        MyBase.FindChildBindings()
      End If
    End Sub

    ''' <summary>
    ''' Check the read and write status
    ''' of the control based on the current
    ''' user's authorization.
    ''' </summary>
    ''' <param name="bnd">The Binding object.</param>
    ''' <param name="control">The control containing the binding.</param>
    ''' <param name="prop">The data bound DependencyProperty.</param>
    Protected Overrides Sub FoundBinding(ByVal bnd As Binding, ByVal control As FrameworkElement, ByVal prop As DependencyProperty)
      SetRead(bnd, CType(control, UIElement), mSource)
      SetWrite(bnd, CType(control, UIElement), mSource)
    End Sub

    Private Sub SetWrite(ByVal bnd As Binding, ByVal ctl As UIElement, ByVal source As IAuthorizeReadWrite)
      Dim canWrite As Boolean = source.CanWriteProperty(bnd.Path.Path)

      ' enable/disable writing of the value
      Dim propertyInfo As PropertyInfo = ctl.GetType().GetProperty("IsReadOnly", BindingFlags.FlattenHierarchy Or BindingFlags.Instance Or BindingFlags.Public)
      If Not propertyInfo Is Nothing Then
        propertyInfo.SetValue(ctl, (Not canWrite), New Object() {})
      Else
        ctl.IsEnabled = canWrite
      End If
    End Sub

    Private Sub SetRead(ByVal bnd As Binding, ByVal ctl As UIElement, ByVal source As IAuthorizeReadWrite)
      Dim canRead As Boolean = source.CanReadProperty(bnd.Path.Path)

      Dim visibilityMode As VisibilityMode = NotVisibleMode
      Dim controlVisibility As Object = ctl.GetValue(Authorizer.ControlVisibilityProperty)
      If controlVisibility IsNot Nothing Then
        visibilityMode = CType(controlVisibility, VisibilityMode)
      End If

      If canRead Then
        Select Case visibilityMode
          Case visibilityMode.Collapsed
            If ctl.Visibility = Visibility.Collapsed Then
              ctl.Visibility = Visibility.Visible
            End If
          Case visibilityMode.Hidden
            If ctl.Visibility = Visibility.Hidden Then
              ctl.Visibility = Visibility.Visible
            End If
          Case Else
            ' ignore
        End Select
      Else
        Select Case visibilityMode
          Case visibilityMode.Collapsed
            ctl.Visibility = Visibility.Collapsed
          Case visibilityMode.Hidden
            ctl.Visibility = Visibility.Hidden
          Case Else
            ' ignore
        End Select
      End If
    End Sub
  End Class

End Namespace
#End If