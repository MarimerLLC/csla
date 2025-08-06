//-----------------------------------------------------------------------
// <copyright file="CustomMobileList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.Core;
using Csla.Serialization.Mobile;
using System.Xml;
using System.Text;
using System.IO;
using Csla.Serialization;
using serialization = System.Runtime.Serialization;

namespace cslalighttest.Serialization
{
  [Serializable]
  public class CustomMobileList : MobileBindingList<MockNonBusinessObject>
  {
		//protected override void OnSetState(SerializationInfo info)
		//{
		//  string xml = info.GetValue<string>("$items");
		//  using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
		//  {
		//    XmlReader reader = Csla.Serialization.Mobile.MobileFormatter.GetXmlReader(stream); // XmlReader.Create(stream);
		//    serialization.DataContractSerializer dcs = new serialization.DataContractSerializer(typeof(MockNonBusinessObject[]));
		//    MockNonBusinessObject[] items = (MockNonBusinessObject[])dcs.ReadObject(reader);
                
		//    foreach (MockNonBusinessObject mock in items)
		//      Add(mock);
		//  }

		//  base.OnSetState(info);
		//}

		//protected override void OnGetState(SerializationInfo info)
		//{
		//  using (MemoryStream stream = new MemoryStream())
		//  {
		//    XmlWriter writer = Csla.Serialization.Mobile.MobileFormatter.GetXmlWriter(stream); // XmlWriter.Create(stream);
		//    serialization.DataContractSerializer dcs = new serialization.DataContractSerializer(typeof(MockNonBusinessObject[]));
		//    dcs.WriteObject(writer, Items.ToArray());
		//    writer.Flush();

		//    byte[] buffer = stream.ToArray();
		//    string xml = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
		//    info.AddValue("$items", xml);
		//  }
		//  base.OnGetState(info);
		//}

    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
    }

    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
    }
  }
}