﻿//-----------------------------------------------------------------------
// <copyright file="EditableGetSet.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;

namespace Csla.Test.PropertyGetSet
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [Serializable]
  public class EditableGetSet : EditableGetSetBase<EditableGetSet>
  {
    private static PropertyInfo<string> FieldBackedStringProperty = RegisterProperty<string>(typeof(EditableGetSet), new PropertyInfo<string>("FieldBackedString", RelationshipTypes.PrivateField));
    private string _fieldBackedString = FieldBackedStringProperty.DefaultValue;
    public string FieldBackedString
    {
      get { return GetProperty<string>(FieldBackedStringProperty, _fieldBackedString); }
      set { SetProperty<string>(FieldBackedStringProperty, ref _fieldBackedString, value); }
    }

    private static PropertyInfo<int> F02Property = RegisterProperty<int>(typeof(EditableGetSet), new PropertyInfo<int>("F02", RelationshipTypes.PrivateField));
    private int _f02 = F02Property.DefaultValue;
    public int F02
    {
      get { return GetProperty<int>(F02Property, _f02); }
      set { SetProperty<int>(F02Property, ref _f02, value); }
    }

    private static PropertyInfo<string> F03Property = RegisterProperty<string>(c => c.F03, "field 3", "n/a", RelationshipTypes.PrivateField);
    private string _f03 = F03Property.DefaultValue;
    public string F03
    {
      get { return GetProperty<string>(F03Property, _f03); }
      set { SetProperty<string>(F03Property, ref _f03, value); }
    }

    private static PropertyInfo<Csla.SmartDate> F04Property = RegisterProperty<Csla.SmartDate>(typeof(EditableGetSet), new PropertyInfo<Csla.SmartDate>("F04", RelationshipTypes.PrivateField));
    private Csla.SmartDate _F04 = F04Property.DefaultValue;
    public string F04
    {
      get { return GetPropertyConvert<Csla.SmartDate, string>(F04Property, _F04); }
      set { SetPropertyConvert(F04Property, ref _F04, value); }
    }

    private static PropertyInfo<bool> F05Property = RegisterProperty<bool>(nameof(F05), "field 5", false, RelationshipTypes.PrivateField);
    private bool _f05 = F05Property.DefaultValue;
    public bool F05
    {
      get { return GetProperty<bool>(F05Property, _f05); }
      set { SetProperty<bool>(F05Property, ref _f05, value); }
    }

    private static PropertyInfo<object> F06Property = RegisterProperty<object>(c => c.F06, "field 6", null, RelationshipTypes.PrivateField);
    private object _F06 = string.Empty;
    public string F06
    {
      get { return GetPropertyConvert<object, string>(F06Property, _F06); }
      set { SetPropertyConvert<object, string>(F06Property, ref _F06, value); }
    }

    public static readonly PropertyInfo<string> ManagedStringFieldProperty = RegisterProperty<string>(nameof(ManagedStringField));
    public string ManagedStringField
    {
      get => GetProperty(ManagedStringFieldProperty);
      set => SetProperty(ManagedStringFieldProperty, value);
    }

    public bool ManagedStringFieldDirty
    {
      get { return FieldManager.IsFieldDirty(ManagedStringFieldProperty); }
    }

    private static PropertyInfo<int> M02Property = RegisterProperty(typeof(EditableGetSet), new PropertyInfo<int>("M02"));
    public int M02
    {
      get { return GetProperty<int>(M02Property); }
      set { SetProperty<int>(M02Property, value); }
    }

    public void LoadM02(int value)
    {
      var p = M02Property as Core.IPropertyInfo;
      LoadProperty(p, value);
    }

    private static PropertyInfo<string> M03Property = RegisterProperty(c => c.M03, "field 3", "n/a");
    public string M03
    {
      get { return GetProperty<string>(M03Property); }
      set { SetProperty<string>(M03Property, value); }
    }

    private static PropertyInfo<Csla.SmartDate> M04Property = RegisterProperty(typeof(EditableGetSet), new PropertyInfo<Csla.SmartDate>("M04"));
    public string M04
    {
      get { return GetPropertyConvert<Csla.SmartDate, string>(M04Property); }
      set { SetPropertyConvert(M04Property, value); }
    }

    private static PropertyInfo<bool> M05Property = RegisterProperty<bool>(c => c.M05, "field 5");
    public bool M05
    {
      get { return GetProperty(M05Property); }
      set { SetProperty(M05Property, value); }
    }

    private static PropertyInfo<Guid> M06Property = RegisterProperty<Guid>(c => c.M06, "field 6");
    public Guid M06
    {
      get { return GetProperty(M06Property); }
      set { SetProperty(M06Property, value); }
    }

    private static PropertyInfo<object> M07Property = RegisterProperty<object>(c => c.M07, "field 7");
    public string M07
    {
      get { return GetPropertyConvert<object, string>(M07Property); }
      set { SetPropertyConvert<object, string>(M07Property, value); }
    }

    public static readonly PropertyInfo<string> _m08Property = RegisterProperty<string>(c => c.M08);
    internal string M08
    {
      get { return GetProperty(_m08Property); }
      set { SetProperty(_m08Property, value); }
    }

    public static readonly PropertyInfo<string> _m09Property = RegisterProperty<string>(c => c.M09);
    private string M09
    {
      get { return GetProperty(_m09Property); }
      set { SetProperty(_m09Property, value); }
    }

    public void LoadInternalAndPrivate(string value)
    {
      using (BypassPropertyChecks)
      {
        LoadProperty(_m08Property, value);
        LoadProperty(_m09Property, value);
      }
    }

    private static PropertyInfo<EditableGetSet> ManagedChildProperty = RegisterProperty(
      typeof(EditableGetSet), new PropertyInfo<EditableGetSet>("ManagedChild", RelationshipTypes.LazyLoad));
    public EditableGetSet ManagedChild
    {
      get 
      { 
        return LazyGetProperty(ManagedChildProperty, () => GetDataPortal<EditableGetSet>().Create()); 
      }
    }

    private static PropertyInfo<ChildList> ManagedChildListProperty = RegisterProperty(
      typeof(EditableGetSet), new PropertyInfo<ChildList>("ManagedChildList", RelationshipTypes.LazyLoad));
    public ChildList ManagedChildList
    {
      get
      {
        return LazyGetProperty(ManagedChildListProperty, () => GetDataPortal<ChildList>().Create());
      }
    }

    private static PropertyInfo<ChildList> LazyChildProperty = 
      RegisterProperty(new PropertyInfo<ChildList>("LazyChild", "Child list", null, RelationshipTypes.LazyLoad));
    public ChildList LazyChild
    {
      get { return LazyGetProperty(LazyChildProperty, () => GetDataPortal<ChildList>().Create()); }
      set { SetProperty(LazyChildProperty, value); }
    }

    public new int EditLevel
    {
      get { return base.EditLevel; }
    }

    public new void MarkClean()
    {
      base.MarkClean();
    }

    #region Factory Methods

    public static EditableGetSet NewObject(IDataPortal<EditableGetSet> dataPortal)
    {
      return dataPortal.Create();
    }

    public static EditableGetSet NewChildObject(IChildDataPortal<EditableGetSet> childDataPortal)
    {
      return childDataPortal.CreateChild();
    }

    public static EditableGetSet GetObject(IDataPortal<EditableGetSet> dataPortal)
    {
      return dataPortal.Fetch();
    }

    public static EditableGetSet GetChildObject(IChildDataPortal<EditableGetSet> dataPortal)
    {
      return dataPortal.FetchChild();
    }

    #endregion

    #region Data Access

    [Create]
    private void Create([Inject] IChildDataPortal<EditableGetSet> dataPortal)
    {
      LoadProperty(M06Property, null);
    }

    [CreateChild]
    private void CreateChild()
    {
      LoadProperty(M06Property, null);
    }

    [Fetch]
    private void DataPortal_Fetch()
    {
      LoadProperty(M06Property, null);
    }

    [FetchChild]
    private void DataPortal_FetchChild()
    {
      LoadProperty(M06Property, null);
    }

    [Insert]
    [InsertChild]
    protected void DataPortal_Insert()
    {
      FieldManager.UpdateChildren();
    }

    [Update]
    [UpdateChild]
    protected void DataPortal_Update()
    {
      FieldManager.UpdateChildren();
    }

    #endregion

    #region MobileObject

    protected override void OnGetState(SerializationInfo info, Core.StateMode mode)
    {
      if(_fieldBackedString!=null)
        info.AddValue("_fieldBackedString", _fieldBackedString);
      if (_f02 != 0)
        info.AddValue("_f02", _f02);
      if (_f03 != null)
        info.AddValue("_f03", _f03);
      if (_F04 != default(Csla.SmartDate))
        info.AddValue("_f04", _F04);
      if (_f05 == true)
        info.AddValue("_f05", _f05);

      base.OnGetState(info, mode);
    }

    protected override void OnSetState(SerializationInfo info, Core.StateMode mode)
    {
      if (info.Values.ContainsKey("_fieldBackedString"))
        _fieldBackedString = info.GetValue<string>("_fieldBackedString");
      if (info.Values.ContainsKey("_f02"))
        _f02 = info.GetValue<int>("_f02");
      if(info.Values.ContainsKey("_f03"))
        _f03 = info.GetValue<string>("_f03");
      if(info.Values.ContainsKey("_f04"))
        _F04 = info.GetValue<Csla.SmartDate>("_f04");
      if (info.Values.ContainsKey("_f05"))
        _f05 = info.GetValue<bool>("_f05");

      base.OnSetState(info, mode);
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Construct an instance of IDataPortal<typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type which is to be accessed</typeparam>
    /// <returns>An instance of IDataPortal for use in data access</returns>
    private IDataPortal<T> GetDataPortal<T>() where T:class
    {
      return ApplicationContext.GetRequiredService<IDataPortal<T>>();
    }

    #endregion
  }

  [Serializable]
  public class EditableGetSetNFI : EditableGetSetNFIBase<EditableGetSetNFI>
  {
    public static PropertyInfo<string> DataProperty = RegisterProperty(new PropertyInfo<string>("Data", "Data"));
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public new int EditLevel
    {
      get { return base.EditLevel; }
    }

    [Fetch]
    private void DataPortal_Fetch()
    {
      LoadProperty(DataProperty, "Hi");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
    }

    [Update]
    protected void DataPortal_Update()
    {
      //FieldManager.UpdateChildren();
    }


    internal void Insert()
    {
      MarkOld();
    }

    internal void Update()
    {
      MarkOld();
    }
  }
}