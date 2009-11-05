Imports Microsoft.VisualBasic
Imports System
Imports System.Net
Imports Csla.DataPortalClient

Namespace Sample.Business.Compression
  Public Class CompressedProxy(Of T As Csla.Serialization.Mobile.IMobileObject)
	  Inherits WcfProxy(Of T)
	Protected Overrides Function ConvertRequest(ByVal request As Csla.WcfPortal.CriteriaRequest) As Csla.WcfPortal.CriteriaRequest
	  Dim returnValue As New Csla.WcfPortal.CriteriaRequest()
	  returnValue.ClientContext = CompressionUtility.Compress(request.ClientContext)
      returnValue.GlobalContext = CompressionUtility.Compress(request.GlobalContext)
      If request.CriteriaData IsNot Nothing Then
        returnValue.CriteriaData = CompressionUtility.Compress(request.CriteriaData)
      End If
      returnValue.Principal = CompressionUtility.Compress(request.Principal)
      returnValue.TypeName = request.TypeName
      Return returnValue
    End Function

	Protected Overrides Function ConvertRequest(ByVal request As Csla.WcfPortal.UpdateRequest) As Csla.WcfPortal.UpdateRequest
	  Dim returnValue As New Csla.WcfPortal.UpdateRequest()
	  returnValue.ClientContext = CompressionUtility.Compress(request.ClientContext)
	  returnValue.GlobalContext = CompressionUtility.Compress(request.GlobalContext)
	  returnValue.ObjectData = CompressionUtility.Compress(request.ObjectData)
	  returnValue.Principal = CompressionUtility.Compress(request.Principal)
	  Return returnValue
	End Function

	Protected Overrides Function ConvertResponse(ByVal response As Csla.WcfPortal.WcfResponse) As Csla.WcfPortal.WcfResponse
	  Dim returnValue As New Csla.WcfPortal.WcfResponse()
	  returnValue.GlobalContext = CompressionUtility.Decompress(response.GlobalContext)
	  returnValue.ObjectData = CompressionUtility.Decompress(response.ObjectData)
	  returnValue.ErrorData = response.ErrorData
	  Return returnValue
	End Function
  End Class
End Namespace
