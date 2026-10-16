//-----------------------------------------------------------------------
// <copyright file="Models.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business types exercised by the NativeAOT smoke test</summary>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;
using Csla.Server;

namespace Csla.AotSmokeTest
{
  public enum PersonKind
  {
    Customer,
    Vendor
  }

  /// <summary>
  /// Editable root with a child list, data annotations, rule classes,
  /// lambda rules, an async rule and property/object authorization rules.
  /// </summary>
  [CslaImplementProperties]
  public partial class PersonEdit : BusinessBase<PersonEdit>
  {
    public partial int Id { get; private set; }

    [Required]
    [StringLength(20)]
    public partial string Name { get; set; }

    [Range(0, 150)]
    public partial int Age { get; set; }

    public partial int? Score { get; set; }
    public partial long Total { get; private set; }
    public partial PersonKind Kind { get; set; }
    public partial string Nickname { get; set; }
    public partial string Secret { get; set; }
    public partial FriendList Friends { get; private set; }

    public bool CanWriteSecret => CanWriteProperty(SecretProperty);

    public void LoadUntyped(IPropertyInfo propertyInfo, object value) => LoadProperty(propertyInfo, value);

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule<PersonEdit>(NicknameProperty, o => o.Nickname != "bad", "{0} can't be 'bad'.");
      BusinessRules.AddRule(new NotReservedAsync(NicknameProperty));
      BusinessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, SecretProperty, "Admin"));
    }

    [ObjectAuthorizationRules]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(PersonEdit), new IsInRole(AuthorizationActions.DeleteObject, "Admin"));
    }

    [Create]
    private async Task Create([Inject] IChildDataPortal<FriendList> friendsPortal)
    {
      LoadProperty(FriendsProperty, await friendsPortal.CreateChildAsync());
      await BusinessRules.CheckRulesAsync();
    }

    [Fetch]
    private async Task Fetch(int id, [Inject] IChildDataPortal<FriendList> friendsPortal)
    {
      LoadProperty(IdProperty, id);
      LoadProperty(NameProperty, "Fetched");
      LoadProperty(AgeProperty, id);
      LoadProperty(FriendsProperty, await friendsPortal.FetchChildAsync(id));
      await BusinessRules.CheckRulesAsync();
    }

    // Returns Task<T> on purpose: exercises the data portal's Task<T> result conversion
    [Insert]
    private async Task<bool> Insert()
    {
      await Task.Yield();
      LoadProperty(IdProperty, 100);
      await FieldManager.UpdateChildrenAsync();
      return true;
    }

    [Update]
    private async Task Update()
    {
      await Task.Yield();
      await FieldManager.UpdateChildrenAsync();
    }

    [DeleteSelf]
    private async Task DeleteSelf()
    {
      await Task.Yield();
      MarkNew();
    }

    [Delete]
    private Task Delete(int id) => Task.CompletedTask;
  }

  public class NotReservedAsync : BusinessRuleAsync
  {
    public NotReservedAsync(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties.Add(primaryProperty);
    }

    protected override async Task ExecuteAsync(IRuleContext context)
    {
      await Task.Delay(1);
      if ((string)context.InputPropertyValues[PrimaryProperty!] == "reserved")
        context.AddErrorResult("Nickname is reserved");
    }
  }

  /// <summary>
  /// Editable child list.
  /// </summary>
  public class FriendList : BusinessListBase<FriendList, FriendEdit>
  {
    [CreateChild]
    private void CreateChild()
    { }

    [FetchChild]
    private async Task FetchChild(int personId, [Inject] IChildDataPortal<FriendEdit> friendPortal)
    {
      using (LoadListMode)
      {
        Add(await friendPortal.FetchChildAsync($"Friend of {personId}"));
      }
    }
  }

  /// <summary>
  /// Editable child.
  /// </summary>
  [CslaImplementProperties]
  public partial class FriendEdit : BusinessBase<FriendEdit>
  {
    [Required]
    public partial string Name { get; set; }

    [CreateChild]
    private async Task CreateChild()
    {
      await BusinessRules.CheckRulesAsync();
    }

    [FetchChild]
    private void FetchChild(string name)
    {
      LoadProperty(NameProperty, name);
    }

    [InsertChild]
    private void InsertChild()
    { }

    [UpdateChild]
    private void UpdateChild()
    { }

    [DeleteSelfChild]
    private void DeleteSelfChild()
    { }
  }

  /// <summary>
  /// Read-only root list of read-only children.
  /// </summary>
  public class PersonInfoList : ReadOnlyListBase<PersonInfoList, PersonInfo>
  {
    [Fetch]
    private async Task Fetch([Inject] IChildDataPortal<PersonInfo> childPortal)
    {
      using (LoadListMode)
      {
        Add(await childPortal.FetchChildAsync("Ada"));
        Add(await childPortal.FetchChildAsync("Grace"));
      }
      IsReadOnly = true;
    }
  }

  [CslaImplementProperties]
  public partial class PersonInfo : ReadOnlyBase<PersonInfo>
  {
    public partial string Name { get; private set; }

    [FetchChild]
    private void FetchChild(string name)
    {
      LoadProperty(NameProperty, name);
    }
  }

  /// <summary>
  /// Command executed with criteria in one call.
  /// </summary>
  [CslaImplementProperties]
  public partial class PersonExists : CommandBase<PersonExists>
  {
    public partial bool Exists { get; private set; }

    [Execute]
    private async Task Execute(string name)
    {
      await Task.Yield();
      LoadProperty(ExistsProperty, name == "Ada");
    }
  }

  /// <summary>
  /// Editable root persisted through an object factory.
  /// </summary>
  [CslaImplementProperties]
  [ObjectFactory(typeof(InventoryFactory))]
  public partial class InventoryItem : BusinessBase<InventoryItem>
  {
    public partial string Sku { get; set; }
  }

  public class InventoryFactory : ObjectFactory
  {
    public InventoryFactory(ApplicationContext applicationContext)
      : base(applicationContext)
    { }

    // Returns Task<T> after a real suspension, so the data portal must extract the
    // result through its Task<T> conversion and the scenario verifies that value
    public async Task<InventoryItem> Fetch(string sku)
    {
      await Task.Delay(1);
      var item = (InventoryItem)ApplicationContext.CreateInstanceDI(typeof(InventoryItem));
      using (BypassPropertyChecks(item))
      {
        item.Sku = sku;
      }
      MarkOld(item);
      return item;
    }
  }

  /// <summary>
  /// Read-only root whose factory is named only by a string, so nothing but the
  /// ObjectFactoryAttribute type name keeps the factory type from being trimmed.
  /// </summary>
  [CslaImplementProperties]
  [ObjectFactory("Csla.AotSmokeTest.CatalogFactory, Csla.AotSmokeTest")]
  public partial class CatalogItem : ReadOnlyBase<CatalogItem>
  {
    public partial string Code { get; private set; }
  }

  public class CatalogFactory : ObjectFactory
  {
    public CatalogFactory(ApplicationContext applicationContext)
      : base(applicationContext)
    { }

    public CatalogItem Fetch(string code)
    {
      var item = (CatalogItem)ApplicationContext.CreateInstanceDI(typeof(CatalogItem));
      LoadProperty(item, CatalogItem.CodeProperty, code);
      return item;
    }
  }
}
