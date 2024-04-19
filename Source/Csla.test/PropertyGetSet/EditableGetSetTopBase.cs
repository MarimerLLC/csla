//-----------------------------------------------------------------------
// <copyright file="EditableGetSetTopBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.PropertyGetSet
{
  [Serializable]
  public class EditableGetSetTopBase<T> : BusinessBase<T> 
    where T: EditableGetSetTopBase<T>
  {
    private static int _dummy;

    public EditableGetSetTopBase()
    {
      _dummy += 0;
    }

    private static PropertyInfo<string> TopBaseProperty = RegisterProperty<string>(new PropertyInfo<string>("TopBase", "TopBase"));
    public string TopBase
    {
      get { return GetProperty<string>(TopBaseProperty); }
      set { SetProperty<string>(TopBaseProperty, value); }
    }
  }

  [Serializable]
  public class EditableGetSetTopNFIBase<T> : BusinessBase<T>
    where T : EditableGetSetTopNFIBase<T>
  {
    public EditableGetSetTopNFIBase()
    {
    }

    public static PropertyInfo<string> TopBaseProperty = RegisterProperty<string>(new PropertyInfo<string>("TopBase", "TopBase"));
    public string TopBase
    {
      get { return GetProperty<string>(TopBaseProperty); }
      set { SetProperty<string>(TopBaseProperty, value); }
    }
  }
}