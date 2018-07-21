using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace ConsoleApplication1
{
  class Program
  {
    static void Main(string[] args)
    {
      var obj = DataPortal.Fetch<Root>();

      Console.WriteLine("Serialized size: {0}", SerializedSize(obj));

      var start = DateTime.Now;
      var data = Serialize(obj);
      Console.WriteLine("Serialized in: {0}", DateTime.Now - start);

      start = DateTime.Now;
      var tmp = DeSerialize(data);
      Console.WriteLine("Deserialized in: {0}", DateTime.Now - start);

      Console.ReadLine();
    }

    private static long SerializedSize(object obj)
    {
      var buffer = new System.IO.MemoryStream();
      var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      formatter.Serialize(buffer, obj);
      return buffer.Length;
    }

    private static byte[] Serialize(object obj)
    {
      var buffer = new System.IO.MemoryStream();
      var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      formatter.Serialize(buffer, obj);
      return buffer.ToArray();
    }

    private static object DeSerialize(byte[] data)
    {
      var buffer = new System.IO.MemoryStream(data);
      var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      return formatter.Deserialize(buffer);
    }
  }

  [Serializable]
  public class Root : BusinessBase<Root>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<ChildList> ChildListProperty = RegisterProperty<ChildList>(c => c.ChildList);
    public ChildList ChildList
    {
      get { return GetProperty(ChildListProperty); }
      private set { LoadProperty(ChildListProperty, value); }
    }

    private void DataPortal_Fetch()
    {
      Id = 123;
      Name = "Rockford Lhotka";
      ChildList = DataPortal.FetchChild<ChildList>();
    }

    protected override void DataPortal_Insert()
    {
      FieldManager.UpdateChildren();
    }

    protected override void DataPortal_Update()
    {
      FieldManager.UpdateChildren();
    }

    protected override void DataPortal_DeleteSelf()
    {
    }
  }

  [Serializable]
  public class ChildList : BusinessListBase<ChildList, Child>
  {
    private void Child_Fetch()
    {
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());

      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());

      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());

      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());

      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());

      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());

      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());

      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
      Add(DataPortal.FetchChild<Child>());
    }
  }

  [Serializable]
  public class Child : BusinessBase<Child>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private void Child_Fetch()
    {
      Id = 222;
      Name = "Benjamin Franklin";
    }

    private void Child_Insert()
    {
    }

    private void Child_Update()
    {
    }

    private void Child_DeleteSelf()
    {
    }
  }
}
