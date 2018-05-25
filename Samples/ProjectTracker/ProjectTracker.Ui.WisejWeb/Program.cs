using CslaContrib.WisejWeb;

namespace PTWisej
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
      Csla.ApplicationContext.ContextManager = new ApplicationContextManager();

      var page = new MainPage();
      page.Show();
    }
  }
}