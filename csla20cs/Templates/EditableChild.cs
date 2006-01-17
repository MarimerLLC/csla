using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace Templates
{
  [Serializable()]
  class EditableChild : BusinessBase<EditableChild>
  {
    #region Business Methods

    private int _id;

    public int id
    {
      get
      {
        CanReadProperty(true);
        return _id;
      }
      set
      {
        CanWriteProperty(true);
        if (_id != value)
        {
          _id = value;
          PropertyHasChanged();
        }
      }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // TODO: add validation rules
      //ValidationRules.AddRule(null, "");
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowWrite("", "");
    }

    #endregion

    #region Factory Methods

    internal static EditableChild NewEditableChild()
    {
      return new EditableChild();
    }

    internal static EditableChild GetEditableChild(SqlDataReader dr)
    {
      return new EditableChild(dr);
    }

    private EditableChild()
    {
      MarkAsChild();
    }

    private EditableChild(SqlDataReader dr)
    {
      MarkAsChild();
      Fetch(dr);
    }

    #endregion

    #region Data Access

    private void Fetch(SqlDataReader dr)
    {
      // TODO: load values
      MarkOld();
    }

    internal void Insert()
    {
      // TODO: insert values
      MarkOld();
    }

    internal void Update()
    {
      // TODO: update values
      MarkOld();
    }

    internal void DeleteSelf()
    {
      // TODO: delete values
      MarkNew();
    }

    #endregion
  }
}
