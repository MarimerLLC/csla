using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.DiffGram;

namespace Test.Library
{
  [Serializable]
  public class OrderEdit : DiffBase<OrderEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(c => c.CustomerName);
    public string CustomerName
    {
      get { return GetProperty(CustomerNameProperty); }
      set { SetProperty(CustomerNameProperty, value); }
    }

    public static readonly PropertyInfo<LineItems> LineItemsProperty = RegisterProperty<LineItems>(c => c.LineItems);
    public LineItems LineItems
    {
      get { return GetProperty(LineItemsProperty); }
    }

    public static OrderEdit GetObject(int id)
    {
      return DataPortal.Fetch<OrderEdit>(
        new SingleCriteria<OrderEdit, int>(id));
    }

    protected override void DataPortal_Create()
    {
      LoadProperty(LineItemsProperty, DataPortal.CreateChild<LineItems>());
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch(SingleCriteria<OrderEdit, int> criteria)
    {
      using (BypassPropertyChecks)
      {
        Id = criteria.Value;
        CustomerName = "Customer " + Id;
      }
      LoadProperty(LineItemsProperty, DataPortal.FetchChild<LineItems>(criteria.Value));
    }

    protected override void ExportTo(Csla.DiffGram.DataItem dto)
    {
      dto.DataFields.Add(new Csla.DiffGram.DataField { Name = "Id", Value = Id });
      dto.DataFields.Add(new Csla.DiffGram.DataField { Name = "CustomerName", Value = CustomerName });
    }

    protected override void ImportFrom(Csla.DiffGram.DataItem dto)
    {
      // update changed values here
    }

    #region Save

    public new OrderEdit Save()
    {
      var dg = new Csla.DiffGram.DiffGramGenerator();
      var dto = dg.GenerateGraph(this);
      var cmd = new SaveOrder { DiffGram = dto };
      cmd = DataPortal.Execute<SaveOrder>(cmd);
      dg.IntegrateGraph(this, cmd.DiffGram);
      return this;
    }

    [Serializable]
    private class SaveOrder : CommandBase<SaveOrder>
    {
      public Csla.DiffGram.DataItem DiffGram { get; set; }

      protected override void DataPortal_Execute()
      {
        // do insert/update/delete operations on entire diffgram
      }
    }

    #endregion
  }
}
