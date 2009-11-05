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
    private static PropertyInfo<int> LineProperty = RegisterProperty<int>(c => c.Line);
    public int Line
    {
      get { return GetProperty(LineProperty); }
      set { SetProperty(LineProperty, value); }
    }

    private static PropertyInfo<string> ProductNameProperty = RegisterProperty<string>(c => c.ProductName);
    public string ProductName
    {
      get { return GetProperty(ProductNameProperty); }
      set { SetProperty(ProductNameProperty, value); }
    }

    private void Child_Fetch(int id, int line)
    {
      using (BypassPropertyChecks)
      {
        Line = line;
        ProductName = "Product " + id;
      }
    }

    protected override void ExportTo(Csla.DiffGram.DataItem dto)
    {
      dto.DataFields.Add(new Csla.DiffGram.DataField { Name="Line", Value=Line });
      dto.DataFields.Add(new Csla.DiffGram.DataField { Name = "ProductName", Value = ProductName });
    }

    protected override void ImportFrom(Csla.DiffGram.DataItem dto)
    {
      // update changed values here
    }
  }
}
