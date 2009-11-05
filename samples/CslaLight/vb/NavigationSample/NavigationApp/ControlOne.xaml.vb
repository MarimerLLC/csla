Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports Csla.Silverlight

Namespace NavigationApp
  Partial Public Class ControlOne
	  Inherits UserControl
    Implements ISupportNavigation

    Public Sub New()
      InitializeComponent()
    End Sub

    Public Const Bookmark As String = "ControlOne"


#Region "ISupportsNavigation Members"

    Public Sub SetParameters(ByVal parameters As String) Implements ISupportNavigation.SetParameters
      Me.ParametersBlock.Text = parameters
    End Sub

    Public ReadOnly Property Title() As String Implements ISupportNavigation.Title
      Get
        Return "Control One"
      End Get
    End Property

#End Region

    Public ReadOnly Property CreateBookmarkAfterLoadCompleted() As Boolean Implements Csla.Silverlight.ISupportNavigation.CreateBookmarkAfterLoadCompleted
      Get
        Return False
      End Get
    End Property

    Public Event LoadCompleted(ByVal sender As Object, ByVal e As System.EventArgs) Implements Csla.Silverlight.ISupportNavigation.LoadCompleted
  End Class
End Namespace
