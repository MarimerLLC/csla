using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Threading;
using CSLA.Security;
using ProjectTracker.Library;

namespace PTServicecs
{
  public class ProjectTracker : System.Web.Services.WebService
  {
    public ProjectTracker()
    {
      //CODEGEN: This call is required by the ASP.NET Web Services Designer
      InitializeComponent();
    }

		#region Component Designer generated code
		
    //Required by the Web Services Designer 
    private IContainer components = null;
				
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if(disposing && components != null)
      {
        components.Dispose();
      }
      base.Dispose(disposing);		
    }
		
		#endregion

    #region Security

    public class CSLACredentials : SoapHeader
    {
      public string Username = string.Empty;
      public string Password = string.Empty;
    }

    public CSLACredentials Credentials = new CSLACredentials();

    public void Login()
    {
      if(Credentials.Username.Length == 0)
        throw new System.Security.SecurityException(
          "Valid credentials not provided");

      BusinessPrincipal.Login(
        Credentials.Username, Credentials.Password);

      System.Security.Principal.IPrincipal principal =
        Thread.CurrentPrincipal;

      if(principal.Identity.IsAuthenticated)
      {
        // the user is valid - set up the HttpContext
        HttpContext.Current.User = principal;
      }
      else
      {
        // the user is not valid, raise an error
        throw new System.Security.SecurityException("Invalid user or password");
      }
    }

    #endregion

    #region Data Structures

    public struct ProjectInfo
    {
      public string ID;
      public string Name;
      public string Started;
      public string Ended;
      public string Description;
      public ProjectResourceInfo [] Resources;
    }

    public struct ProjectResourceInfo
    {
      public string ResourceID;
      public string FirstName;
      public string LastName;
      public string Assigned;
      public string Role;
    }

    public struct ResourceInfo
    {
      public string ID;
      public string Name;
      public ResourceAssignmentInfo [] Assignments;
    }

    public struct ResourceAssignmentInfo
    {
      public string ProjectID;
      public string Name;
      public string Assigned;
      public string Role;
    }

    #endregion

    #region Projects

    [WebMethod(Description="Get a list of projects")]
    [SoapHeader("Credentials")]
    public ProjectInfo [] GetProjectList()
    {
      Login();

      ProjectList list = ProjectList.GetProjectList();
      ProjectInfo[] info = new ProjectInfo[list.Count];
      int index = 0;

      foreach(ProjectList.ProjectInfo project in list)
      {
        // ProjectList only returns 2 of our data fields
        info[index].ID = project.ID.ToString();
        info[index].Name = project.Name;
        index++;
      }
      return info;
    }

    [WebMethod(Description="Get detailed data for a specific project")]
    [SoapHeader("Credentials")]
    public ProjectInfo GetProject(string id)
    {
      Login();

      ProjectInfo info;
      Project project = Project.GetProject(new Guid(id));

      info.ID = project.ID.ToString();
      info.Name = project.Name;
      info.Started = project.Started;
      info.Ended = project.Ended;
      info.Description = project.Description;

      // load child objects
      info.Resources = new ProjectResourceInfo[project.Resources.Count];
      for(int idx = 0; idx < project.Resources.Count; idx++)
      {
        info.Resources[idx].ResourceID = project.Resources[idx].ResourceID;
        info.Resources[idx].FirstName = project.Resources[idx].FirstName;
        info.Resources[idx].LastName = project.Resources[idx].LastName;
        info.Resources[idx].Assigned = project.Resources[idx].Assigned;
        info.Resources[idx].Role = project.Resources[idx].Role;
      }
      return info;
    }

    [WebMethod(Description="Add or update a project " +
      "(only provide the ID field for an update operation)")]
    [SoapHeader("Credentials")]
    public string UpdateProject(ProjectInfo data)
    {

      Login();

      Project project;
      if(data.ID == null || data.ID.Length == 0)
      {
        // no id so this is a new project
        project = Project.NewProject();
      }
      else
      {
        // they provided an id so we are updating a project
        project = Project.GetProject(new Guid(data.ID));
      }

      if(data.Name != null)
        project.Name = data.Name;
      if(data.Description != null)
        project.Started = data.Started;
      if(data.Ended != null)
        project.Ended = data.Ended;
      if(data.Description != null)
        project.Description = data.Description;

      if(data.Resources != null)
      {
        // load child objects
        for(int idx = 0; idx < data.Resources.Length; idx++)
        {
          if(project.Resources.Contains(data.Resources[idx].ResourceID))
          {
            // update existing resource
            // of course only the role field is changable...
            project.Resources[data.Resources[idx].ResourceID].Role = 
              data.Resources[idx].Role;
          }

          else
          {
            // just add new resource
            project.Resources.Assign(
              data.Resources[idx].ResourceID, data.Resources[idx].Role);
          }
        }
      }

      project = (Project)project.Save();

      return project.ID.ToString();

    }

    [WebMethod(Description="Remove a project from the system")]
    [SoapHeader("Credentials")]
    public void DeleteProject(string projectID)
    {
      Login();

      Project.DeleteProject(new Guid(projectID));
    }

    #endregion

    #region Resources

    [WebMethod(Description="Get a list of resources")]
    [SoapHeader("Credentials")]
    public ResourceInfo[] GetResourceList()
    {
      Login();

      ResourceList list = ResourceList.GetResourceList();
      ResourceInfo[] info = new ResourceInfo[list.Count];
      int index = 0;

      foreach(ResourceList.ResourceInfo resource in list)
      {
        info[index].ID = resource.ID;
        info[index].Name = resource.Name;
        index++;
      }
      return info;
    }

    [WebMethod(Description="Get details for a specific resource")]
    [SoapHeader("Credentials")]
    public ResourceInfo GetResource(string id) 
    {
      Login();

      ResourceInfo info;
      Resource resource = Resource.GetResource(id);

      info.ID = resource.ID;
      info.Name = resource.LastName + ", " + resource.FirstName;

      // load child objects
      info.Assignments = new ResourceAssignmentInfo[resource.Assignments.Count];
      for(int idx = 0; idx < resource.Assignments.Count; idx++)
      {
        info.Assignments[idx].ProjectID = 
          resource.Assignments[idx].ProjectID.ToString();
        info.Assignments[idx].Name = resource.Assignments[idx].ProjectName;
        info.Assignments[idx].Assigned = resource.Assignments[idx].Assigned;
        info.Assignments[idx].Role = resource.Assignments[idx].Role;
      }
      return info;
    }

    [WebMethod(Description="Add or update a resource")]
    [SoapHeader("Credentials")]
    public void UpdateResource(ResourceInfo data)
    {
      Login();

      Resource resource;
      try
      {
        resource = Resource.GetResource(data.ID);
      }
      catch
      {
        // failed to retrieve resource, so this must be new
        resource = Resource.NewResource(data.ID);
      }

      string val = data.Name;
      resource.FirstName = val.Substring(val.IndexOf(",")+1);
      resource.LastName = val.Substring(0, val.IndexOf(","));

      if(data.Assignments != null)
      {
        // load child objects
        for(int idx = 0; idx < data.Assignments.Length; idx++)
        {
          if(resource.Assignments.Contains(new Guid(data.Assignments[idx].ProjectID)))
          {
            // update existing resource
            // of course only the role field is changable...
            resource.Assignments[idx].Role = data.Assignments[idx].Role;
          }                                                                                     
          else
          {
            // just add new resource
            resource.Assignments.AssignTo(
              new Guid(data.Assignments[idx].ProjectID), 
              data.Assignments[idx].Role);
          }
        }
      }
      resource = (Resource)resource.Save();
    }

    [WebMethod(Description="Remove a resource from the system")]
    [SoapHeader("Credentials")]
    public void DeleteResource(string resourceID)
    {
      Login();

      Resource.DeleteResource(resourceID);
    }

    #endregion
  }
}
