//-----------------------------------------------------------------------
// <copyright file="nonSerializableEventHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.Serialization
{
  public class nonSerializableEventHandler
  {
    private int _count;

    public void Reg(Core.BusinessBase obj)
    {
      obj.PropertyChanged += obj_PropertyChanged;
    }

    public void obj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      _count += 1;
      Console.WriteLine(_count.ToString());
      TestResults.AddOrOverwrite("PropertyChangedFiredCount", _count.ToString());
    }
  }
}