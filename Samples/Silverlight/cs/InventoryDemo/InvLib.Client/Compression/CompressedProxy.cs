using System;
using System.Net;
using Csla.DataPortalClient;

namespace InvLib.Compression
{
  public class CompressedProxy<T> : WcfProxy<T>
    where T : Csla.Serialization.Mobile.IMobileObject
  {
    protected override Csla.WcfPortal.CriteriaRequest ConvertRequest(Csla.WcfPortal.CriteriaRequest request)
    {
      Csla.WcfPortal.CriteriaRequest returnValue = new Csla.WcfPortal.CriteriaRequest();
      returnValue.ClientContext = CompressionUtility.Compress(request.ClientContext);
      returnValue.GlobalContext = CompressionUtility.Compress(request.GlobalContext);
      if (request.CriteriaData != null)
        returnValue.CriteriaData = CompressionUtility.Compress(request.CriteriaData);
      returnValue.Principal = CompressionUtility.Compress(request.Principal);
      returnValue.TypeName = request.TypeName;
      return returnValue;
    }

    protected override Csla.WcfPortal.UpdateRequest ConvertRequest(Csla.WcfPortal.UpdateRequest request)
    {
      Csla.WcfPortal.UpdateRequest returnValue = new Csla.WcfPortal.UpdateRequest();
      returnValue.ClientContext = CompressionUtility.Compress(request.ClientContext);
      returnValue.GlobalContext = CompressionUtility.Compress(request.GlobalContext);
      returnValue.ObjectData = CompressionUtility.Compress(request.ObjectData);
      returnValue.Principal = CompressionUtility.Compress(request.Principal);
      return returnValue;
    }

    protected override Csla.WcfPortal.WcfResponse ConvertResponse(Csla.WcfPortal.WcfResponse response)
    {
      Csla.WcfPortal.WcfResponse returnValue = new Csla.WcfPortal.WcfResponse();
      returnValue.GlobalContext = CompressionUtility.Decompress(response.GlobalContext);
      returnValue.ObjectData = CompressionUtility.Decompress(response.ObjectData);
      returnValue.ErrorData = response.ErrorData;
      return returnValue;
    }
  }
}
