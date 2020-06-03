using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using UnitDriven;

namespace Csla.Test.SourceLink
{
  [TestClass]
  public class PdbContentTests :TestBase
  {
    private bool BufferContainsData(byte[] buffer, byte[] data)
    {
      for (int i = 0; i <= (buffer.Length - data.Length); i++)
      {
        if (buffer[i] == data[0])
        {
          int j;
          for (j = 1; j < data.Length && buffer[i + j] == data[j]; j++) ;
          if (j == data.Length)
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Search all symbol files in the release folder for the string "https://raw.githubusercontent.com",
    /// which is injected by Source Link as the location for github hosted projects. Should be a safe
    /// assumption that this string will only appear in a pdb if Source Link is enabled.
    /// </summary>
    [TestMethod]
    [Ignore]
    public void SearchPdbForString()
    {
      byte[] searchBytes = Encoding.UTF8.GetBytes("https://raw.githubusercontent.com");

      var symbolFiles = Directory.GetFiles("..\\..\\..\\..\\Bin\\Release", "*.pdb", SearchOption.AllDirectories);
      foreach (var path in symbolFiles)
      {
        byte[] fileData = File.ReadAllBytes(path);
        Assert.IsTrue(BufferContainsData(fileData, searchBytes), $"{path} doesn't contain Source Link information!");
      }
    }
  }
}
