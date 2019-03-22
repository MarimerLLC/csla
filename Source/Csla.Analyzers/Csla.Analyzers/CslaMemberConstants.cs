namespace Csla.Analyzers
{
  public static class CslaMemberConstants
  {
    public static class Properties
    {
      public const string GetProperty = "GetProperty";
      public const string GetPropertyConvert = "GetPropertyConvert";
      public const string ReadProperty = "ReadProperty";
      public const string ReadPropertyConvert = "ReadPropertyConvert";
      public const string LazyGetProperty = "LazyGetProperty";
      public const string LazyGetPropertyAsync = "LazyGetPropertyAsync";
      public const string LazyReadProperty = "LazyReadProperty";
      public const string LazyReadPropertyAsync = "LazyReadPropertyAsync";
      public const string LoadProperty = "LoadProperty";
      public const string LoadPropertyAsync = "LoadPropertyAsync";
      public const string LoadPropertyConvert = "LoadPropertyConvert";
      public const string LoadPropertyMarkDirty = "LoadPropertyMarkDirty";
      public const string SetProperty = "SetProperty";
      public const string SetPropertyConvert = "SetPropertyConvert";
    }

    public static class Operations
    {
      public const string DataPortalCreate = "DataPortal_Create";
      public const string DataPortalFetch = "DataPortal_Fetch";
      public const string DataPortalInsert = "DataPortal_Insert";
      public const string DataPortalUpdate = "DataPortal_Update";
      public const string DataPortalDelete = "DataPortal_Delete";
      public const string DataPortalDeleteSelf = "DataPortal_DeleteSelf";
      public const string DataPortalExecute = "DataPortal_Execute";
      public const string ChildCreate = "Child_Create";
      public const string ChildFetch = "Child_Fetch";
      public const string ChildInsert = "Child_Insert";
      public const string ChildUpdate = "Child_Update";
      public const string ChildDeleteSelf = "Child_DeleteSelf";
    }

    public static class Types
    {
      public const string BusinessBase = "BusinessBase";
      public const string BusinessBindingListBase = "BusinessBindingListBase";
      public const string BusinessListBase = "BusinessListBase";
      public const string DynamicListBase = "DynamicListBase";
      public const string IBusinessObject = "IBusinessObject";
      public const string IMobileObject = "IMobileObject";
      public const string IPropertyInfo = "IPropertyInfo";
      public const string ManagedObjectBase = "ManagedObjectBase";
      public const string ReadOnlyBase = "ReadOnlyBase";
    }

    public const string AssemblyName = "Csla";
    public const string SerializableAttribute = "SerializableAttribute";
  }
}