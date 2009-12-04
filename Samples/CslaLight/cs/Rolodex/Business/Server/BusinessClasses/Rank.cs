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

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
using Csla.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class Rank : BusinessBase<Rank>
  {
#if SILVERLIGHT
    public Rank() { }
#else
    private Rank() { }
#endif

    internal static Rank NewRank()
    {
      Rank returnValue = new Rank();
      returnValue.ValidationRules.CheckRules();
      return returnValue;
    }

    private static PropertyInfo<int> RankIdProperty = RegisterProperty<int>(new PropertyInfo<int>("RankId", "Rank Id", 0));
    public int RankId
    {
      get
      {
        return GetProperty(RankIdProperty);
      }
    }

    private static PropertyInfo<string> RankNameProperty = RegisterProperty<string>(new PropertyInfo<string>("RankName", "Rank Name", string.Empty));
    public string RankName
    {
      get
      {
        return GetProperty(RankNameProperty);
      }
      set
      {
        SetProperty(RankNameProperty, value);
      }
    }

    protected override void AddAuthorizationRules()
    {
      string[] canWrite = new string[] { "AdminUser", "RegularUser" };
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
      string[] admin = new string[] { "AdminUser" };
      AuthorizationRules.AllowCreate(typeof(Rank), admin);
      AuthorizationRules.AllowDelete(typeof(Rank), admin);
      AuthorizationRules.AllowEdit(typeof(Rank), canWrite);
      AuthorizationRules.AllowGet(typeof(Rank), canRead);
      AuthorizationRules.AllowWrite(RankNameProperty, canWrite);
      AuthorizationRules.AllowRead(RankNameProperty, canRead);
      AuthorizationRules.AllowRead(RankIdProperty, canRead);
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, new Csla.Validation.RuleArgs(RankNameProperty));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs(RankNameProperty, 20));

    }

#if !SILVERLIGHT

    protected override void DataPortal_Insert()
    {
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("RanksInsert", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          SqlParameter idParameter = new SqlParameter("@rankId", ReadProperty(RankIdProperty));
          idParameter.Direction = System.Data.ParameterDirection.Output;
          command.Parameters.Add(idParameter);
          command.Parameters.Add(new SqlParameter("@rank", ReadProperty(RankNameProperty)));

          command.ExecuteNonQuery();
          LoadProperty(RankIdProperty, (int)idParameter.Value);
        }
        connection.Close();
      }
    }
    protected override void DataPortal_Update()
    {
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("RanksUpdate", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          command.Parameters.Add(new SqlParameter("@rankId", ReadProperty(RankIdProperty)));
          command.Parameters.Add(new SqlParameter("@rank", ReadProperty(RankNameProperty)));
          command.ExecuteNonQuery();
        }
        connection.Close();
      }
    }

    protected override void DataPortal_DeleteSelf()
    {
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("RanksDelete", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          command.Parameters.Add(new SqlParameter("@rankId", ReadProperty(RankIdProperty)));
          command.ExecuteNonQuery();
        }
        connection.Close();
      }
    }

    internal static Rank GetRank(SafeDataReader reader)
    {
      Rank returnValue = new Rank();
      returnValue.LoadProperty<int>(RankIdProperty, reader.GetInt32("RankID"));
      returnValue.LoadProperty<string>(RankNameProperty, reader.GetString("Rank"));
      returnValue.MarkOld();
      return returnValue;
    }

#endif
  }
}
