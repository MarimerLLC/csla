Imports Microsoft.VisualBasic
Imports System
Imports System.Net
Imports Csla.Server.Hosts

Namespace Sample.Business.Compression
  Public Class CompressedHost
    Inherits Csla.Server.Hosts.Silverlight.WcfPortal
    Protected Overrides Function ConvertRequest(ByVal request As Csla.Server.Hosts.Silverlight.CriteriaRequest) As Csla.Server.Hosts.Silverlight.CriteriaRequest
      Dim returnValue As New Csla.Server.Hosts.Silverlight.CriteriaRequest()
      returnValue.ClientContext = CompressionUtility.Decompress(request.ClientContext)
      returnValue.GlobalContext = CompressionUtility.Decompress(request.GlobalContext)
      If request.CriteriaData IsNot Nothing Then
        returnValue.CriteriaData = CompressionUtility.Decompress(request.CriteriaData)
      End If

      returnValue.Principal = CompressionUtility.Decompress(request.Principal)
      returnValue.TypeName = request.TypeName
      Return returnValue
    End Function

    Protected Overrides Function ConvertRequest(ByVal request As Csla.Server.Hosts.Silverlight.UpdateRequest) As Csla.Server.Hosts.Silverlight.UpdateRequest
      Dim returnValue As New Csla.Server.Hosts.Silverlight.UpdateRequest()
      returnValue.ClientContext = CompressionUtility.Decompress(request.ClientContext)
      returnValue.GlobalContext = CompressionUtility.Decompress(request.GlobalContext)
      returnValue.ObjectData = CompressionUtility.Decompress(request.ObjectData)
      returnValue.Principal = CompressionUtility.Decompress(request.Principal)
      Return returnValue
    End Function

    Protected Overrides Function ConvertResponse(ByVal response As Csla.Server.Hosts.Silverlight.WcfResponse) As Csla.Server.Hosts.Silverlight.WcfResponse
      Dim returnValue As New Csla.Server.Hosts.Silverlight.WcfResponse()
      returnValue.GlobalContext = CompressionUtility.Compress(response.GlobalContext)
      returnValue.ObjectData = CompressionUtility.Compress(response.ObjectData)
      returnValue.ErrorData = response.ErrorData
      Return returnValue
    End Function
  End Class
End Namespace
