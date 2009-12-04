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
Imports Rolodex.Business.BusinessClasses

Namespace Rolodex
  Partial Public Class CompanyEditor
    Inherits UserControl
    Public Sub New()
      InitializeComponent()
    End Sub

    Private companyDataLoaded As Boolean = False

    Public Event DataLoaded As EventHandler
    Private Sub OnDataLoaded()
      If companyDataLoaded AndAlso DataLoadedEvent IsNot Nothing Then
        DataLoadedEvent.Invoke(Me, EventArgs.Empty)
      End If
    End Sub

    Public Event CloseRequested As EventHandler

    Public Sub LoadCompanyData(ByVal companyID As Integer)
      Dim provider As CslaDataProvider = TryCast(Me.Resources("CompanyData"), CslaDataProvider)
      provider.FactoryParameters.Add(companyID)
      provider.FactoryMethod = "GetCompany"
      provider.Refresh()
    End Sub

    Public Sub CreateNewCompanyData()
      Dim provider As CslaDataProvider = (CType(Me.Resources("CompanyData"), CslaDataProvider))
      provider.FactoryMethod = "CreateCompany"
      provider.FactoryParameters.Clear()
      provider.Refresh()
    End Sub

    Private Sub CslaDataProvider_DataChanged(ByVal sender As Object, ByVal e As EventArgs)
      If Not companyDataLoaded Then
        companyDataLoaded = True
        OnDataLoaded()
      End If

      Dim provider As CslaDataProvider = (CType(Me.Resources("CompanyData"), CslaDataProvider))
      Me.Contacts.SelectedItem = Nothing
      If provider.Data IsNot Nothing AndAlso (CType(provider.Data, Company)).Contacts.Count > 0 Then
        Me.Contacts.SelectedItem = (CType(provider.Data, Company)).Contacts(0)
        Me.Contacts.Focus()
      End If
      If provider.Error IsNot Nothing Then
        System.Windows.Browser.HtmlPage.Window.Alert((CType(sender, Csla.Silverlight.CslaDataProvider)).Error.Message)
      End If

    End Sub

    Private Sub CloseButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If CloseRequestedEvent IsNot Nothing Then
        CloseRequestedEvent.Invoke(Me, EventArgs.Empty)
      End If

    End Sub

    Private Sub AddContactButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If (CType(Me.Resources("CompanyData"), CslaDataProvider)).Data IsNot Nothing Then
        CType((CType(Me.Resources("CompanyData"), CslaDataProvider)).Data, Company).Contacts.AddNew()
      End If
    End Sub

    Private Sub DeleteContact_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      CType((CType(Me.Resources("CompanyData"), CslaDataProvider)).Data, Company).Contacts.Remove(CType((CType(sender, Button)).Tag, CompanyContact))
    End Sub

    Private Sub AddContactPhoneButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If Me.Contacts.SelectedItem IsNot Nothing Then
        Dim selectedContact As CompanyContact = TryCast(Me.Contacts.SelectedItem, CompanyContact)
        selectedContact.ContactPhones.AddNew()
      End If
    End Sub

    Private Sub DeleteContactPhone_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      If Me.Contacts.SelectedItem IsNot Nothing Then
        Dim selectedContact As CompanyContact = TryCast(Me.Contacts.SelectedItem, CompanyContact)
        selectedContact.ContactPhones.Remove(CType((CType(sender, Button)).Tag, CompanyContactPhone))
      End If
    End Sub

    Private Sub Contacts_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs)
      If Me.Contacts.SelectedItem Is Nothing Then
        Me.ContactsPhones.ItemsSource = Nothing
      Else
        Me.ContactsPhones.ItemsSource = (CType(Me.Contacts.SelectedItem, CompanyContact)).ContactPhones
      End If
    End Sub

    Private Sub NewButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      Dim provider As CslaDataProvider = (CType(Me.Resources("CompanyData"), CslaDataProvider))
      provider.FactoryMethod = "CreateCompany"
      provider.FactoryParameters.Clear()
      provider.Refresh()
    End Sub
  End Class
End Namespace
