using Csla.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Test.ChildChanged
{
  [TestClass]
  public class OptimizeChildChangedTests
  {



    public enum enumBO { SimpleBO, SimpleBOList };
    public enum enumEvent { OnPropertyChanged, OnChildChanged, PropertyChanged, ChildChanged };
    public struct EventDetail
    {
      public int SequenceID => NextSequenceID;
      public int UniqueID;
      public int Depth { get; set; }
      public enumBO BO { get; set; }
      public enumEvent Event { get; set; }
      public string PropertyName { get; set; }

      public override string ToString()
      {
        return $"{SequenceID}: {BO.ToString()} {UniqueID} {Depth} {Event.ToString()} {PropertyName}";
      }
    }

    private static int _UniqueID = 0;
    protected static int NextUniqueID() => _UniqueID++;

    private static int _SequenceID = 0;
    protected static int NextSequenceID => _SequenceID++;

    protected static List<EventDetail> EventDetails = new List<EventDetail>();


    [Serializable]
    public class SimpleBO : BusinessBase<SimpleBO>
    {
      // I don't need these raising CSLA events
      public int Depth { get; private set; }
      public int UniqueID = NextUniqueID();

      public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
      public string Name
      {
        get { return GetProperty(NameProperty); }
        set { SetProperty(NameProperty, value); }
      }

      public static readonly PropertyInfo<SimpleBO> ChildProperty = RegisterProperty<SimpleBO>(c => c.Child);
      public SimpleBO Child
      {
        get { return GetProperty(ChildProperty); }
        set { SetProperty(ChildProperty, value); }
      }

      public static readonly PropertyInfo<SimpleBOList> ChildListProperty = RegisterProperty<SimpleBOList>(c => c.ChildList);
      public SimpleBOList ChildList
      {
        get { return GetProperty(ChildListProperty); }
        set { SetProperty(ChildListProperty, value); }
      }

      protected override void AddBusinessRules()
      {
        base.AddBusinessRules();

        BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));

      }

      private void DataPortal_Fetch()
      {
        Depth = 0;
        LoadProperty(NameProperty, "Jupiter");
        LoadProperty(ChildProperty, Csla.DataPortal.FetchChild<SimpleBO>(Depth + 1));
        LoadProperty(ChildListProperty, Csla.DataPortal.FetchChild<SimpleBOList>(Depth + 1));

        BusinessRules.CheckRules();

      }

      private void Child_Fetch(int depth)
      {
        Depth = depth;
        LoadProperty(NameProperty, "Saturn");

        if (depth < 4)
        {
          LoadProperty(ChildProperty, Csla.DataPortal.FetchChild<SimpleBO>(Depth + 1));
          LoadProperty(ChildListProperty, Csla.DataPortal.FetchChild<SimpleBOList>(Depth + 1));
        }

        BusinessRules.CheckRules();
      }

      protected override void OnPropertyChanged(string propertyName)
      {
        EventDetails.Add(new EventDetail() { BO = enumBO.SimpleBO, Depth = Depth, Event = enumEvent.OnPropertyChanged, UniqueID = UniqueID, PropertyName = propertyName });
        base.OnPropertyChanged(propertyName);
      }

      protected override void OnChildChanged(ChildChangedEventArgs e)
      {
        EventDetails.Add(new EventDetail() { BO = enumBO.SimpleBO, Depth = Depth, Event = enumEvent.OnChildChanged, UniqueID = UniqueID, PropertyName = e.PropertyChangedArgs?.PropertyName });
        base.OnChildChanged(e);
      }

    }

    [Serializable]
    public class SimpleBOList : BusinessListBase<SimpleBOList, SimpleBO>
    {
      // I don't need these raising CSLA events
      public int Depth { get; private set; }
      public int UniqueID = NextUniqueID();

      private void Child_Fetch(int depth)
      {
        Depth = depth;
        Add(Csla.DataPortal.FetchChild<SimpleBO>(depth));
      }

      protected override void OnPropertyChanged(PropertyChangedEventArgs e)
      {
        EventDetails.Add(new EventDetail() { BO = enumBO.SimpleBOList, Depth = Depth, Event = enumEvent.OnPropertyChanged, UniqueID = UniqueID, PropertyName = e?.PropertyName });
        base.OnPropertyChanged(e);
      }

      protected override void OnChildChanged(ChildChangedEventArgs e)
      {
        EventDetails.Add(new EventDetail() { BO = enumBO.SimpleBOList, Depth = Depth, Event = enumEvent.OnChildChanged, UniqueID = UniqueID, PropertyName = e.PropertyChangedArgs?.PropertyName });
        base.OnChildChanged(e);
      }

    }

    private SimpleBO Fetch()
    {
      var result = Csla.DataPortal.Fetch<SimpleBO>();

      void HookEvents(SimpleBO bo)
      {
        bo.PropertyChanged += Result_PropertyChanged;
        bo.ChildChanged += Result_ChildChanged;

        if (bo.Child != null)
        {
          bo.ChildList.ChildChanged += ChildList_ChildChanged;

          HookEvents(bo.Child);
          bo.ChildList.ToList().ForEach(c => HookEvents(c));
        }

      }

      HookEvents(result);

      _SequenceID = 0;
      EventDetails.Clear();

      return result;

    }

    private void ChildList_ChildChanged(object sender, ChildChangedEventArgs e)
    {
      var list = sender as SimpleBOList ?? throw new ArgumentNullException("Not a SimpleBOList");
      EventDetails.Add(new EventDetail() { BO = enumBO.SimpleBOList, Depth = list[0].Depth, Event = enumEvent.ChildChanged, UniqueID = list.UniqueID, PropertyName = e.PropertyChangedArgs?.PropertyName });
    }

    private void Result_ChildChanged(object sender, ChildChangedEventArgs e)
    {
      var bo = sender as SimpleBO ?? throw new ArgumentNullException("Not a SimpleBO");
      EventDetails.Add(new EventDetail() { BO = enumBO.SimpleBO, Depth = bo.Depth, Event = enumEvent.ChildChanged, UniqueID = bo.UniqueID, PropertyName = e.PropertyChangedArgs?.PropertyName });
    }

    private void Result_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      var bo = sender as SimpleBO ?? throw new ArgumentNullException("Not a SimpleBO");
      EventDetails.Add(new EventDetail() { BO = enumBO.SimpleBO, Depth = bo.Depth, Event = enumEvent.PropertyChanged, UniqueID = bo.UniqueID, PropertyName = e.PropertyName });
    }


    [TestMethod]
    
    public void OptimizeChildChangedTests_Fetch()
    {
      var result = Fetch();

      Assert.IsNotNull(result.Child);
      Assert.IsNotNull(result.ChildList);

      Assert.IsNotNull(result.Child.Child);
      Assert.IsNotNull(result.Child.ChildList);

      Assert.IsNotNull(result.Child.Child.Child);
      Assert.IsNotNull(result.Child.Child.ChildList);

      Assert.IsTrue(result.IsValid);
      Assert.IsFalse(result.IsNew);
      Assert.IsFalse(result.IsDirty);

    }

    [TestMethod]
    
    public void OptimizeChildChangedTests_Name_Depth0()
    {
      var result = Fetch();

      result.Name = "Keith";

      WriteEventDetails();

      CheckBottomDepth(EventDetails, 0);

      Assert.AreEqual(0, EventDetails.Count);

    }

    [TestMethod]
    public void OptimizeChildChangedTests_Name_Depth1()
    {
      var result = Fetch();

      result.Child.Name = "Keith";

      WriteEventDetails();

      CheckMidDepth(EventDetails, 0);
      CheckBottomDepth(EventDetails, 1);

      Assert.AreEqual(0, EventDetails.Count);

    }


    [TestMethod]
    public void OptimizeChildChangedTests_Name_Depth2()
    {
      var result = Fetch();

      result.Child.Child.Name = "Keith";

      WriteEventDetails();

      CheckMidDepth(EventDetails, 0);
      CheckMidDepth(EventDetails, 1);
      CheckBottomDepth(EventDetails, 2);

      Assert.AreEqual(0, EventDetails.Count);

    }


    [TestMethod]
    public void OptimizeChildChangedTests_Name_Depth3()
    {
      var result = Fetch();

      _SequenceID = 0;
      EventDetails.Clear();

      result.Child.Child.Child.Name = "Keith";

      WriteEventDetails();

      CheckMidDepth(EventDetails, 0);
      CheckMidDepth(EventDetails, 1);
      CheckMidDepth(EventDetails, 2);
      CheckBottomDepth(EventDetails, 3);

      Assert.AreEqual(0, EventDetails.Count);

    }

    [TestMethod]
    
    public void OptimizeChildChangedTests_List_Name_Depth1()
    {
      var result = Fetch();

      result.ChildList[0].Name = "Keith";

      WriteEventDetails();

      CheckMidDepth(EventDetails, 0);
      CheckBottomDepthList(EventDetails, 1);

      Assert.AreEqual(0, EventDetails.Count);

    }


    [TestMethod]
    
    public void OptimizeChildChangedTests_List_Name_Depth2()
    {
      var result = Fetch();

      result.ChildList[0].ChildList[0].Name = "Keith";

      WriteEventDetails();

      CheckMidDepth(EventDetails, 0);
      CheckMidDepthList(EventDetails, 1);
      CheckBottomDepthList(EventDetails, 2);

      Assert.AreEqual(0, EventDetails.Count);

    }


    [TestMethod]
    public void OptimizeChildChangedTests_List_Name_Depth3()
    {
      var result = Fetch();

      _SequenceID = 0;
      EventDetails.Clear();

      result.ChildList[0].ChildList[0].ChildList[0].Name = "Keith";

      WriteEventDetails();

      CheckMidDepth(EventDetails, 0);
      CheckMidDepthList(EventDetails, 1);
      CheckMidDepthList(EventDetails, 2);
      CheckBottomDepthList(EventDetails, 3);

      Assert.AreEqual(0, EventDetails.Count);

    }

    private void WriteEventDetails()
    {
      if (EventDetails != null)
      {
        EventDetails.ForEach(ev => Debug.WriteLine(ev.ToString()));
        Debug.WriteLine("Name events");
        EventDetails.Where(ev => ev.PropertyName == "Name").ToList().ForEach(x => Debug.WriteLine(x.ToString()));
      }
    }

    private void CheckBottomDepth(List<EventDetail> list, int depth)
    {
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO && ev.Depth == depth && ev.Event == enumEvent.OnPropertyChanged && ev.PropertyName == "Name"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO && ev.Depth == depth && ev.Event == enumEvent.PropertyChanged && ev.PropertyName == "Name"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO && ev.Depth == depth && ev.Event == enumEvent.PropertyChanged && ev.PropertyName == "IsDirty"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO && ev.Depth == depth && ev.Event == enumEvent.PropertyChanged && ev.PropertyName == "IsSelfDirty"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO && ev.Depth == depth && ev.Event == enumEvent.PropertyChanged && ev.PropertyName == "IsValid"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO && ev.Depth == depth && ev.Event == enumEvent.PropertyChanged && ev.PropertyName == "IsSelfValid"));
      Assert.AreEqual(2, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO && ev.Depth == depth && ev.Event == enumEvent.PropertyChanged && ev.PropertyName == "IsSavable"));

    }

    private void CheckBottomDepthList(List<EventDetail> list, int depth)
    {
      CheckBottomDepth(list, depth);
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBOList && ev.Depth == depth && ev.Event == enumEvent.OnChildChanged && ev.PropertyName == "Name"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBOList && ev.Depth == depth && ev.Event == enumEvent.ChildChanged && ev.PropertyName == "Name"));
    }

    private void CheckMidDepth(List<EventDetail> list, int depth)
    {
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO &&  ev.Depth == depth && ev.Event == enumEvent.OnChildChanged && ev.PropertyName == "Name"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO &&  ev.Depth == depth && ev.Event == enumEvent.PropertyChanged && ev.PropertyName == "IsDirty"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO &&  ev.Depth == depth && ev.Event == enumEvent.PropertyChanged && ev.PropertyName == "IsValid"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO &&  ev.Depth == depth && ev.Event == enumEvent.PropertyChanged && ev.PropertyName == "IsSavable"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBO &&  ev.Depth == depth && ev.Event == enumEvent.ChildChanged && ev.PropertyName == "Name"));
    }

    private void CheckMidDepthList(List<EventDetail> list, int depth)
    {
      CheckMidDepth(list, depth);
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBOList && ev.Depth == depth && ev.Event == enumEvent.OnChildChanged && ev.PropertyName == "Name"));
      Assert.AreEqual(1, list.RemoveAll(ev => ev.BO == enumBO.SimpleBOList && ev.Depth == depth && ev.Event == enumEvent.ChildChanged && ev.PropertyName == "Name"));
    }

  }
}
