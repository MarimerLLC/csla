using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess
{
  public static class MockData
  {
    public static List<Data> Data;

    static MockData()
    {
      Data = new List<Data>();
      var rnd = new Random(1234);
      for (int i = 0; i < 100; i++)
        Data.Add(new Data { Id = rnd.Next(1, 10000), Name = "abc" });
    }
  }

  public class Data
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }
}
