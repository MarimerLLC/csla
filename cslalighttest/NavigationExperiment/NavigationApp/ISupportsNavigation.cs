using System;

namespace NavigationApp
{
  public interface ISupportsNavigation
  {
    void SetParameters(string parameters);
    string Title { get; }
  }
}
