//-----------------------------------------------------------------------
// <copyright file="ISupportNavigation.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Interface that defines template that Navigatgor </summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Xaml
{
  /// <summary>
  /// Interface that defines template that Navigatgor 
  /// control can understand.  This interface should be 
  /// implemented by controls that would rely on Navigator
  /// object to show them.
  /// </summary>
  public interface ISupportNavigation
  {
    /// <summary>
    /// This method is called by Navigatgor in order
    /// to pass parameters from a bookmark into 
    /// the shown control
    /// </summary>
    /// <param name="parameters">
    /// Parameters passed as string.  Is is up to control
    /// to parse them
    /// </param>
    void SetParameters(string parameters);

    /// <summary>
    /// Get the title of the control
    /// </summary>
    string Title { get; }

    /// <summary>
    /// This event should be raised after the control is populated with data
    /// </summary>
    event EventHandler LoadCompleted;

    /// <summary>
    /// If set to false, bookamrk will be created as part of navigation.
    /// If set to true, bokmakr will be created when LoadCompleted
    /// event is raised by the control
    /// </summary>
    bool CreateBookmarkAfterLoadCompleted { get; }
  }
}