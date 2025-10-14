//-----------------------------------------------------------------------
// <copyright file="MockReadOnlyList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Net;
using System.Windows;
#if !__ANDROID__
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endif
using Csla;
using Csla.Serialization;

namespace cslalighttest.Serialization
{
  [Serializable]
  public class MockReadOnlyList : ReadOnlyBindingListBase<MockReadOnlyList, MockReadOnly>
  {
    public MockReadOnlyList() { }

    public MockReadOnlyList(params MockReadOnly[] items)
    {
      IsReadOnly = false;
      AddRange(items);
      IsReadOnly = true;
    }
  }
}