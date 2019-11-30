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
      public const string ChildExecute = "Child_Execute";
    }

    public static class OperationAttributes
    {
      public const string Create = nameof(Create);
      public const string Fetch = nameof(Fetch);
      public const string Insert = nameof(Insert);
      public const string Update = nameof(Update);
      public const string Delete = nameof(Delete);
      public const string DeleteSelf = nameof(DeleteSelf);
      public const string Execute = nameof(Execute);
      public const string CreateChild = nameof(CreateChild);
      public const string FetchChild = nameof(FetchChild);
      public const string InsertChild = nameof(InsertChild);
      public const string UpdateChild = nameof(UpdateChild);
      public const string DeleteSelfChild = nameof(DeleteSelfChild);
      public const string ExecuteChild = nameof(ExecuteChild);
    }

    public static class Types
    {
      public const string BusinessBase = nameof(BusinessBase);
      public const string BusinessBindingListBase = nameof(BusinessBindingListBase);
      public const string BusinessListBase = nameof(BusinessListBase);
      public const string IBusinessRule = nameof(IBusinessRule);
      public const string IBusinessRuleAsync = nameof(IBusinessRuleAsync);
      public const string DataPortalChildOperationAttribute = nameof(DataPortalChildOperationAttribute);
      public const string DataPortalOperationAttribute = nameof(DataPortalOperationAttribute);
      public const string DataPortalRootOperationAttribute = nameof(DataPortalRootOperationAttribute);
      public const string DynamicListBase = nameof(DynamicListBase);
      public const string IBusinessObject = nameof(IBusinessObject);
      public const string IMobileObject = nameof(IMobileObject);
      public const string InjectAttribute = nameof(InjectAttribute);
      public const string IPropertyInfo = nameof(IPropertyInfo);
      public const string ManagedObjectBase = nameof(ManagedObjectBase);
      public const string ObjectFactory = nameof(ObjectFactory);
      public const string ReadOnlyBase = nameof(ReadOnlyBase);
      public const string RunLocalAttribute = nameof(RunLocalAttribute);
    }

    public const string AssemblyName = "Csla";
    public const string SerializableAttribute = "SerializableAttribute";
  }
}