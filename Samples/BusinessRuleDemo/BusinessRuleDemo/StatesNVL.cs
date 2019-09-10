using System;
using System.Linq;
using System.Threading.Tasks;
using Csla;

namespace BusinessRuleDemo
{
  [Serializable]
  public class StatesNVL : NameValueListBase<string, string>
  {
    [Fetch]
    private void Fetch()
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;

      var data = System.IO.File.ReadAllLines("AmericanStates.txt");
      foreach (var x in data.Select(s => s.Split(',')))
      {
        this.Add(new NameValuePair(x[0].Trim(), x[1].Trim()));
      }

      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }
  }
}
