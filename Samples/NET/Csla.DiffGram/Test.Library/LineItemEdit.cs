using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.DiffGram;

namespace Test.Library
{
  [Serializable]
  public class LineItemEdit : DiffBase<LineItemEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<string> ProductNameProperty = RegisterProperty<string>(nameof(ProductName));
    public string ProductName
    {
      get => GetProperty(ProductNameProperty);
      set => SetProperty(ProductNameProperty, value);
    }

    [FetchChild]
    private void Fetch(int id, int line)
    {
      using (BypassPropertyChecks)
      {
        Id = line;
        ProductName = "Product " + id + line;
      }
    }

    protected override void ExportTo(Csla.DiffGram.DataItem dto)
    {
      dto.DataFields.Add(new Csla.DiffGram.DataField { Name="Id", Value=Id });
      dto.DataFields.Add(new Csla.DiffGram.DataField { Name = "ProductName", Value = ProductName });
    }

    protected override void ImportFrom(Csla.DiffGram.DataItem dto)
    {
      // update changed values here
    }
  }
}
