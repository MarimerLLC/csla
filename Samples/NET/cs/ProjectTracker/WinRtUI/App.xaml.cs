﻿using WinRtUI.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace WinRtUI
{
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  sealed partial class App : Application
  {
    /// <summary>
    /// Initializes the singleton Application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
      this.InitializeComponent();
      this.Suspending += OnSuspending;
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used when the application is launched to open a specific file, to display
    /// search results, and so forth.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
      // Do not repeat app initialization when already running, just ensure that
      // the window is active
      if (args.PreviousExecutionState == ApplicationExecutionState.Running)
      {
        Window.Current.Activate();
        return;
      }

      Csla.DataPortal.ProxyTypeName = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
      Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://localhost:22627/SlPortal.svc";

      ProjectTracker.Library.Security.PTPrincipal.BeginLogin("manager", "manager");

      // Create a Frame to act as the navigation context and associate it with
      // a SuspensionManager key
      var rootFrame = new Frame();
      SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

      if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
      {
        // Restore the saved session state only when appropriate
        await SuspensionManager.RestoreAsync();
      }

      if (rootFrame.Content == null)
      {
        // When the navigation stack isn't restored navigate to the first page,
        // configuring the new page by passing required information as a navigation
        // parameter
        if (!rootFrame.Navigate(typeof(HomePage), "AllGroups"))
        {
          throw new Exception("Failed to create initial page");
        }
      }

      // Place the frame in the current Window and ensure that it is active
      Window.Current.Content = rootFrame;
      Window.Current.Activate();
    }

    /// <summary>
    /// Invoked when application execution is being suspended.  Application state is saved
    /// without knowing whether the application will be terminated or resumed with the contents
    /// of memory still intact.
    /// </summary>
    /// <param name="sender">The source of the suspend request.</param>
    /// <param name="e">Details about the suspend request.</param>
    private async void OnSuspending(object sender, SuspendingEventArgs e)
    {
      var deferral = e.SuspendingOperation.GetDeferral();
      await SuspensionManager.SaveAsync();
      deferral.Complete();
    }
  }
}
