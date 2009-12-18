using System;
using System.ComponentModel;
using Csla.Serialization.Mobile;
using System.Runtime.Serialization;
using Csla.Serialization;

namespace Csla.Core
{
  /// <summary>
  /// Extends BindingList of T by adding extra
  /// behaviors.
  /// </summary>
  /// <typeparam name="T">Type of item contained in list.</typeparam>
  [Serializable]
  public class ExtendedObservableCollection<T> : MobileObservableCollection<T>
  {
  }
}
