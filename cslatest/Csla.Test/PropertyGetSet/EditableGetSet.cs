using System;
using System.Collections.Generic;
using System.Text;
using Csla;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.PropertyGetSet
{
  [Serializable]
  public class EditableGetSet : Csla.BusinessBase<EditableGetSet>
  {
    private static Csla.PropertyInfo<string> F01Property = new Csla.PropertyInfo<string>("F01");
    private string _f01 = F01Property.DefaultValue;
    public string F01
    {
      get { return GetProperty<string>(F01Property, _f01); }
      set { SetProperty<string>(F01Property, ref _f01, value); }
    }

    private static Csla.PropertyInfo<int> F02Property = new Csla.PropertyInfo<int>("F02");
    private int _f02 = F02Property.DefaultValue;
    public int F02
    {
      get { return GetProperty<int>(F02Property, _f02); }
      set { SetProperty<int>(F02Property, ref _f02, value); }
    }

    private static Csla.PropertyInfo<string> F03Property = new Csla.PropertyInfo<string>("F03", "field 3", "n/a");
    private string _f03 = F03Property.DefaultValue;
    public string F03
    {
      get { return GetProperty<string>(F03Property, _f03); }
      set { SetProperty<string>(F03Property, ref _f03, value); }
    }

    private static Csla.PropertyInfo<string> M01Property = new Csla.PropertyInfo<string>("M01");
    public string M01
    {
      get { return GetProperty<string>(M01Property); }
      set { SetProperty<string>(M01Property, value); }
    }

    private static Csla.PropertyInfo<int> M02Property = new Csla.PropertyInfo<int>("M02");
    public int M02
    {
      get { return GetProperty<int>(M02Property); }
      set { SetProperty<int>(M02Property, value); }
    }

    private static Csla.PropertyInfo<string> M03Property = new Csla.PropertyInfo<string>("M03", "field 3", "n/a");
    public string M03
    {
      get { return GetProperty<string>(M03Property); }
      set { SetProperty<string>(M03Property, value); }
    }

    private static Csla.PropertyInfo<Csla.SmartDate> F04Property = new Csla.PropertyInfo<Csla.SmartDate>("F04");
    private Csla.SmartDate _F04 = F04Property.DefaultValue;
    public string F04
    {
      get { return GetProperty<Csla.SmartDate, string>(F04Property, _F04); }
      set { SetProperty<Csla.SmartDate, string>(F04Property, ref _F04, value); }
    }

    private static Csla.PropertyInfo<Csla.SmartDate> M04Property = new Csla.PropertyInfo<Csla.SmartDate>("M04");
    public string M04
    {
      get { return GetProperty<Csla.SmartDate, string>(M04Property); }
      set { SetProperty<Csla.SmartDate, string>(M04Property, value); }
    }

    private static Csla.PropertyInfo<EditableGetSet> C01Property = new Csla.PropertyInfo<EditableGetSet>("C01");
    public EditableGetSet C01
    {
      get 
      { 
        if (!FieldManager.FieldExists(C01Property))
          SetProperty<EditableGetSet>(C01Property, new EditableGetSet(true));
        return GetProperty<EditableGetSet>(C01Property); 
      }
    }

    public int EditLevel
    {
      get { return base.EditLevel; }
    }

    public void MarkClean()
    {
      base.MarkClean();
    }

    public EditableGetSet()
    {
      MarkNew();
      MarkClean();
    }

    public EditableGetSet(bool isChild)
    {
      if (isChild)
      {
        MarkAsChild();
        MarkNew();
      }
    }

    #region " Data Access "

    protected override void DataPortal_Insert()
    {
      //FieldManager.UpdateChildren();
      if (FieldManager.FieldExists(C01Property))
        C01.Insert();
    }

    protected override void DataPortal_Update()
    {
      //FieldManager.UpdateChildren();
      if (FieldManager.FieldExists(C01Property))
        C01.Update();
    }

    internal void Insert()
    {
      MarkOld();
    }

    internal void Update()
    {
      MarkOld();
    }

    #endregion
  }
}
