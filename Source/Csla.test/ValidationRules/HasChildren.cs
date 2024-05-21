﻿//-----------------------------------------------------------------------
// <copyright file="HasChildren.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class HasChildren : BusinessBase<HasChildren>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id, "Id");
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<ChildList> ChildListProperty = RegisterProperty<ChildList>(c => c.ChildList, "Child list", null, RelationshipTypes.LazyLoad);
    public ChildList ChildList
    {
      get 
      {
        return LazyGetProperty(ChildListProperty, () => GetDataPortal<ChildList>().Create()); 
      }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new OneItem<HasChildren> { PrimaryProperty = ChildListProperty });
    }

    public class OneItem<T> : Rules.BusinessRule
      where T : HasChildren
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        var target = (T)context.Target;
        if (target.ChildList.Count < 1)
          context.AddErrorResult("At least one item required");
      }
    }

    protected override void Initialize()
    {
      base.Initialize();
      ChildList.ListChanged += ChildList_ListChanged;
      ChildChanged += HasChildren_ChildChanged;
    }

    void ChildList_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
    {
      //ValidationRules.CheckRules(ChildListProperty);
    }

    void HasChildren_ChildChanged(object sender, ChildChangedEventArgs e)
    {
      BusinessRules.CheckRules(ChildListProperty);
    }

    [Create]
    private Task Create()
    {
      return BusinessRules.CheckRulesAsync();
    }

    #region Private Helper Methods

    /// <summary>
    /// Construct an instance of IDataPortal<typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type which is to be accessed</typeparam>
    /// <returns>An instance of IDataPortal for use in data access</returns>
    private IDataPortal<T> GetDataPortal<T>() where T : class
    {
      return ApplicationContext.GetRequiredService<IDataPortal<T>>();
    }

    #endregion

  }
}