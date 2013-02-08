using System;
using System.Net;
using Csla.Server.Hosts;

namespace InvLib.Compression
{
  public class CompressedHost : Csla.Server.Hosts.Silverlight.WcfPortal
  {
    protected override Csla.Server.Hosts.Silverlight.CriteriaRequest ConvertRequest(Csla.Server.Hosts.Silverlight.CriteriaRequest request)
    {
      Csla.Server.Hosts.Silverlight.CriteriaRequest returnValue = new Csla.Server.Hosts.Silverlight.CriteriaRequest();
      returnValue.ClientContext = CompressionUtility.Decompress(request.ClientContext);
      returnValue.GlobalContext = CompressionUtility.Decompress(request.GlobalContext);
      if (request.CriteriaData != null)
        returnValue.CriteriaData = CompressionUtility.Decompress(request.CriteriaData);
      returnValue.Principal = CompressionUtility.Decompress(request.Principal);
      returnValue.TypeName = request.TypeName;
      return returnValue;
    }

    protected override Csla.Server.Hosts.Silverlight.UpdateRequest ConvertRequest(Csla.Server.Hosts.Silverlight.UpdateRequest request)
    {
      Csla.Server.Hosts.Silverlight.UpdateRequest returnValue = new Csla.Server.Hosts.Silverlight.UpdateRequest();
      returnValue.ClientContext = CompressionUtility.Decompress(request.ClientContext);
      returnValue.GlobalContext = CompressionUtility.Decompress(request.GlobalContext);
      returnValue.ObjectData = CompressionUtility.Decompress(request.ObjectData);
      returnValue.Principal = CompressionUtility.Decompress(request.Principal);
      return returnValue;
    }

    protected override Csla.Server.Hosts.Silverlight.WcfResponse ConvertResponse(Csla.Server.Hosts.Silverlight.WcfResponse response)
    {
      Csla.Server.Hosts.Silverlight.WcfResponse returnValue = new Csla.Server.Hosts.Silverlight.WcfResponse();
      returnValue.GlobalContext = CompressionUtility.Compress(response.GlobalContext);
      returnValue.ObjectData = CompressionUtility.Compress(response.ObjectData);
      returnValue.ErrorData = response.ErrorData;
      return returnValue;
    }
  }
}
