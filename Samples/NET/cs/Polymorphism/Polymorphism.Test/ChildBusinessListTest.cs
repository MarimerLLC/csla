using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Csla.Serialization.Mobile;
using Polymorphism.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Polymorphism.Test
{   
    /// <summary>
    ///This is a test class for ChildBusinessListTest and is intended
    ///to contain all ChildBusinessListTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ChildBusinessListTest
  {
    [TestInitialize]
    public void Initialize()
    {
      if (!System.IO.Directory.Exists("c:\\temp"))
        System.IO.Directory.CreateDirectory("c:\\temp");
    }

    /// <summary>
    ///A test for GetEditableRootList
    ///</summary>
    [TestMethod()]
    public void GetEditableRootListTest()
    {
      var list = ChildBusinessList.GetEditableRootList(1);

      Assert.AreEqual(typeof(ChildType1), list[0].GetType());
      Assert.AreEqual(typeof(ChildType2), list[1].GetType());

      list.BeginEdit();
      list.Add(new ChildType2(3, "Test", "UK"));
      list.CancelEdit();

      Assert.AreEqual(2, list.Count);
    }

    [TestMethod()]
    public void SerializeToXml()
    {
      ChildBusinessList result;
      var list = ChildBusinessList.GetEditableRootList(1);

      Assert.AreEqual(typeof(ChildType1), list[0].GetType());
      Assert.AreEqual(typeof(ChildType2), list[1].GetType());

      System.IO.File.Delete("c:\\temp\\test1.xml");

			var serialized = MobileFormatter.SerializeToDTO(list);
			var writer = new CslaXmlWriter();
      var streamOut = new System.IO.FileStream("c:\\temp\\test1.xml", FileMode.OpenOrCreate);
      writer.Write(streamOut, serialized);
      streamOut.Close();

      Thread.Sleep(500);

      var streamIn = new System.IO.FileStream("c:\\temp\\test1.xml", FileMode.Open);
			var reader = new CslaXmlReader();
			var deserialized = reader.Read(streamIn);
   		result = (ChildBusinessList)MobileFormatter.DeserializeFromDTO(deserialized);
		
      Assert.AreEqual(list.Count, result.Count);
      var c1 = (ChildType1) result[0];
      Assert.AreEqual("MVP", c1.Group);
      var c2 = (ChildType2) result[1];
      Assert.AreEqual("Norway", c2.Address);
    }


    [TestMethod()]
    public void SerializeToXmlBinary()
    {
      ChildBusinessList result;
      var list = ChildBusinessList.GetEditableRootList(1);

      System.IO.File.Delete("c:\\temp\\test2.xml");

      Assert.AreEqual(typeof(ChildType1), list[0].GetType());
      Assert.AreEqual(typeof(ChildType2), list[1].GetType());

      var serialized = MobileFormatter.SerializeToDTO(list);
      var writer = new CslaXmlBinaryWriter();
      var streamOut = new System.IO.FileStream("c:\\temp\\test2.xml", FileMode.OpenOrCreate);
      writer.Write(streamOut, serialized);
      streamOut.Close();

      Thread.Sleep(500);

      var streamIn = new System.IO.FileStream("c:\\temp\\test2.xml", FileMode.Open);
      var reader = new CslaXmlBinaryReader();
      var deserialized = reader.Read(streamIn);
      result = (ChildBusinessList)MobileFormatter.DeserializeFromDTO(deserialized);

      Assert.AreEqual(list.Count, result.Count);
    }

    [TestMethod()]
    public void SerializeToBinary()
    {
      ChildBusinessList result;
      var list = ChildBusinessList.GetEditableRootList(1);

      System.IO.File.Delete("c:\\temp\\test3.xml");

      Assert.AreEqual(typeof(ChildType1), list[0].GetType());
      Assert.AreEqual(typeof(ChildType2), list[1].GetType());

      var serialized = MobileFormatter.SerializeToDTO(list);
      var writer = new CslaBinaryWriter();
      var streamOut = new System.IO.FileStream("c:\\temp\\test3.xml", FileMode.OpenOrCreate);
      writer.Write(streamOut, serialized);
      streamOut.Close();

      Thread.Sleep(500);

      var streamIn = new System.IO.FileStream("c:\\temp\\test3.xml", FileMode.Open);
      var reader = new CslaBinaryReader();
      var deserialized = reader.Read(streamIn);
      result = (ChildBusinessList)MobileFormatter.DeserializeFromDTO(deserialized);

      Assert.AreEqual(list.Count, result.Count);
    }
  }
}
