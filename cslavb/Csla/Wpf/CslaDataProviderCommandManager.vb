Imports System.Windows.Input
Imports System.Windows
Imports System.ComponentModel

Namespace Wpf

  ''' <summary>
  ''' Implements support for RoutedCommands that can
  ''' be executed by the CslaDataProvider control.
  ''' </summary>
  ''' <remarks>
  ''' Use this object as the CommandTarget for command
  ''' source objects when you want the CslaDataProvider
  ''' to execute the command.
  ''' </remarks>
  Public Class CslaDataProviderCommandManager
    Inherits System.Windows.UIElement

    Private _provider As CslaDataProvider

    Private ReadOnly Property Provider() As CslaDataProvider
      Get
        Return _provider
      End Get
    End Property

    Friend Sub New(ByVal provider As CslaDataProvider)

      _provider = provider

    End Sub

    Shared Sub New()

      Dim binding As CommandBinding

      binding = New CommandBinding(ApplicationCommands.Save, AddressOf SaveCommand, AddressOf CanExecuteSave)
      CommandManager.RegisterClassCommandBinding(GetType(CslaDataProviderCommandManager), binding)

      binding = New CommandBinding(ApplicationCommands.Undo, AddressOf UndoCommand, AddressOf CanExecuteUndo)
      CommandManager.RegisterClassCommandBinding(GetType(CslaDataProviderCommandManager), binding)

      binding = New CommandBinding(ApplicationCommands.[New], AddressOf NewCommand, AddressOf CanExecuteNew)
      CommandManager.RegisterClassCommandBinding(GetType(CslaDataProviderCommandManager), binding)

    End Sub

    Private Shared Sub CanExecuteSave(ByVal target As Object, ByVal e As CanExecuteRoutedEventArgs)

      Dim result As Boolean = False
      Dim ctl As CslaDataProviderCommandManager = TryCast(target, CslaDataProviderCommandManager)
      If Not ctl Is Nothing AndAlso Not ctl.Provider Is Nothing Then
        Dim ibiz As Csla.Core.IEditableBusinessObject = TryCast(ctl.Provider.Data, Csla.Core.IEditableBusinessObject)
        If Not ibiz Is Nothing Then
          result = ibiz.IsSavable
        Else
          Dim icol As Csla.Core.IEditableCollection = TryCast(ctl.Provider.Data, Csla.Core.IEditableCollection)
          If Not icol Is Nothing Then
            result = icol.IsSavable
          End If
        End If
      End If
      e.CanExecute = result

    End Sub

    Private Shared Sub SaveCommand(ByVal target As Object, ByVal e As ExecutedRoutedEventArgs)

      Dim ctl As CslaDataProviderCommandManager = TryCast(target, CslaDataProviderCommandManager)
      If Not ctl Is Nothing AndAlso Not ctl.Provider Is Nothing Then
        ctl.Provider.Save()
      End If

    End Sub

    Private Shared Sub CanExecuteUndo(ByVal target As Object, ByVal e As CanExecuteRoutedEventArgs)

      Dim result As Boolean = False
      Dim ctl As CslaDataProviderCommandManager = TryCast(target, CslaDataProviderCommandManager)
      If Not ctl Is Nothing AndAlso Not ctl.Provider Is Nothing Then
        Dim ibiz As Csla.Core.IEditableBusinessObject = TryCast(ctl.Provider.Data, Csla.Core.IEditableBusinessObject)
        If Not ibiz Is Nothing Then
          result = ibiz.IsDirty
        Else
          Dim icol As Csla.Core.IEditableCollection = TryCast(ctl.Provider.Data, Csla.Core.IEditableCollection)
          If Not icol Is Nothing Then
            result = icol.IsDirty
          End If
        End If
      End If
      e.CanExecute = result

    End Sub

    Private Shared Sub UndoCommand(ByVal target As Object, ByVal e As ExecutedRoutedEventArgs)

      Dim ctl As CslaDataProviderCommandManager = TryCast(target, CslaDataProviderCommandManager)
      If Not ctl Is Nothing AndAlso Not ctl.Provider Is Nothing Then
        ctl.Provider.Cancel()
      End If

    End Sub

    Private Shared Sub CanExecuteNew(ByVal target As Object, ByVal e As CanExecuteRoutedEventArgs)

      Dim result As Boolean = False
      Dim ctl As CslaDataProviderCommandManager = TryCast(target, CslaDataProviderCommandManager)
      If Not ctl Is Nothing AndAlso Not ctl.Provider Is Nothing Then
        Dim list As IBindingList = TryCast(ctl.Provider.Data, IBindingList)
        If list IsNot Nothing Then
          result = list.AllowNew
          If result AndAlso Not Csla.Security.AuthorizationRules.CanEditObject(ctl.Provider.Data.GetType) Then
            result = False
          End If
        End If
      End If
      e.CanExecute = result

    End Sub

    Private Shared Sub NewCommand(ByVal target As Object, ByVal e As ExecutedRoutedEventArgs)

      Dim ctl As CslaDataProviderCommandManager = TryCast(target, CslaDataProviderCommandManager)
      If Not ctl Is Nothing AndAlso Not ctl.Provider Is Nothing Then
        ctl.Provider.AddNew()
      End If

    End Sub

  End Class

End Namespace