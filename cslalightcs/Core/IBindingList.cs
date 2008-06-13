using System;

namespace Csla.Core
{
  public interface IBindingList
  {
    event EventHandler<ListChangedEventArgs> ListChanged;
  }
}
