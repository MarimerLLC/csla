#if !XAMARIN && !WINDOWS_UWP && !MAUI
//-----------------------------------------------------------------------
// <copyright file="CslaDataProviderCommandManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements support for RoutedCommands that can</summary>
//-----------------------------------------------------------------------

using System.Windows.Input;
using Csla.Core;

namespace Csla.Xaml
{
  /// <summary>
  /// Implements support for RoutedCommands that can
  /// be executed by the CslaDataProvider control.
  /// </summary>
  /// <remarks>
  /// Use this object as the CommandTarget for command
  /// source objects when you want the CslaDataProvider
  /// to execute the command.
  /// </remarks>
  public class CslaDataProviderCommandManager : System.Windows.UIElement
  {
    private CslaDataProvider Provider { get; }

    internal CslaDataProviderCommandManager(CslaDataProvider provider)
    {
      Provider = provider;
    }

    static CslaDataProviderCommandManager()
    {
      CommandBinding binding;
      
      binding = new CommandBinding(ApplicationCommands.Save, SaveCommand, CanExecuteSave);
      CommandManager.RegisterClassCommandBinding(typeof(CslaDataProviderCommandManager), binding);
      
      binding = new CommandBinding(ApplicationCommands.Undo, UndoCommand, CanExecuteUndo);
      CommandManager.RegisterClassCommandBinding(typeof(CslaDataProviderCommandManager), binding);

      binding = new CommandBinding(ApplicationCommands.New, NewCommand, CanExecuteNew);
      CommandManager.RegisterClassCommandBinding(typeof(CslaDataProviderCommandManager), binding);

      binding = new CommandBinding(ApplicationCommands.Delete, RemoveCommand, CanExecuteRemove);
      CommandManager.RegisterClassCommandBinding(typeof(CslaDataProviderCommandManager), binding);
    }

    private static void CanExecuteSave(object target, CanExecuteRoutedEventArgs e)
    {
      bool result = false;
      if (target is CslaDataProviderCommandManager ctl && ctl.Provider != null)
      {
        if (ctl.Provider.Data is IEditableBusinessObject ibiz)
          result = ibiz.IsSavable;
        else
        {
          if (ctl.Provider.Data is IEditableCollection icol)
            result = icol.IsSavable;
        }
      }
      e.CanExecute = result;
    }

    private static void SaveCommand(object target, ExecutedRoutedEventArgs e)
    {
      if (target is CslaDataProviderCommandManager ctl && ctl.Provider != null)
        ctl.Provider.Save();
    }

    private static void CanExecuteUndo(object target, CanExecuteRoutedEventArgs e)
    {
      bool result = false;
      if (target is CslaDataProviderCommandManager ctl && ctl.Provider != null)
      {
        if (ctl.Provider.Data != null)
        {
          if (ctl.Provider.Data is IEditableBusinessObject ibiz)
            result = ibiz.IsDirty;
          else
          {
            if (ctl.Provider.Data is IEditableCollection icol)
              result = icol.IsDirty;
          }
        }
      }
      e.CanExecute = result;
    }

    private static void UndoCommand(object target, ExecutedRoutedEventArgs e)
    {
      if (target is CslaDataProviderCommandManager ctl && ctl.Provider != null)
        ctl.Provider.Cancel();
    }

    private static void CanExecuteNew(object target, CanExecuteRoutedEventArgs e)
    {
      bool result = false;
      if (target is CslaDataProviderCommandManager ctl && ctl.Provider != null)
      {
        if (ctl.Provider.Data is IBindingList list)
        {
          result = list.AllowNew;
          if (result && !Rules.BusinessRules.HasPermission(ApplicationContextManager.GetApplicationContext(), Rules.AuthorizationActions.EditObject, ctl.Provider.Data))
            result = false;
        }
      }
      e.CanExecute = result;
    }

    private static void NewCommand(object target, ExecutedRoutedEventArgs e)
    {
      if (target is CslaDataProviderCommandManager ctl && ctl.Provider != null)
        ctl.Provider.AddNew();
    }

    private static void CanExecuteRemove(object target, CanExecuteRoutedEventArgs e)
    {
      bool result = false;
      if (target is CslaDataProviderCommandManager ctl && ctl.Provider != null)
      {
        if (ctl.Provider.Data != null)
        {
          IBindingList list;
          if (e.Parameter is BusinessBase bb)
            list = bb.Parent as IBindingList;
          else
            list = ctl.Provider.Data as IBindingList;
          if (list != null)
          {
            result = list.AllowRemove;
            if (result && !Rules.BusinessRules.HasPermission(ApplicationContextManager.GetApplicationContext(), Rules.AuthorizationActions.EditObject, ctl.Provider.Data))
              result = false;
          }
        }
      }
      e.CanExecute = result;
    }

    private static void RemoveCommand(object target, ExecutedRoutedEventArgs e)
    {
      if (target is CslaDataProviderCommandManager ctl && ctl.Provider != null)
        ctl.Provider.RemoveItem(null, new ExecuteEventArgs { MethodParameter = e.Parameter });
    }
  }
}
#endif