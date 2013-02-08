using System;
using System.Net;
using Csla.DataPortalClient;

namespace ExtendableWcfPortalForDotNet.Compression
{
  public class CompressedProxy : ExtendableWcfPortalForDotNet.Client.WcfProxy
  {
    protected override CriteriaRequest ConvertRequest(CriteriaRequest request)
    {

      CriteriaRequest returnValue = new CriteriaRequest();
      returnValue.ClientContext = CompressionUtility.Compress(request.ClientContext);
      returnValue.GlobalContext = CompressionUtility.Compress(request.GlobalContext);
      if (request.CriteriaData != null)
        returnValue.CriteriaData = CompressionUtility.Compress(request.CriteriaData);
      returnValue.Principal = CompressionUtility.Compress(request.Principal);
      returnValue.TypeName = request.TypeName;
      returnValue.ClientCulture = request.ClientCulture;
      returnValue.ClientUICulture = request.ClientUICulture;
      return returnValue;
    }

    protected override UpdateRequest ConvertRequest(UpdateRequest request)
    {
      UpdateRequest returnValue = new UpdateRequest();
      returnValue.ClientContext = CompressionUtility.Compress(request.ClientContext);
      returnValue.GlobalContext = CompressionUtility.Compress(request.GlobalContext);
      returnValue.ObjectData = CompressionUtility.Compress(request.ObjectData);
      returnValue.Principal = CompressionUtility.Compress(request.Principal);
      returnValue.ClientCulture = request.ClientCulture;
      returnValue.ClientUICulture = request.ClientUICulture;
      return returnValue;
    }

    protected override WcfResponse ConvertResponse(WcfResponse response)
    {
      WcfResponse returnValue = new WcfResponse();
      returnValue.GlobalContext = CompressionUtility.Decompress(response.GlobalContext);
      returnValue.ObjectData = CompressionUtility.Decompress(response.ObjectData);
      returnValue.Error = response.Error;
      return returnValue;
    }
  }
}
