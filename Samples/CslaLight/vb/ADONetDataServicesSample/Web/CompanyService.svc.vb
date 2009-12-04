Imports System.Data.Services
Imports System.Linq
Imports System.ServiceModel.Web

Public Class CompanyService
  ' TODO: replace [[class name]] with your data class name
  Inherits DataService(Of CompanyEntities)

  ' This method is called only once to initialize service-wide policies.
  Public Shared Sub InitializeService(ByVal config As IDataServiceConfiguration)
    config.SetServiceOperationAccessRule("*", ServiceOperationRights.All)
    config.SetEntitySetAccessRule("*", EntitySetRights.All)
  End Sub

End Class
