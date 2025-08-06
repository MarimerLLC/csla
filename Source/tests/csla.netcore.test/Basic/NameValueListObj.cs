//-----------------------------------------------------------------------
// <copyright file="NameValueListObj.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.Basic
{
  [Serializable]
  public class NameValueListObj : NameValueListBase<int, string>
  {
    [Fetch]
    protected void DataPortal_Fetch()
    {
      TestResults.Add("NameValueListObj", "Fetched");

      IsReadOnly = false;
      for (int i = 0; i < 10; i++)
      {
        Add(new NameValuePair(i, $"element_{i}"));
      }

      IsReadOnly = true;
    }
  }
}