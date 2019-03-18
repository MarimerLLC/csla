using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Xaml;

namespace Csla.Test.ViewModelTests
{
  public class TestViewModel<T> : ViewModel<T>
  {
    public void Save() => DoSave();
  }
}
