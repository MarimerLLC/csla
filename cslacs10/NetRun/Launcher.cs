using System;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;

namespace NetRun
{
  public class Launcher : MarshalByRefObject
  {
    string _appURL;
    string _appDir;
    string _appName;

    bool _groupExisted;

    public void RunApp(string appURL)
    {
      // before we do anything, invoke the workaround
      // for the serialization bug
      SerializationWorkaround();

      try
      {
        // get and parse the URL for the app we are
        // launching
        _appURL = appURL;
        _appDir = AppMain.GetAppDirectory(_appURL);
        _appName = AppMain.GetAppName(_appURL);

        // TODO: MAKE SURE TO TIGHTEN SECURITY BEFORE USING!!!!
        // see http://www.lhotka.net/Articles.aspx?id=2f5a8115-b425-4aa1-bae2-b8f80766ecb3
        SetSecurity();

        // load the assembly into our AppDomain
        Assembly asm = Assembly.LoadFrom(appURL);

        // run the program by invoking its entry point
        asm.EntryPoint.Invoke(asm.EntryPoint, null);
      }
      finally
      {
        RemoveSecurity();
      }
    }

    #region Serialization bug workaround

    private void SerializationWorkaround()
    {
      // hook up the AssemblyResolve
      // event so deep serialization works properly
      // this is a workaround for a bug in the .NET runtime
      AppDomain.CurrentDomain.AssemblyResolve += 
        new System.ResolveEventHandler(ResolveEventHandler);
    }

    private Assembly ResolveEventHandler(object sender, ResolveEventArgs e)
    {
      // get a list of all the assemblies loaded in our appdomain
      Assembly [] list = AppDomain.CurrentDomain.GetAssemblies();

      // search the list to find the assemby that was not found automatically
      // and return the assembly from the list
      foreach(Assembly asm in list)
        if(asm.FullName == e.Name)
          return asm;

      // we didn't find it either, so return null
      return null;
    }

    #endregion

    #region SetSecurity to FullTrust

    private void SetSecurity()
    {
      System.Collections.IEnumerator ph = null;
      System.Security.Policy.PolicyLevel pl = null;
      bool found = false;

      // retrieve the security policy hierarchy
      ph = SecurityManager.PolicyHierarchy();

      // loop through to find the Machine level sub-tree
      while(ph.MoveNext())
      {
        pl = (PolicyLevel)ph.Current;
        if(pl.Label == "Machine")
        {
          found = true;
          break;
        }
      }

      if(found)
      {
        // see if the codegroup for this app already exists
        // as a machine-level entry
        foreach(CodeGroup cg in pl.RootCodeGroup.Children)
        {
          if(cg.Name == _appName)
          {
            // codegroup already exists
            // we assume it is set to a valid
            // permission level
            _groupExisted = true;
            return;
          }
        }

        // the codegroup doesn't already exist, so 
        // we'll add a url group with FullTrust
        _groupExisted = false;
        UnionCodeGroup ucg = 
          new UnionCodeGroup(new UrlMembershipCondition(_appDir + "/*"), 
          new PolicyStatement(new NamedPermissionSet("FullTrust")));
        ucg.Description = "Temporary entry for " + _appURL;
        ucg.Name = _appName;
        pl.RootCodeGroup.AddChild(ucg);
        SecurityManager.SavePolicy();
      }
    }

    #endregion

    #region RemoveSecurity

    private void RemoveSecurity()
    {
      // if the group existed before NetRun was used
      // we want to leave the group intact, so we
      // can just exit
      if(_groupExisted) return;

      // on the other hand, if the group didn't already
      // exist then we need to remove it now that
      // the business application is closed
      System.Collections.IEnumerator ph = null;
      System.Security.Policy.PolicyLevel pl = null;
      bool found = false;

      // retrieve the security policy hierarchy
      ph = SecurityManager.PolicyHierarchy();

      // loop through to find the Machine level sub-tree
      while(ph.MoveNext())
      {
        pl = (PolicyLevel)ph.Current;
        if(pl.Label == "Machine")
        {
          found = true;
          break;
        }
      }

      if(found)
      {
        // see if the codegroup for this app exists
        // as a machine-level entry
        foreach(CodeGroup cg in pl.RootCodeGroup.Children)
        {
          if(cg.Name == _appName)
          {
            // codegroup exits - remove it
            pl.RootCodeGroup.RemoveChild(cg);
            SecurityManager.SavePolicy();
            break;
          }
        }
      }
    }

    #endregion

  }
}
