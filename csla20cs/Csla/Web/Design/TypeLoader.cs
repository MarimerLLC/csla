using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;

namespace Csla.Web.Design
{
  /// <summary>
  /// Loads a Type object into the AppDomain
  /// from the specified assembly in the most
  /// current shadow directory used by VS 2005.
  /// </summary>
  public class TypeLoader : MarshalByRefObject
  {
    #region static methods for primary AppDomain

    /// <summary>
    /// Gets a list of
    /// <see cref="ObjectFieldInfo"/> describing
    /// the most recent version of the specified
    /// assembly and class.
    /// </summary>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="typeName">Name of the type</param>
    /// <returns></returns>
    public static IDataSourceFieldSchema[] GetFields(
      string assemblyName, string typeName)
    {
      List<ObjectFieldInfo> result =
        new List<ObjectFieldInfo>();

      string originalPath = GetOriginalPath(
        assemblyName, typeName);

      AppDomain tempDomain = GetTemporaryAppDomain();
      try
      {
        result = GetTypeLoader(tempDomain).GetFields(
          originalPath, assemblyName, typeName);
      }
      finally
      {
        AppDomain.Unload(tempDomain);
      }
      return result.ToArray();
    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// delete the object.
    /// </summary>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="typeName">Name of the type</param>
    public static bool CanDelete(
      string assemblyName, string typeName)
    {
      bool result;

      string originalPath = GetOriginalPath(
        assemblyName, typeName);

      AppDomain tempDomain = GetTemporaryAppDomain();
      try
      {
        result = GetTypeLoader(tempDomain).CanDelete(
          originalPath, assemblyName, typeName);
      }
      finally
      {
        AppDomain.Unload(tempDomain);
      }
      return result;

    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// insert an instance of the object.
    /// </summary>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="typeName">Name of the type</param>
    public static bool CanInsert(
      string assemblyName, string typeName)
    {
      bool result;

      string originalPath = GetOriginalPath(
        assemblyName, typeName);

      AppDomain tempDomain = GetTemporaryAppDomain();
      try
      {
        result = GetTypeLoader(tempDomain).CanInsert(
          originalPath, assemblyName, typeName);
      }
      finally
      {
        AppDomain.Unload(tempDomain);
      }
      return result;

    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// update or edit the object.
    /// </summary>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="typeName">Name of the type</param>
    public static bool CanUpdate(
      string assemblyName, string typeName)
    {
      bool result;

      string originalPath = GetOriginalPath(
        assemblyName, typeName);

      AppDomain tempDomain = GetTemporaryAppDomain();
      try
      {
        result = GetTypeLoader(tempDomain).CanUpdate(
          originalPath, assemblyName, typeName);
      }
      finally
      {
        AppDomain.Unload(tempDomain);
      }
      return result;

    }

    private static TypeLoader GetTypeLoader(AppDomain tempDomain)
    {
      // load the TypeLoader object in the temp AppDomain
      Assembly thisAssembly = Assembly.GetExecutingAssembly();
      TypeLoader loader =
        (TypeLoader)tempDomain.CreateInstanceFromAndUnwrap(
          thisAssembly.CodeBase, typeof(TypeLoader).FullName);
      return loader;
    }

    private static AppDomain GetTemporaryAppDomain()
    {
      System.Security.NamedPermissionSet fulltrust =
        new System.Security.NamedPermissionSet("FullTrust");
      AppDomain tempDomain = AppDomain.CreateDomain(
        "__CslaDataSource__temp",
        AppDomain.CurrentDomain.Evidence,
        AppDomain.CurrentDomain.SetupInformation,
        fulltrust,
        new System.Security.Policy.StrongName[] { });
      return tempDomain;
    }

    private static string GetOriginalPath(string assemblyName, string typeName)
    {
      Assembly asm = Assembly.Load(assemblyName);
      return asm.CodeBase;
    }

    #endregion

    #region Implementation for temporary AppDomain

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
    /// Get a value indicating whether data binding can directly
    /// delete the object.
    /// </summary>
    /// <param name="originalPath">Path to the assembly
    /// as determined by Visual Studio</param>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="typeName">Name of the type</param>
    public bool CanDelete(
      string originalPath, string assemblyName, string typeName)
    {
      Type objectType = GetType(originalPath, assemblyName, typeName);
      if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(objectType))
        return true;
      else if (objectType.GetMethod("Remove") != null)
        return true;
      else
        return false;
    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// insert an instance of the object.
    /// </summary>
    /// <param name="originalPath">Path to the assembly
    /// as determined by Visual Studio</param>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="typeName">Name of the type</param>
    public bool CanInsert(
      string originalPath, string assemblyName, string typeName)
    {
      Type objectType = GetType(originalPath, assemblyName, typeName);
      if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(objectType))
        return true;
      else
        return false;
    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// update or edit the object.
    /// </summary>
    /// <param name="originalPath">Path to the assembly
    /// as determined by Visual Studio</param>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="typeName">Name of the type</param>
    public bool CanUpdate(
      string originalPath, string assemblyName, string typeName)
    {
      Type objectType = GetType(originalPath, assemblyName, typeName);
      if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(objectType))
          return true;
        else
          return false;
    }

    /// <summary>
    /// Gets a <see cref="System.Type"/> object
    /// corresponding to the business type specified.
    /// </summary>
    private static Type GetType(
      string originalPath, string assemblyName, string typeName)
    {
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

    #endregion
  }
}
