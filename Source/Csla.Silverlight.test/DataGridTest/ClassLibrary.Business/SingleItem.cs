//-----------------------------------------------------------------------
// <copyright file="SingleItem.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace ClassLibrary.Business
{
  [Serializable]
  public class SingleItem : BusinessBase<SingleItem>
  {
#if SILVERLIGHT
    public SingleItem() { }
#else
    private SingleItem() { }
#endif

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(new PropertyInfo<int>("Id", "Internal Id", 0));
    public int Id
    {
      get
      {
        return GetProperty<int>(IdProperty);
      }
      set
      {
        SetProperty<int>(IdProperty,value);
      }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(new PropertyInfo<string>("Name", "Item Name", ""));
    public string Name
    {
      get
      {
        return GetProperty<string>(NameProperty);
      }
      set
      {
        SetProperty<string>(NameProperty, value);
      }
    }

    private static PropertyInfo<SmartDate> DateCreatedProperty = RegisterProperty<SmartDate>(new PropertyInfo<SmartDate>("DateCreated", "Date Created On"));
    public string DateCreated
    {
      get
      {
        return GetProperty<SmartDate>(DateCreatedProperty).Text;
      }
      set
      {
        SmartDate test=new SmartDate();
        if (SmartDate.TryParse(value, ref test) == true)
        {
          SetProperty<SmartDate>(DateCreatedProperty, test);
        }
      }
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.IntegerMinValue, new Csla.Validation.CommonRules.IntegerMinValueRuleArgs(IdProperty,1));
      ValidationRules.AddRule<SingleItem>(IsDateValid,new Csla.Validation.RuleArgs(DateCreatedProperty));
    }

    private static bool IsDateValid(SingleItem sender, Csla.Validation.RuleArgs e)
    {
      if (sender.ReadProperty<SmartDate>(DateCreatedProperty) < new SmartDate(new DateTime(2000, 1, 1)))
      {
        e.Description = "Date cannot be in the last century!";
        return false;
      }
      return true;
    }

#if !SILVERLIGHT
    internal static SingleItem GetSingleItem(int id, string name, DateTime createdOn)
    {
      SingleItem newItem = new SingleItem();
      newItem.LoadProperty(IdProperty, id);
      newItem.LoadProperty(NameProperty, name);
      newItem.LoadProperty(DateCreatedProperty, new SmartDate(createdOn));
      //newItem.MarkAsChild();  Leave this off to allow deletion
      newItem.MarkOld();
      return newItem;
    }

    protected override void DataPortal_DeleteSelf()
    {
      Csla.ApplicationContext.GlobalContext["ERLBDelete"] = "Deleted Item " + GetProperty<string>(NameProperty);
    }

    protected override void DataPortal_Insert()
    {
      Csla.ApplicationContext.GlobalContext["ERLBInsert"] = "Inserted Item " + GetProperty<string>(NameProperty);
    }
    protected override void DataPortal_Update()
    {
      Csla.ApplicationContext.GlobalContext["ERLBUpdate"] = "Updating Item " + GetProperty<string>(NameProperty);
      SetProperty<string>(NameProperty,GetProperty<string>(NameProperty) + "_Updated");
    }
#endif
  }
}