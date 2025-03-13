namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class CslaMemberConstants
  {
    public static class Properties
    {
      /// <summary>
      /// 
      /// </summary>
      public const string GetProperty = "GetProperty";
      /// <summary>
      /// 
      /// </summary>
      public const string GetPropertyConvert = "GetPropertyConvert";
      /// <summary>
      /// 
      /// </summary>
      public const string ReadProperty = "ReadProperty";
      /// <summary>
      /// 
      /// </summary>
      public const string ReadPropertyConvert = "ReadPropertyConvert";
      /// <summary>
      /// 
      /// </summary>
      public const string LazyGetProperty = "LazyGetProperty";
      /// <summary>
      /// 
      /// </summary>
      public const string LazyGetPropertyAsync = "LazyGetPropertyAsync";
      /// <summary>
      /// 
      /// </summary>
      public const string LazyReadProperty = "LazyReadProperty";
      /// <summary>
      /// 
      /// </summary>
      public const string LazyReadPropertyAsync = "LazyReadPropertyAsync";
      /// <summary>
      /// 
      /// </summary>
      public const string LoadProperty = "LoadProperty";
      /// <summary>
      /// 
      /// </summary>
      public const string LoadPropertyAsync = "LoadPropertyAsync";
      /// <summary>
      /// 
      /// </summary>
      public const string LoadPropertyConvert = "LoadPropertyConvert";
      /// <summary>
      /// 
      /// </summary>
      public const string LoadPropertyMarkDirty = "LoadPropertyMarkDirty";
      /// <summary>
      /// 
      /// </summary>
      public const string SetProperty = "SetProperty";
      /// <summary>
      /// 
      /// </summary>
      public const string SetPropertyConvert = "SetPropertyConvert";
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Operations
    {
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalCreate = "DataPortal_Create";
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalFetch = "DataPortal_Fetch";
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalInsert = "DataPortal_Insert";
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalUpdate = "DataPortal_Update";
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalDelete = "DataPortal_Delete";
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalDeleteSelf = "DataPortal_DeleteSelf";
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalExecute = "DataPortal_Execute";
      /// <summary>
      /// 
      /// </summary>
      public const string ChildCreate = "Child_Create";
      /// <summary>
      /// 
      /// </summary>
      public const string ChildFetch = "Child_Fetch";
      /// <summary>
      /// 
      /// </summary>
      public const string ChildInsert = "Child_Insert";
      /// <summary>
      /// 
      /// </summary>
      public const string ChildUpdate = "Child_Update";
      /// <summary>
      /// 
      /// </summary>
      public const string ChildDeleteSelf = "Child_DeleteSelf";
      /// <summary>
      /// 
      /// </summary>
      public const string ChildExecute = "Child_Execute";
      /// <summary>
      /// 
      /// </summary>
      public const string AddObjectAuthorizationRules = "AddObjectAuthorizationRules";
    }

    /// <summary>
    /// 
    /// </summary>
    public static class OperationAttributes
    {
      /// <summary>
      /// 
      /// </summary>
      public const string Create = nameof(Create);
      /// <summary>
      /// 
      /// </summary>
      public const string Fetch = nameof(Fetch);
      /// <summary>
      /// 
      /// </summary>
      public const string Insert = nameof(Insert);
      /// <summary>
      /// 
      /// </summary>
      public const string Update = nameof(Update);
      /// <summary>
      /// 
      /// </summary>
      public const string Delete = nameof(Delete);
      /// <summary>
      /// 
      /// </summary>
      public const string DeleteSelf = nameof(DeleteSelf);
      /// <summary>
      /// 
      /// </summary>
      public const string Execute = nameof(Execute);
      /// <summary>
      /// 
      /// </summary>
      public const string CreateChild = nameof(CreateChild);
      /// <summary>
      /// 
      /// </summary>
      public const string FetchChild = nameof(FetchChild);
      /// <summary>
      /// 
      /// </summary>
      public const string InsertChild = nameof(InsertChild);
      /// <summary>
      /// 
      /// </summary>
      public const string UpdateChild = nameof(UpdateChild);
      /// <summary>
      /// 
      /// </summary>
      public const string DeleteSelfChild = nameof(DeleteSelfChild);
      /// <summary>
      /// 
      /// </summary>
      public const string ExecuteChild = nameof(ExecuteChild);
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Types
    {
      /// <summary>
      /// 
      /// </summary>
      public const string BusinessBase = nameof(BusinessBase);
      /// <summary>
      /// 
      /// </summary>
      public const string BusinessBindingListBase = nameof(BusinessBindingListBase);
      /// <summary>
      /// 
      /// </summary>
      public const string BusinessListBase = nameof(BusinessListBase);
      /// <summary>
      /// 
      /// </summary>
      public const string IBusinessRule = nameof(IBusinessRule);
      /// <summary>
      /// 
      /// </summary>
      public const string IBusinessRuleAsync = nameof(IBusinessRuleAsync);
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalChildOperationAttribute = nameof(DataPortalChildOperationAttribute);
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalOperationAttribute = nameof(DataPortalOperationAttribute);
      /// <summary>
      /// 
      /// </summary>
      public const string DataPortalRootOperationAttribute = nameof(DataPortalRootOperationAttribute);
      /// <summary>
      /// 
      /// </summary>
      public const string ObjectAuthorizationRulesAttribute = nameof(ObjectAuthorizationRulesAttribute);
      /// <summary>
      /// 
      /// </summary>
      public const string DynamicListBase = nameof(DynamicListBase);
      /// <summary>
      /// 
      /// </summary>
      public const string IBusinessObject = nameof(IBusinessObject);
      /// <summary>
      /// 
      /// </summary>
      public const string IMobileObject = nameof(IMobileObject);
      /// <summary>
      /// 
      /// </summary>
      public const string InjectAttribute = nameof(InjectAttribute);
      /// <summary>
      /// 
      /// </summary>
      public const string IPropertyInfo = nameof(IPropertyInfo);
      /// <summary>
      /// 
      /// </summary>
      public const string ManagedObjectBase = nameof(ManagedObjectBase);
      /// <summary>
      /// 
      /// </summary>
      public const string ObjectFactory = nameof(ObjectFactory);
      /// <summary>
      /// 
      /// </summary>
      public const string ReadOnlyBase = nameof(ReadOnlyBase);
      /// <summary>
      /// 
      /// </summary>
      public const string RunLocalAttribute = nameof(RunLocalAttribute);
    }

    /// <summary>
    /// 
    /// </summary>
    public const string AssemblyName = "Csla";
    /// <summary>
    /// 
    /// </summary>
    public const string SerializableAttribute = "SerializableAttribute";
  }
}