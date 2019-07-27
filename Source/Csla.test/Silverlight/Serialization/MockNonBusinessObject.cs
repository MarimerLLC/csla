//-----------------------------------------------------------------------
// <copyright file="MockNonBusinessObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace cslalighttest.Serialization
{
  [DataContract]
  [KnownType(typeof(MockNonBusinessObject2))]
  public class MockNonBusinessObject
  {
    [DataMember]
    public string Member { get; set; }

    [DataMember]
    public MockNonBusinessObject2 Child { get; set; }
  }
  
  [DataContract]
  public class MockNonBusinessObject2
  {
    [DataMember]
    public int Id { get; set; }
  }
}