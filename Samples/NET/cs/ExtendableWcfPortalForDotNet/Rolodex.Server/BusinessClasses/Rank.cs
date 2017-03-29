using System;
using Csla;
using Csla.Data;
using Csla.Rules;
using Csla.Rules.CommonRules;
using Rolodex.Business.Data;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class Rank : BusinessBase<Rank>
  {
    internal static Rank NewRank()
    {
      Rank returnValue = new Rank();
      returnValue.BusinessRules.CheckRules();
      return returnValue;
    }

    public static readonly PropertyInfo<int> RankIdProperty =
      RegisterProperty<int>(new PropertyInfo<int>("RankId", "Rank Id", 0));

    public int RankId
    {
      get { return GetProperty(RankIdProperty); }
    }

    public static readonly PropertyInfo<string> RankNameProperty =
      RegisterProperty<string>(new PropertyInfo<string>("RankName", "Rank Name", string.Empty));

    public string RankName
    {
      get { return GetProperty(RankNameProperty); }
      set { SetProperty(RankNameProperty, value); }
    }

    public static void AddObjectAuthorizationRules()
    {
      var canWrite = new[] {"AdminUser", "RegularUser"};
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};
      var admin = new[] {"AdminUser"};

      BusinessRules.AddRule(typeof(Rank), new IsInRole(AuthorizationActions.CreateObject, admin));
      BusinessRules.AddRule(typeof(Rank), new IsInRole(AuthorizationActions.DeleteObject, admin));
      BusinessRules.AddRule(typeof(Rank), new IsInRole(AuthorizationActions.EditObject, canWrite));
      BusinessRules.AddRule(typeof(Rank), new IsInRole(AuthorizationActions.GetObject, canRead));
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Required(RankNameProperty));
      BusinessRules.AddRule(new MaxLength(RankNameProperty, 20));

      var canWrite = new[] {"AdminUser", "RegularUser"};
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};

      FieldManager.GetRegisteredProperties().ForEach(item =>
      {
        BusinessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, item, canWrite));
        BusinessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, item, canRead));
      });
    }

    protected override void DataPortal_Insert()
    {
      using (var manager =
        ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
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
      using (var manager =
        ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
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
      if (!IsNew)
      {
        using (var manager =
          ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
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
      fetchedRank.LoadProperty(RankIdProperty, rank.RankId);
      fetchedRank.LoadProperty(RankNameProperty, rank.Rank);
      fetchedRank.MarkOld();
      return fetchedRank;
    }
  }
}