using System;
using System.Resources;

namespace CSLA.Resources
{
  public class Strings
  {
    public static ResourceManager rm = 
      new ResourceManager("CSLA.Resources.Strings", System.Reflection.Assembly.GetExecutingAssembly());

    public static string GetResourceString(string name)
    {
      return rm.GetString(name);
    }
  }
}
