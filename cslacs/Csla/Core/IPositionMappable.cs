using System;

namespace Csla.Core
{
  interface IPositionMappable<T>
  {
    int PositionOf(T item);
  }
}
