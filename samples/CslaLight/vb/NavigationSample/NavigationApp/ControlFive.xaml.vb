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
Imports System.ComponentModel

Namespace NavigationApp
  Partial Public Class ControlFive
    Inherits UserControl
    Implements ISupportNavigation

    Public Sub New()
      InitializeComponent()
    End Sub

    Private m_title As String = "Control Five"
    Public Const Bookmark As String = "ControlThree"

#Region "ISupportNavigation Members"

    Public ReadOnly Property CreateBookmarkAfterLoadCompleted() As Boolean Implements Csla.Silverlight.ISupportNavigation.CreateBookmarkAfterLoadCompleted
      Get
        Return True
      End Get
    End Property

    Public Event LoadCompleted As EventHandler Implements Csla.Silverlight.ISupportNavigation.LoadCompleted

    Public Sub SetParameters(ByVal parameters As String) Implements Csla.Silverlight.ISupportNavigation.SetParameters
      Me.ParametersBlock.Text = parameters
      Dim worder As New BackgroundWorker()
      AddHandler worder.RunWorkerCompleted, AddressOf worder_RunWorkerCompleted
      AddHandler worder.DoWork, AddressOf worder_DoWork
      worder.RunWorkerAsync()
    End Sub

    Private Sub worder_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
      'simulate long running process 
      'just waste some time 
      For i As Integer = 0 To 99999999
      Next
    End Sub

    Private Sub worder_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
      m_title = "Asynch Control"
      RaiseEvent LoadCompleted(Me, EventArgs.Empty)
    End Sub

    Public ReadOnly Property Title() As String Implements Csla.Silverlight.ISupportNavigation.Title
      Get
        Return m_title
      End Get
    End Property

#End Region

  End Class
End Namespace