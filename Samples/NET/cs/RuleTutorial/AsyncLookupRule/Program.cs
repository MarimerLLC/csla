using System;

namespace AsyncLookupRule
{
  class Program
  {
    static void Main(string[] args)
    {
      var root = Root.NewEditableRoot();
      root.ValidationComplete += (o, e) =>
                                   {
                                     var obj = (Root) o;
                                     Console.WriteLine("Rules completed, Name=\"{0}\"", obj.Name);
                                   };
      Console.WriteLine("About to set CustomerId, Name=\"{0}\"", root.Name);
      root.CustomerId = 1134;
      Console.WriteLine();
      Console.WriteLine("Press <ENTER> to continue. (Wait while async rule runs)");
      Console.ReadLine();
    }
  }
}
