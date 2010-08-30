using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Serialization;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.LineItemFactory, DataAccess")]
  public class LineItem : BusinessBase<LineItem>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    [Range(1, 9999)]
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

#if !SILVERLIGHT
    private LineItem()
    { }
#endif
  }
}
