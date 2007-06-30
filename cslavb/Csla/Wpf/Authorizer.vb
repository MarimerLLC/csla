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
      DependencyProperty.RegisterAttached("NotVisibleMode", _
                                          GetType(VisibilityMode), _
                                          GetType(Authorizer), _
                                          New FrameworkPropertyMetadata(VisibilityMode.Collapsed), _
                                          AddressOf IsValidVisibilityMode)

    ' Define method to validate the value
    Private Shared Function IsValidVisibilityMode(ByVal o As Object) As Boolean
      Return (TypeOf o Is VisibilityMode)
    End Function

    ''' <summary>
    ''' Gets the value controlling how controls
    ''' bound to non-readable properties will be rendered.
    ''' </summary>
    Public Shared Function GetNotVisibleMode(ByVal obj As DependencyObject) As VisibilityMode
      Return CType(obj.GetValue(NotVisibleModeProperty), VisibilityMode)
    End Function

    ''' <summary>
    ''' Sets the value controlling how controls
    ''' bound to non-readable properties will be rendered.
    ''' </summary>
    Public Shared Sub SetNotVisibleMode(ByVal obj As DependencyObject, ByVal mode As VisibilityMode)
      obj.SetValue(NotVisibleModeProperty, mode)
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
      Dim visibilityMode As VisibilityMode = GetNotVisibleMode(ctl)

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