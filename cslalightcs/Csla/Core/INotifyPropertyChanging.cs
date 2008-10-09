using System;

namespace Csla.Core
{
  public interface INotifyPropertyChanging
  {
    event PropertyChangingEventHandler PropertyChanging;
  }
}
