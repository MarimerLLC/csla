using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Validation;

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
using Csla.Data;
using RolodexEF;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class Rank : BusinessBase<Rank>
  {
#if SILVERLIGHT
    public Rank() { }
#else
    private Rank() { }
#endif

    internal static Rank NewRank()
    {
      Rank returnValue = new Rank();
      returnValue.ValidationRules.CheckRules();
      return returnValue;
    }

    private static PropertyInfo<int> RankIdProperty = RegisterProperty<int>(new PropertyInfo<int>("RankId", "Rank Id", 0));
    public int RankId
    {
      get
      {
        return GetProperty(RankIdProperty);
      }
    }

    private static PropertyInfo<string> RankNameProperty = RegisterProperty<string>(new PropertyInfo<string>("RankName", "Rank Name", string.Empty));
    public string RankName
    {
      get
      {
        return GetProperty(RankNameProperty);
      }
      set
      {
        SetProperty(RankNameProperty, value);
      }
    }

    protected override void AddAuthorizationRules()
    {
      string[] canWrite = new string[] { "AdminUser", "RegularUser" };
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
      string[] admin = new string[] { "AdminUser" };

      foreach (var item in this.FieldManager.GetRegisteredProperties())
      {
        AuthorizationRules.AllowWrite(item, canWrite);
        AuthorizationRules.AllowRead(item, canRead);
      }
    }

    public static void AddObjectAuthorizationRules()
    {
      string[] canWrite = new string[] { "AdminUser", "RegularUser" };
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
      string[] admin = new string[] { "AdminUser" };
      AuthorizationRules.AllowCreate(typeof(Rank), admin);
      AuthorizationRules.AllowDelete(typeof(Rank), admin);
      AuthorizationRules.AllowEdit(typeof(Rank), canWrite);
      AuthorizationRules.AllowGet(typeof(Rank), canRead);
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, new Csla.Validation.RuleArgs(RankNameProperty));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs(RankNameProperty, 20));

    }

#if !SILVERLIGHT

    protected override void DataPortal_Insert()
    {
      using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        RolodexEF.Ranks newRank = new RolodexEF.Ranks();
        newRank.Rank = ReadProperty(RankNameProperty);
        manager.ObjectContext.AddToRanks(newRank);
        manager.ObjectContext.SaveChanges();
        LoadProperty(RankIdProperty, newRank.RankId);
      }
    }
    protected override void DataPortal_Update()
    {
      using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {

        RolodexEF.Ranks newRank = new RolodexEF.Ranks();
        newRank.RankId = ReadProperty(RankIdProperty);
        newRank.EntityKey = new System.Data.EntityKey("RolodexEntities.Ranks", "RankId", newRank.RankId);
        manager.ObjectContext.Attach(newRank);

        newRank.Rank = ReadProperty(RankNameProperty);
        manager.ObjectContext.SaveChanges();
      }
    }

    protected override void DataPortal_DeleteSelf()
    {
      if (!this.IsNew)
      {
        using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
        {
          RolodexEF.Ranks deleted = new RolodexEF.Ranks();
          deleted.RankId = ReadProperty(RankIdProperty);
          deleted.EntityKey = new System.Data.EntityKey("RolodexEntities.Ranks", "RankId", deleted.RankId);
          manager.ObjectContext.Attach(deleted);
          manager.ObjectContext.DeleteObject(deleted);
          manager.ObjectContext.SaveChanges();
        }
      }
    }

    internal static Rank GetRank(RolodexEF.Ranks rank)
    {
      Rank fetchedRank = new Rank();
      fetchedRank.LoadProperty<int>(RankIdProperty, rank.RankId);
      fetchedRank.LoadProperty<string>(RankNameProperty, rank.Rank);
      fetchedRank.MarkOld();
      return fetchedRank;
    }

#endif
  }
}
