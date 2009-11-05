using System;
using System.Collections.Generic;
using System.Text;
using Csla;
namespace Csla.Test.CslaDataProvider
{
  [Serializable]
  public class ProviderList : BusinessListBase<ProviderList, ProviderChild>
  {
    public ProviderList()
    {
      AllowEdit = true;
      AllowNew = true;
      AllowRemove = true;
    }

    public static ProviderList GetList()
    {
      ProviderList returnValue = new ProviderList();
      returnValue.Add(new ProviderChild(true));
      returnValue.Add(new ProviderChild(true));
      return returnValue;
    }

    protected override object AddNewCore()
    {
      ProviderChild child = new ProviderChild();
      Add(child);
      return child;
    }

    //error testing
    public override ProviderList Save()
    {
      throw new Exception();
    }

  }

  [Serializable]
  public class ProviderChild : BusinessBase<ProviderChild>
  {
    int _data;
    public int Data
    {
      get
      {
        CanReadProperty(true);
        return _data;
      }

      set
      {
        CanWriteProperty(true);
        if (!_data.Equals(value))
        {
          _data = value;
          PropertyHasChanged();
        }
      }
    }


    public ProviderChild()
    {
      MarkAsChild();
    }

    public ProviderChild(bool makeOld)
    {
      MarkAsChild();
      if (makeOld)
        MarkOld();
    }

    public void MarkItOld()
    {
      MarkOld();
    }
  }
}
