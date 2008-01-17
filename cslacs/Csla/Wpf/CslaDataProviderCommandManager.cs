#if !NET20
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace Csla.Wpf
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
    private CslaDataProvider _provider;

    private CslaDataProvider Provider
    {
      get { return _provider; }
    }

    internal CslaDataProviderCommandManager(CslaDataProvider provider)
    {
      _provider = provider;
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
    }

    private static void CanExecuteSave(object target, CanExecuteRoutedEventArgs e)
    {
      bool result = false;
      CslaDataProviderCommandManager ctl = target as CslaDataProviderCommandManager;
      if (ctl != null && ctl.Provider != null)
      {
        Csla.Core.IEditableBusinessObject ibiz = ctl.Provider.Data as Csla.Core.IEditableBusinessObject;
        if (ibiz != null)
          result = ibiz.IsSavable;
        else
        {
          Csla.Core.IEditableCollection icol = ctl.Provider.Data as Csla.Core.IEditableCollection;
          if (icol != null)
            result = icol.IsSavable;
        }
      }
      e.CanExecute = result;
    }

    private static void SaveCommand(object target, ExecutedRoutedEventArgs e)
    {
      CslaDataProviderCommandManager ctl = target as CslaDataProviderCommandManager;
      if (ctl != null && ctl.Provider != null)
        ctl.Provider.Save();
    }

    private static void CanExecuteUndo(object target, CanExecuteRoutedEventArgs e)
    {
      bool result = false;
      CslaDataProviderCommandManager ctl = target as CslaDataProviderCommandManager;
      if (ctl != null && ctl.Provider != null)
      {
        Csla.Core.IEditableBusinessObject ibiz = ctl.Provider.Data as Csla.Core.IEditableBusinessObject;
        if (ibiz != null)
          result = ibiz.IsDirty;
        else
        {
          Csla.Core.IEditableCollection icol = ctl.Provider.Data as Csla.Core.IEditableCollection;
          if (icol != null)
            result = icol.IsDirty;
        }
      }
      e.CanExecute = result;
    }

    private static void UndoCommand(object target, ExecutedRoutedEventArgs e)
    {
      CslaDataProviderCommandManager ctl = target as CslaDataProviderCommandManager;
      if (ctl != null && ctl.Provider != null)
        ctl.Provider.Cancel();
    }

    private static void CanExecuteNew(object target, CanExecuteRoutedEventArgs e)
    {
      bool result = false;
      CslaDataProviderCommandManager ctl = target as CslaDataProviderCommandManager;
      if (ctl != null && ctl.Provider != null)
      {
        IBindingList list = ctl.Provider.Data as IBindingList;
        if (list != null)
        {
          result = list.AllowNew;
          if (result && !Csla.Security.AuthorizationRules.CanEditObject(ctl.Provider.Data.GetType()))
            result = false;
        }
      }
      e.CanExecute = result;
    }

    private static void NewCommand(object target, ExecutedRoutedEventArgs e)
    {
      CslaDataProviderCommandManager ctl = target as CslaDataProviderCommandManager;
      if (ctl != null && ctl.Provider != null)
        ctl.Provider.AddNew();
    }
  }
}
#endif