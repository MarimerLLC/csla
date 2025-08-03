using Csla.Core;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class ParameterOverloadforSync
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }


    [TestMethod]
    public void ParameterOverloadforSyncTest()
    {
      var result = FetchValid();
      var res = FetchInValid();
      Assert.IsTrue(result);
      Assert.IsFalse(res);
    }

    private bool FetchValid()
    {
      IDataPortal<UserValidation> portal = _testDIContext.CreateDataPortal<UserValidation>();

      var userValidation = portal.Execute("admin", "admin");
      if (userValidation.IsValid)
      {
        return true;
      }
      return false;
    }

    private bool FetchInValid()
    {
      IDataPortal<UserValidation> portal = _testDIContext.CreateDataPortal<UserValidation>();

      var userValidation = portal.Execute("admin", "123");
      if (userValidation.IsValid)
      {
        return true;
      }
      return false;
    }

  }

  [Serializable]
  public class UserValidation : CommandBase<UserValidation>
  {
    public static readonly PropertyInfo<bool> IsValidProperty = RegisterProperty<bool>(nameof(IsValid));
    public bool IsValid
    {
      get => ReadProperty(IsValidProperty);
      private set => LoadProperty(IsValidProperty, value);
    }

    public static readonly PropertyInfo<MobileList<string>> RolesProperty = RegisterProperty<MobileList<string>>(nameof(Roles));
    public MobileList<string> Roles
    {
      get => ReadProperty(RolesProperty);
      private set => LoadProperty(RolesProperty, value);
    }


    [Execute]
    private void Execute(string username, string password)
    {
      var user = Fetch(username, password);
      IsValid = (user is not null);
      if (IsValid)
        Roles = [.. user.Roles];
    }

    public UserDto Fetch(string username, string password)
    {
      var result = (from r in MockDb.Users
                    where r.Username == username && r.Password == password
                    select new UserDto { Username = r.Username, Roles = r.Roles }).FirstOrDefault();
      return result;
    }

    #region MocDB
    public static class MockDb
    {
      public static List<RoleData> Roles { get; private set; }
      public static List<UserData> Users { get; private set; }

      static MockDb()
      {
        Roles = new List<RoleData>
        {
          new RoleData { Id = 1, Name = "Project manager", LastChanged = GetTimeStamp() },
          new RoleData { Id = 2, Name = "Developer", LastChanged = GetTimeStamp() },
          new RoleData { Id = 3, Name = "QA", LastChanged = GetTimeStamp() },
          new RoleData { Id = 4, Name = "Sponsor", LastChanged = GetTimeStamp() }
        };
        Users = new List<UserData>
        {
          new UserData { Username = "manager", Password = "manager", Roles = new string[] { "ProjectManager" }},
          new UserData { Username = "admin", Password = "admin", Roles = new string[] { "Administrator" }}
        };
      }

      private static long _lastTimeStamp = 1;

      public static byte[] GetTimeStamp()
      {
        var stamp = System.Threading.Interlocked.Add(ref _lastTimeStamp, 1);
        return System.Text.ASCIIEncoding.ASCII.GetBytes(stamp.ToString());
      }
    }

    #endregion MocDB

    #region Models

    public class UserData
    {
      public string Username { get; set; }
      public string Password { get; set; }
      public string[] Roles { get; set; }
    }
    public class UserDto
    {
      public string Username { get; set; }
      public string[] Roles { get; set; }
    }
    public class RoleData
    {
      public int Id { get; set; }
      public string Name { get; set; }
      public byte[] LastChanged { get; set; }
    }

    #endregion Models
  }



}
