using System;
using Csla;
using Csla.DiffGram;

namespace Test.Library
{
  [Serializable]
  public class OrderEdit : DiffBase<OrderEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(nameof(CustomerName));
    public string CustomerName
    {
      get => GetProperty(CustomerNameProperty);
      set => SetProperty(CustomerNameProperty, value);
    }

    public static readonly PropertyInfo<LineItems> LineItemsProperty = RegisterProperty<LineItems>(nameof(LineItems));
    public LineItems LineItems
    {
      get => GetProperty(LineItemsProperty);
      private set => LoadProperty(LineItemsProperty, value);
    }

    public static OrderEdit GetObject(int id)
    {
      return DataPortal.Fetch<OrderEdit>(id);
    }

    [Create]
    private void Create()
    {
      LoadProperty(LineItemsProperty, DataPortal.CreateChild<LineItems>());
      base.DataPortal_Create();
    }

    [Fetch]
    private void Fetch(int id)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        CustomerName = "Customer " + Id;
      }
      LoadProperty(LineItemsProperty, DataPortal.FetchChild<LineItems>(id));
    }

    protected override void ExportTo(Csla.DiffGram.DataItem dto)
    {
      dto.DataFields.Add(new DataField { Name = "Id", Value = Id });
      dto.DataFields.Add(new DataField { Name = "CustomerName", Value = CustomerName });
    }

    protected override void ImportFrom(DataItem dto)
    {
      // update changed values here
    }

    public new OrderEdit Save()
    {
      var dg = new Csla.DiffGram.DiffGramGenerator();
      var dto = dg.GenerateGraph(this);
      var cmd = DataPortal.Create<SaveOrder>(dto);
      cmd = DataPortal.Execute(cmd);
      dg.IntegrateGraph(this, cmd.DiffGram);
      return this;
    }

    [Serializable]
    private class SaveOrder : CommandBase<SaveOrder>
    {
      public static readonly PropertyInfo<DataItem> DiffGramProperty = RegisterProperty<DataItem>(nameof(DiffGram));
      public DataItem DiffGram
      {
        get => ReadProperty(DiffGramProperty);
        private set => LoadProperty(DiffGramProperty, value);
      }

      [Create]
      private void Create(DataItem dto)
      {
        DiffGram = dto;
      }

      [Execute]
      private void Execute()
      {
        // do insert/update/delete operations on entire diffgram
      }
    }
  }
}
