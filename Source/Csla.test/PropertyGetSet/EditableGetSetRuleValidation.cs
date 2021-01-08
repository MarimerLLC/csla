//-----------------------------------------------------------------------
// <copyright file="EditableGetSetRuleValidation.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.PropertyGetSet
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [Serializable]
  public class EditableGetSetRuleValidation : BusinessBase<EditableGetSetRuleValidation>
  {
    #region Business Rules

    protected override void AddBusinessRules()
    {
      // Id
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_idProperty));
      
      // MemberBackedId
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_memberBackedIdProperty));

      // MemberBackedIdWithNoRelationshipTypes
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(_memberBackedIdWithNoRelationshipTypesProperty));
    }

    #endregion

    #region Properties

    public static readonly PropertyInfo<System.String> _memberBackedIdProperty = RegisterProperty<System.String>(p => p.MemberBackedId, RelationshipTypes.PrivateField);
    private System.String _memberBackedId = _memberBackedIdProperty.DefaultValue;
    public System.String MemberBackedId
    {
      get { return GetProperty(_memberBackedIdProperty, _memberBackedId); }
      set { SetProperty(_memberBackedIdProperty, ref _memberBackedId, value ); }
    }

    public static readonly PropertyInfo<System.String> _memberBackedIdWithNoRelationshipTypesProperty = RegisterProperty<System.String>(p => p.MemberBackedIdWithNoRelationshipTypes, string.Empty);
    private System.String _memberBackedIdWithNoRelationshipTypes = _memberBackedIdWithNoRelationshipTypesProperty.DefaultValue;
    public System.String MemberBackedIdWithNoRelationshipTypes
    {
      get { return GetProperty(_memberBackedIdWithNoRelationshipTypesProperty, _memberBackedIdWithNoRelationshipTypes); }
      set { SetProperty(_memberBackedIdWithNoRelationshipTypesProperty, ref _memberBackedIdWithNoRelationshipTypes, value); }
    }

    public static readonly PropertyInfo<System.String> _idProperty = RegisterProperty<System.String>(p => p.Id, string.Empty);
    public System.String Id
    {
      get { return GetProperty(_idProperty); }
      set { SetProperty(_idProperty, value); }
    }

    #endregion

    #region Data Access

    [RunLocal]
    protected override void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }

    #endregion

    #region Factory Methods

    public static EditableGetSetRuleValidation NewEditableGetSetValidation()
    {
      return Csla.DataPortal.Create<EditableGetSetRuleValidation>();
    }

    #endregion
  }
}