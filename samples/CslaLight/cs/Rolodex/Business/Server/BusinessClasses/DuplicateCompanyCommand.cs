using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Validation;
using System.ComponentModel;
using Csla.DataPortalClient;
using System.Threading;

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class DuplicateCompanyCommand : CommandBase
  {
#if SILVERLIGHT
    public DuplicateCompanyCommand(){}
#else
    protected DuplicateCompanyCommand() { }
#endif

    private string _companyName;
    private int _companyId;
    private bool _isDuplicate;

    public DuplicateCompanyCommand(string companyName, int companyId)
    {
      _companyName = companyName;
      _companyId = companyId;
      _isDuplicate = false;
    }

    public bool IsDuplicate
    {
      get { return _isDuplicate; }
    }

    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
    {
      base.OnGetState(info, mode);
      info.AddValue("_companyName", _companyName);
      info.AddValue("_companyId", _companyId);
      info.AddValue("_isDuplicate", _isDuplicate);
    }
    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
    {
      _companyName = info.GetValue<string>("_companyName");
      _companyId = info.GetValue<int>("_companyId");
      _isDuplicate = info.GetValue<bool>("_isDuplicate");
      base.OnSetState(info, mode);
    }

#if !SILVERLIGHT

    protected override void DataPortal_Execute()
    {
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("IsDuplicateCompany", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          command.Parameters.Add(new SqlParameter("@companyID", _companyId));
          command.Parameters.Add(new SqlParameter("@companyName", _companyName));
          SqlParameter isDuplicateParameter = new SqlParameter("@isDuplicate", _isDuplicate);
          isDuplicateParameter.Direction = System.Data.ParameterDirection.Output;
          command.Parameters.Add(isDuplicateParameter);
          command.ExecuteNonQuery();
          _isDuplicate = (bool)isDuplicateParameter.Value;
        }
        connection.Close();
      }
    }
#endif
  }
}
