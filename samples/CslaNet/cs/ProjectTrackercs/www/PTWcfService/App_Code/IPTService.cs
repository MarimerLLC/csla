using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

[ServiceContract]
public interface IPTService
{
  [OperationContract]
  List<ProjectData> GetProjectList(ProjectListRequest request);
  [OperationContract]
  ProjectData GetProject(ProjectRequest request);
  [OperationContract]
  ProjectData AddProject(AddProjectRequest request);
  [OperationContract]
  ProjectData UpdateProject(UpdateProjectRequest request);
  [OperationContract]
  List<RoleData> GetRoles(RoleRequest request);
}
