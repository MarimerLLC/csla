'INSTANT C# NOTE: Formerly VB.NET project-level imports:

Imports Microsoft.VisualBasic
Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.Xml
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Imports Csla.Silverlight
Imports System.Collections.ObjectModel
Imports Sample.Business

Namespace Sample
  Partial Public Class Page
    Inherits UserControl


    Public Sub New()
      AddHandler Loaded, AddressOf Page_Loaded
      InitializeComponent()
    End Sub

    Private Sub CslaDataProvider_DataChanged(ByVal sender As Object, ByVal e As System.EventArgs)
      Dim provider As CslaDataProvider = CType(Me.Resources("CompaniesData"), CslaDataProvider)
      If provider.Error IsNot Nothing Then
        System.Windows.Browser.HtmlPage.Window.Alert(provider.Error.Message)
      End If
    End Sub

    Private Sub Page_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
      Dim provider As CslaDataProvider = CType(Me.Resources("CompaniesData"), CslaDataProvider)
      AddHandler provider.DataChanged, AddressOf CslaDataProvider_DataChanged
      SamplePrincipal.Login("admin", "admin", Function(o1, e1) ShowData())
    End Sub

    Private Function ShowData() As Boolean
      Dim provider As CslaDataProvider = CType(Me.Resources("CompaniesData"), CslaDataProvider)
      provider.Refresh()
      Return True
    End Function

  End Class
End Namespace 'end of root namespace