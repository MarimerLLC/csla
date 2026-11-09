//-----------------------------------------------------------------------
// <copyright file="TestObjects.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business types used by the model binder tests</summary>
//-----------------------------------------------------------------------

using Csla.Rules;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Csla.AspNetCore.Test.ModelBinding;

[Serializable]
public class Parent : BusinessBase<Parent>
{
  public static readonly PropertyInfo<string> SecretProperty = RegisterProperty<string>(nameof(Secret));
  [BindNever]
  public string Secret
  {
    get => GetProperty(SecretProperty);
    set => SetProperty(SecretProperty, value);
  }

  public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
  public int Id
  {
    get => GetProperty(IdProperty);
    set => SetProperty(IdProperty, value);
  }

  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<Address> AddressProperty = RegisterProperty<Address>(nameof(Address));
  public Address Address
  {
    get => GetProperty(AddressProperty);
    private set => LoadProperty(AddressProperty, value);
  }

  public static readonly PropertyInfo<StreetOnlyAddress> MailingAddressProperty = RegisterProperty<StreetOnlyAddress>(nameof(MailingAddress));
  public StreetOnlyAddress MailingAddress
  {
    get => GetProperty(MailingAddressProperty);
    private set => LoadProperty(MailingAddressProperty, value);
  }

  public static readonly PropertyInfo<ChildList> ChildrenProperty = RegisterProperty<ChildList>(nameof(Children));
  public ChildList Children
  {
    get => GetProperty(ChildrenProperty);
    private set => LoadProperty(ChildrenProperty, value);
  }

  [Fetch]
  private void Fetch(int id, [Inject] IChildDataPortal<Address> addressPortal, [Inject] IChildDataPortal<ChildList> childPortal)
  {
    using (BypassPropertyChecks)
    {
      Id = id;
      Name = "parent";
      Secret = "secret";
    }
    Address = addressPortal.FetchChild("street");
    Children = childPortal.FetchChild(2);
  }
}

[Serializable]
public class Address : BusinessBase<Address>
{
  public static readonly PropertyInfo<string> StreetProperty = RegisterProperty<string>(nameof(Street));
  public string Street
  {
    get => GetProperty(StreetProperty);
    set => SetProperty(StreetProperty, value);
  }

  public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(nameof(City));
  public string City
  {
    get => GetProperty(CityProperty);
    set => SetProperty(CityProperty, value);
  }

  [FetchChild]
  private void FetchChild(string street)
  {
    using (BypassPropertyChecks)
    {
      Street = street;
      City = "city";
    }
  }
}

[Serializable]
[Microsoft.AspNetCore.Mvc.Bind(nameof(Street))]
public class StreetOnlyAddress : BusinessBase<StreetOnlyAddress>
{
  public static readonly PropertyInfo<string> StreetProperty = RegisterProperty<string>(nameof(Street));
  public string Street
  {
    get => GetProperty(StreetProperty);
    set => SetProperty(StreetProperty, value);
  }

  public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(nameof(City));
  public string City
  {
    get => GetProperty(CityProperty);
    set => SetProperty(CityProperty, value);
  }
}

[Serializable]
public class ChildList : BusinessListBase<ChildList, Child>
{
  [FetchChild]
  private void FetchChild(int count, [Inject] IChildDataPortal<Child> childPortal)
  {
    using (LoadListMode)
    {
      for (var i = 0; i < count; i++)
        Add(childPortal.FetchChild($"child{i}", 2));
    }
  }
}

[Serializable]
public class RootChildList : BusinessListBase<RootChildList, Child>
{
}

[Serializable]
public class NoNewItemsRootChildList : BusinessListBase<NoNewItemsRootChildList, Child>
{
  public NoNewItemsRootChildList()
  {
    AllowNew = false;
  }
}

[Serializable]
public class Child : BusinessBase<Child>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<int> QuantityProperty = RegisterProperty<int>(nameof(Quantity));
  public int Quantity
  {
    get => GetProperty(QuantityProperty);
    set => SetProperty(QuantityProperty, value);
  }

  public static readonly PropertyInfo<GrandChildList> GrandChildrenProperty = RegisterProperty<GrandChildList>(nameof(GrandChildren));
  public GrandChildList GrandChildren
  {
    get => GetProperty(GrandChildrenProperty);
    private set => LoadProperty(GrandChildrenProperty, value);
  }

  [FetchChild]
  private void FetchChild(string name, int grandChildCount, [Inject] IChildDataPortal<GrandChildList> grandChildPortal)
  {
    using (BypassPropertyChecks)
    {
      Name = name;
      Quantity = 1;
    }
    GrandChildren = grandChildPortal.FetchChild(name, grandChildCount);
  }
}

[Serializable]
public class GrandChildList : BusinessListBase<GrandChildList, GrandChild>
{
  [FetchChild]
  private void FetchChild(string parentName, int count, [Inject] IChildDataPortal<GrandChild> grandChildPortal)
  {
    using (LoadListMode)
    {
      for (var i = 0; i < count; i++)
        Add(grandChildPortal.FetchChild($"{parentName}-grandchild{i}"));
    }
  }
}

[Serializable]
public class GrandChild : BusinessBase<GrandChild>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  protected override void AddBusinessRules()
  {
    base.AddBusinessRules();
    BusinessRules.AddRule(new NotBadAsyncRule(NameProperty));
  }

  [FetchChild]
  private void FetchChild(string name)
  {
    using (BypassPropertyChecks)
      Name = name;
  }

  /// <summary>
  /// Async rule that completes after a delay, so tests can check
  /// that binding waits for the whole graph to be idle.
  /// </summary>
  private sealed class NotBadAsyncRule : BusinessRuleAsync
  {
    public NotBadAsyncRule(Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties.Add(primaryProperty);
    }

    protected override async Task ExecuteAsync(IRuleContext context)
    {
      await Task.Delay(TimeSpan.FromMilliseconds(100));
      if ((string)context.InputPropertyValues[PrimaryProperty] == "bad")
        context.AddErrorResult("Name can't be bad");
    }
  }
}
