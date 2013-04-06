using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;

namespace Csla.Test.BypassPropertyChecks
{
  [Serializable()]
  public class BypassBusinessBase : BusinessBase<BypassBusinessBase>
  {
    public BypassBusinessBase()
    {
      MarkOld();
    }

    protected static PropertyInfo<int> IdProperty = RegisterProperty<int>(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    protected static PropertyInfo<int> Id2Property = RegisterProperty<int>(new PropertyInfo<int>("Id2", "Id2"));
    public int Id2
    {
      get { return GetProperty<int>(Id2Property); }
      set { SetProperty<int>(Id2Property, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    private int _id3;
    protected static PropertyInfo<int> Id3Property = RegisterProperty<int>(new PropertyInfo<int>("Id3", "Id3"));
    public int Id3
    {
      get { return GetProperty<int>(Id3Property, _id3); }
      set { SetProperty<int>(Id3Property.Name, ref _id3, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    private int _id4;
    protected static PropertyInfo<int> Id4Property = RegisterProperty<int>(new PropertyInfo<int>("Id4", "Id4"));
    public int Id4
    {
      get { return GetProperty<int>(Id4Property, _id4); }
      set { SetProperty<int>(Id4Property.Name, ref _id4, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowRead(IdProperty, new string[] { "Admin" });
      AuthorizationRules.AllowWrite(IdProperty, new string[] { "Admin" });
      AuthorizationRules.AllowRead(Id2Property, new string[] { "Random" });
      AuthorizationRules.AllowWrite(Id2Property, new string[] { "Random" });

      AuthorizationRules.AllowRead(Id3Property, new string[] { "Admin" });
      AuthorizationRules.AllowWrite(Id3Property, new string[] { "Admin" });
      AuthorizationRules.AllowRead(Id4Property, new string[] { "Random" });
      AuthorizationRules.AllowWrite(Id4Property, new string[] { "Random" });
    }

    public void LoadIdByPass(int id)
    {
      using (this.BypassPropertyChecks)
      {
        Id = id;
      }
    }

    public void LoadId(int id)
    {
      Id = id;
    }

    public int ReadIdByPass()
    {
      using (this.BypassPropertyChecks)
      {
        return Id;
      }
    }

    public int ReadId()
    {
      return Id;
    }

    public void LoadId2ByPass(int id)
    {
      using (this.BypassPropertyChecks)
      {
        Id2 = id;
      }
    }

    public void LoadId2(int id)
    {
      Id2 = id;
    }

    public int ReadId2ByPass()
    {
      using (this.BypassPropertyChecks)
      {
        return Id2;
      }
    }

    public int ReadId2()
    {
      return Id2;
    }


    public void LoadId3ByPass(int id)
    {
      using (this.BypassPropertyChecks)
      {
        Id3 = id;
      }
    }

    public void LoadId3(int id)
    {
      Id3 = id;
    }

    public int ReadId3ByPass()
    {
      using (this.BypassPropertyChecks)
      {
        return Id3;
      }
    }

    public int ReadId3()
    {
      return Id3;
    }

    public void LoadId4ByPass(int id)
    {
      using (this.BypassPropertyChecks)
      {
        Id4 = id;
      }
    }

    public void LoadId4(int id)
    {
      Id4 = id;
    }

    public int ReadId4ByPass()
    {
      using (this.BypassPropertyChecks)
      {
        return Id4;
      }
    }

    public int ReadId4()
    {
      return Id4;
    }

  }
}
