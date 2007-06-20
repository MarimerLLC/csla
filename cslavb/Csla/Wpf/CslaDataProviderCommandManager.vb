#If Not NET20 Then
Imports System.Windows.Input
Imports System.Windows

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

    Private mProvider As CslaDataProvider

    Private ReadOnly Property Provider() As CslaDataProvider
      Get
        Return mProvider
      End Get
    End Property

    Friend Sub New(ByVal provider As CslaDataProvider)

      mProvider = provider

    End Sub

    Shared Sub New()

      Dim binding As CommandBinding = New CommandBinding(ApplicationCommands.Save, AddressOf SaveCommand, AddressOf CanExecute)
      CommandManager.RegisterClassCommandBinding(GetType(CslaDataProviderCommandManager), binding)

      binding = New CommandBinding(ApplicationCommands.Undo, AddressOf UndoCommand, AddressOf CanExecute)
      CommandManager.RegisterClassCommandBinding(GetType(CslaDataProviderCommandManager), binding)

    End Sub

    Friend Shared Sub CanExecute(ByVal target As Object, ByVal e As CanExecuteRoutedEventArgs)

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

    Friend Shared Sub SaveCommand(ByVal target As Object, ByVal e As ExecutedRoutedEventArgs)

      Dim ctl As CslaDataProviderCommandManager = TryCast(target, CslaDataProviderCommandManager)
      If Not ctl Is Nothing AndAlso Not ctl.Provider Is Nothing Then
        ctl.Provider.Save()
      End If

    End Sub

    Friend Shared Sub UndoCommand(ByVal target As Object, ByVal e As ExecutedRoutedEventArgs)

      Dim ctl As CslaDataProviderCommandManager = TryCast(target, CslaDataProviderCommandManager)
      If Not ctl Is Nothing AndAlso Not ctl.Provider Is Nothing Then
        ctl.Provider.Cancel()
      End If

    End Sub

  End Class

End Namespace
#End If