using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace Csla.Web.Design
{
  /// <summary>
  /// Loads a Type object into the AppDomain
  /// from the specified assembly in the most
  /// current shadow directory used by VS 2005.
  /// </summary>
  public class TypeLoader : MarshalByRefObject
  {
    /// <summary>
    /// Gets a list of
    /// <see cref="ObjectFieldInfo"/> describing
    /// the most recent version of the specified
    /// assembly and class.
    /// </summary>
    /// <param name="originalPath">Path to the assembly
    /// as determined by Visual Studio</param>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="typeName">Name of the type</param>
    /// <returns></returns>
    public List<ObjectFieldInfo> GetFields(
      string originalPath, string assemblyName, string typeName)
    {
      List<ObjectFieldInfo> result =
        new List<ObjectFieldInfo>();

      Type t = GetType(originalPath, assemblyName, typeName);
      if (typeof(IEnumerable).IsAssignableFrom(t))
      {
        // this is a list so get the item type
        t = Utilities.GetChildItemType(t);
      }
      PropertyDescriptorCollection props =
        TypeDescriptor.GetProperties(t);
      foreach (PropertyDescriptor item in props)
        if (item.IsBrowsable)
          result.Add(new ObjectFieldInfo(item));

      return result;
    }

    /// <summary>
    /// Gets a <see cref="System.Type"/> object
    /// corresponding to the business type specified.
    /// </summary>
    private static Type GetType(
      string originalPath, string assemblyName, string typeName)
    {
      //Assembly thisAssembly = Assembly.GetExecutingAssembly();
      //string assemblyPath = GetCodeBase(thisAssembly.CodeBase);
      string assemblyPath = GetCodeBase(originalPath);

      Assembly asm = Assembly.LoadFrom(assemblyPath + assemblyName + ".dll");
      Type result = asm.GetType(typeName, true, true);
      return result;
    }

    /// <summary>
    /// Determines the most recent shadow directory
    /// path used by VS 2005 to store the project's
    /// assemblies.
    /// </summary>
    /// <param name="cslaPath">
    /// Path to the Csla.dll from which the CslaDataSource
    /// control has been loaded (typically not the latest
    /// shadow directory).
    /// </param>
    /// <returns>
    /// Directory path for the shadow directory,
    /// ending in a \ character.
    /// </returns>
    private static string GetCodeBase(string cslaPath)
    {
      if (cslaPath.StartsWith(@"file:///"))
      {
        cslaPath = cslaPath.Substring(8);
        cslaPath = cslaPath.Replace(@"/", @"\");
      }
      int count = 0;
      int end = 1;
      for (int pos = cslaPath.Length - 1; pos > 0; pos--)
        if (cslaPath.Substring(pos, 1) == @"\")
        {
          count++;
          if (count == 2)
          {
            end = pos;
            break;
          }
        }
      string codeBase = cslaPath.Substring(0, end);

      DirectoryInfo baseDir = new DirectoryInfo(codeBase);
      DirectoryInfo result = null;
      DateTime maxDate = DateTime.MinValue;
      foreach (DirectoryInfo dir in baseDir.GetDirectories())
        if (dir.LastWriteTime > maxDate)
        {
          maxDate = dir.LastWriteTime;
          result = dir;
        }

      if (result != null)
        return result.FullName + @"\";
      else
        return null;
    }
  }
}
