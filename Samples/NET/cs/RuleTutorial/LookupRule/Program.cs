using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LookupRule
{
  class Program
  {
    static void Main(string[] args)
    {
      var root = Root.NewEditableRoot();
      root.ValidationComplete += (o, e) =>
                                   {
                                     var obj = (Root) o;
                                     
                                   };
      Console.WriteLine("About to set CustomerId, Name=\"{0}\"", root.Name);
      root.CustomerId = 1134;
      Console.WriteLine("CustomerId set, Name=\"{0}\"", root.Name);
      Console.WriteLine();
      Console.WriteLine("Press <ENTER> to continue.");
      Console.ReadLine();
    }
  }
}
