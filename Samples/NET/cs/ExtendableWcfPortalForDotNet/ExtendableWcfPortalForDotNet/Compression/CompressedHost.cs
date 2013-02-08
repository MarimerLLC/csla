using System;
using System.Net;
using Csla.Server.Hosts;

namespace ExtendableWcfPortalForDotNet.Compression
{
  public class CompressedHost : ExtendableWcfPortalForDotNet.Server.WcfPortal
  {
    protected override CriteriaRequest ConvertRequest(CriteriaRequest request)
    {
      CriteriaRequest returnValue = new CriteriaRequest();
      returnValue.ClientContext = CompressionUtility.Decompress(request.ClientContext);
      returnValue.GlobalContext = CompressionUtility.Decompress(request.GlobalContext);
      if (request.CriteriaData != null)
        returnValue.CriteriaData = CompressionUtility.Decompress(request.CriteriaData);
      returnValue.Principal = CompressionUtility.Decompress(request.Principal);
      returnValue.TypeName = request.TypeName;
      returnValue.ClientCulture = request.ClientCulture;
      returnValue.ClientUICulture = request.ClientUICulture;
      return returnValue;
    }

    protected override UpdateRequest ConvertRequest(UpdateRequest request)
    {
      UpdateRequest returnValue = new UpdateRequest();
      returnValue.ClientContext = CompressionUtility.Decompress(request.ClientContext);
      returnValue.GlobalContext = CompressionUtility.Decompress(request.GlobalContext);
      returnValue.ObjectData = CompressionUtility.Decompress(request.ObjectData);
      returnValue.Principal = CompressionUtility.Decompress(request.Principal);
      returnValue.ClientCulture = request.ClientCulture;
      returnValue.ClientUICulture = request.ClientUICulture;
      return returnValue;
    }

    protected override WcfResponse ConvertResponse(WcfResponse response)
    {
      WcfResponse returnValue = new WcfResponse();
      returnValue.GlobalContext = CompressionUtility.Compress(response.GlobalContext);
      returnValue.ObjectData = CompressionUtility.Compress(response.ObjectData);
      returnValue.Error = response.Error;
      return returnValue;
    }
  }
}
