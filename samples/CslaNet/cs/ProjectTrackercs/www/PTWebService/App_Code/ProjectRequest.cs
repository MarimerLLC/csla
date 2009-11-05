using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public class ProjectRequest
{
  private Guid _id;

  public Guid Id
  {
    get { return _id; }
    set { _id = value; }
  }
}
