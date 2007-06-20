#if !NET20
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;

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
      CommandBinding binding = new CommandBinding(ApplicationCommands.Save, SaveCommand, CanExecute);
      CommandManager.RegisterClassCommandBinding(typeof(CslaDataProviderCommandManager), binding);
      
      binding = new CommandBinding(ApplicationCommands.Undo, UndoCommand, CanExecute);
      CommandManager.RegisterClassCommandBinding(typeof(CslaDataProviderCommandManager), binding);
    }

    internal static void CanExecute(object target, CanExecuteRoutedEventArgs e)
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

    internal static void SaveCommand(object target, ExecutedRoutedEventArgs e)
    {
      CslaDataProviderCommandManager ctl = target as CslaDataProviderCommandManager;
      if (ctl != null && ctl.Provider != null)
        ctl.Provider.Save();
    }

    internal static void UndoCommand(object target, ExecutedRoutedEventArgs e)
    {
      CslaDataProviderCommandManager ctl = target as CslaDataProviderCommandManager;
      if (ctl != null && ctl.Provider != null)
        ctl.Provider.Cancel();
    }
  }
}
#endif