using System;
using System.Runtime.Serialization;
using System.ServiceModel;

[ServiceContract]
public interface IPTService
{
  [OperationContract]
  ProjectData[] GetProjectList();
  [OperationContract]
  ProjectData GetProject(ProjectRequest request);
}
