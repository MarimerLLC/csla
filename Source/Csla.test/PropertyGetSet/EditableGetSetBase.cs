﻿//-----------------------------------------------------------------------
// <copyright file="EditableGetSetBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.PropertyGetSet
{
  [Serializable]
  public class EditableGetSetBase<T> : EditableGetSetTopBase<T> 
    where T: EditableGetSetBase<T>
  {
    private static int _dummy;

    public EditableGetSetBase()
    {
      _dummy += 0;
    }

    private static PropertyInfo<string> BaseProperty = RegisterProperty<string>(typeof(EditableGetSetBase<T>), new PropertyInfo<string>("Base", "Base"));
    public string Base
    {
      get { return GetProperty<string>(BaseProperty); }
      set { SetProperty<string>(BaseProperty, value); }
    }
  }

  [Serializable]
  public class EditableGetSetNFIBase<T> : EditableGetSetTopNFIBase<T>
    where T : EditableGetSetNFIBase<T>
  {
    public static PropertyInfo<string> BaseProperty = RegisterProperty<string>(new PropertyInfo<string>("Base", "Base"));
    public string Base
    {
      get { return GetProperty<string>(BaseProperty); }
      set { SetProperty<string>(BaseProperty, value); }
    }
  }
}