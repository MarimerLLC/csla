using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthzMethod
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("This sample shows how to create Authz for your own methods on a business object.");
      try
      {
        var root = Root.NewEditableRoot();
        root.DoCalc();
      }
      catch (Exception ex)
      {
        Console.WriteLine("DoCalc: {0}", ex.Message);
      }

      Console.WriteLine("Press <enter> to continue.");
      Console.ReadLine();
    }
  }
}
