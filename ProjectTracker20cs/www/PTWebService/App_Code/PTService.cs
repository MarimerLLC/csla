using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using ProjectTracker.Library;


/// <summary>
/// Summary description for PTService
/// </summary>
[WebService(Namespace = "http://ws.lhotka.net/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class PTService : System.Web.Services.WebService
{

  /// <summary>
  /// Credentials for custom authentication.
  /// </summary>
  public CslaCredentials Credentials = new CslaCredentials();

  #region Projects

  [WebMethod(Description="Get a list of projects")]
  public ProjectInfo[] GetProjectList()
  {
    // anonymous access allowed
    Security.UseAnonymous();

    try
    {
      ProjectList list = ProjectList.GetProjectList();
      List<ProjectInfo> result = new List<ProjectInfo>();
      foreach (ProjectList.ProjectInfo item in list)
      {
        ProjectInfo info = new ProjectInfo();
        Csla.Data.DataMapper.Map(item, info);
        result.Add(info);
      }
      return result.ToArray();
    }
    catch (Csla.DataPortalException ex)
    {
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  [WebMethod(Description="Get a project")]
  public ProjectInfo GetProject(string id)
  {
    // anonymous access allowed
    Security.UseAnonymous();

    try
    {
      Project proj = Project.GetProject(new Guid(id));
      ProjectInfo result = new ProjectInfo();
      Csla.Data.DataMapper.Map(proj, result, "Resources");
      foreach (ProjectResource resource in proj.Resources)
      {
        ProjectResourceInfo info = new ProjectResourceInfo();
        Csla.Data.DataMapper.Map(resource, info, "FullName");
        result.AddResource(info);
      }
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  [WebMethod(Description="Add a project")]
  [SoapHeader("Credentials")]
  public ProjectInfo AddProject(string name, string started, string ended, string description)
  {
    // user credentials required
    Security.Login(Credentials);

    try
    {
      Project proj = Project.NewProject();
      proj.Name = name;
      proj.Started = started;
      proj.Ended = ended;
      proj.Description = description;
      proj = proj.Save();

      ProjectInfo result = new ProjectInfo();
      Csla.Data.DataMapper.Map(proj, result, "Resources");
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  [WebMethod(Description = "Edit a project")]
  [SoapHeader("Credentials")]
  public ProjectInfo EditProject(Guid id, string name,
    string started, string ended,
    string description)
  {
    // user credentials required
    Security.Login(Credentials);

    try
    {
      Project proj = Project.GetProject(id);
      proj.Name = name;
      proj.Started = started;
      proj.Ended = ended;
      proj.Description = description;
      proj = proj.Save();

      ProjectInfo result = new ProjectInfo();
      Csla.Data.DataMapper.Map(proj, result, "Resources");
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  #endregion

  #region Resources

  [WebMethod(Description="Get a list of resources")]
  public ResourceInfo[] GetResourceList()
  {
    // anonymous access allowed
    Security.UseAnonymous();

    try
    {
      ResourceList list = ResourceList.GetResourceList();
      List<ResourceInfo> result = new List<ResourceInfo>();
      foreach (ResourceList.ResourceInfo item in list)
      {
        ResourceInfo info = new ResourceInfo();
        Csla.Data.DataMapper.Map(item, info);
        result.Add(info);
      }
      return result.ToArray();
    }
    catch (Csla.DataPortalException ex)
    {
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  [WebMethod(Description="Get a resource")]
  public ResourceInfo GetResource(int id)
  {
    // anonymous access allowed
    Security.UseAnonymous();

    try
    {
      Resource res = Resource.GetResource(id);
      ResourceInfo result = new ResourceInfo();
      result.Id = res.Id;
      result.Name = string.Format("{1}, {0}", res.FirstName, res.LastName);
      foreach (ResourceAssignment resource in res.Assignments)
      {
        ResourceAssignmentInfo info = new ResourceAssignmentInfo();
        Csla.Data.DataMapper.Map(resource, info);
        result.AddProject(info);
      }
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  [WebMethod(Description="Change a resource's name")]
  [SoapHeader("Credentials")]
  public ResourceInfo ChangeResourceName(int id,
    string firstName, string lastName)
  {
     // user credentials required.
    Security.Login(Credentials);

    try
    {
      Resource res = Resource.GetResource(id);
      res.FirstName = firstName;
      res.LastName = lastName;
      res = res.Save();

      ResourceInfo result = new ResourceInfo();
      result.Id = res.Id;
      result.Name = string.Format("{1}, {0}", res.FirstName, res.LastName);
      return result;
    }
    catch (Csla.DataPortalException ex)
    {
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  #endregion

  #region Assign Resource to Project
  
  [WebMethod(Description="Assign resource to a project")]
  [SoapHeader("Credentials")]
  public void AssignResource(int resourceId, Guid projectId)
  {
    // user credentials required
    Security.Login(Credentials);

    try
    {
      Resource res = Resource.GetResource(resourceId);
      res.Assignments.AssignTo(projectId);
      res.Save();
    }
    catch (Csla.DataPortalException ex)
    {
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  #endregion

  #region Roles

  [WebMethod(Description="Get a list of roles")]
  public RoleInfo[] GetRoles()
  {
    // anonymous access allowed
    Security.UseAnonymous();

    try
    {
      RoleList list = RoleList.GetList();
      List<RoleInfo> result = new List<RoleInfo>();
      foreach (RoleList.NameValuePair role in list)
      {
        RoleInfo info = new RoleInfo();
        info.Id = role.Key;
        info.Name = role.Value;
        result.Add(info);
      }
      return result.ToArray();
    }
    catch (Csla.DataPortalException ex)
    {
      throw ex.BusinessException;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  #endregion
}

