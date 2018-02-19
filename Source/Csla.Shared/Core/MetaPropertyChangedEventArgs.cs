using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Csla.Core
{

  /// <summary>
  /// Used to distinguish Meta Properties 
  /// </summary>
  public class MetaPropertyChangedEventArgs : PropertyChangedEventArgs
  {
    public MetaPropertyChangedEventArgs(string propertyName) : base(propertyName)
    {
    }
  }
}
