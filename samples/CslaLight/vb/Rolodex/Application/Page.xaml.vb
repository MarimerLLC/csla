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
Imports System.Windows.Interop
Imports Csla.Silverlight

Namespace Rolodex
  Partial Public Class Page
    Inherits UserControl

    Private _editor As CompanyEditor
    Public Sub New()
      InitializeComponent()
    End Sub

    Private Sub Logger_LoginSuccessfull(ByVal sender As Object, ByVal e As EventArgs)
      Dim list As New CompaniesList()
      AddHandler list.CompanySelected, AddressOf list_CompanySelected
      AddHandler list.ShowRanksRequested, AddressOf list_ShowRanksRequested
      AddHandler list.NewCompanyRequested, AddressOf list_NewCompanyRequested
      ShowControl(list)
    End Sub

    Private Sub RemoveEditHandlers()
      If _editor IsNot Nothing Then
        RemoveHandler _editor.CloseRequested, AddressOf editor_CloseRequested
        RemoveHandler _editor.DataLoaded, AddressOf HandleDataLoaded
      End If
    End Sub

    Private Sub list_NewCompanyRequested(ByVal sender As Object, ByVal e As EventArgs)
      RemoveEditHandlers()
      _editor = Nothing
      _editor = New CompanyEditor()
      AddHandler _editor.CloseRequested, AddressOf editor_CloseRequested
      AddHandler _editor.DataLoaded, AddressOf HandleDataLoaded
      _editor.CreateNewCompanyData()
    End Sub

    Private Sub HandleDataLoaded(ByVal sender As Object, ByVal e As EventArgs)
      ShowControl(_editor)
    End Sub

    Private Sub list_ShowRanksRequested(ByVal sender As Object, ByVal e As EventArgs)
      Dim editor As New RanksEditor()
      AddHandler editor.CloseRequested, AddressOf ranksEditor_CloseRequested
      ShowControl(editor)
    End Sub

    Private Sub ranksEditor_CloseRequested(ByVal sender As Object, ByVal e As EventArgs)
      Logger_LoginSuccessfull(Me, EventArgs.Empty)
    End Sub

    Private Sub list_CompanySelected(ByVal sender As Object, ByVal e As CompanySelectedEventArgs)
      RemoveEditHandlers()
      _editor = Nothing
      _editor = New CompanyEditor()
      AddHandler _editor.CloseRequested, AddressOf editor_CloseRequested
      AddHandler _editor.DataLoaded, AddressOf HandleDataLoaded
      _editor.LoadCompanyData(e.CompanyID)
    End Sub

    Private Sub editor_CloseRequested(ByVal sender As Object, ByVal e As EventArgs)
      Logger_LoginSuccessfull(sender, e)
    End Sub

    Private Sub ShowControl(ByVal control As UserControl)
      If Me.LayoutRoot.Children.Count > 0 Then
        Me.LayoutRoot.Children.Remove(Me.LayoutRoot.Children(0))
      End If
      Me.LayoutRoot.Children.Add(control)
    End Sub

    Private Sub UserControl_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
      AddHandler System.Windows.Application.Current.Host.Content.Resized, AddressOf Content_Resized
    End Sub

    Private Sub Content_Resized(ByVal sender As Object, ByVal e As EventArgs)
      Dim scaleY As Double = (System.Windows.Application.Current.Host.Content.ActualHeight / Me.Height)
      Dim scaleX As Double = (System.Windows.Application.Current.Host.Content.ActualWidth / Me.Width)
      Me.scaleTransform.ScaleX = scaleX
      Me.scaleTransform.ScaleY = scaleY
    End Sub
  End Class
End Namespace
