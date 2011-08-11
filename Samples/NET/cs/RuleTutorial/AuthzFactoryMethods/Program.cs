using System;

namespace AuthzFactoryMethods
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("AuthzFactoryMethods");
      Console.WriteLine("Root DataPortal will automatically check for authorization.");
      Console.WriteLine("Child dataportal does not check authorization.");
      try
      {
        Root.NewEditableRoot();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Root.NewEditableObject: \"{0}\"", ex.Message);
      }

      try
      {
        Root.GetEditableRoot(1);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Root.GetEditableRoot(): \"{0}\"", ex.Message);
      }

      try
      {
        Root.DeleteEditableRoot(1);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Root.DeleteEditableRoot: \"{0}\"", ex.Message);
      }
      
      try
      {
        var root = Root2.NewEditableRoot();
        root.Save(true);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Root2.Save(): \"{0}\"", ex.Message);
      }
 

      Console.WriteLine("Press <enter> to continue.");
      Console.ReadLine();
    }
  }
}
