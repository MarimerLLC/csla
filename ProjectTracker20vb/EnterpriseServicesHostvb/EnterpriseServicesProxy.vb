Public Class EnterpriseServicesProxy
  Inherits Csla.DataPortalClient.EnterpriseServicesProxy

  Protected Overrides Function GetServerObject() As Csla.Server.Hosts.EnterpriseServicesPortal

    Return New EnterpriseServicesPortal

  End Function

End Class
