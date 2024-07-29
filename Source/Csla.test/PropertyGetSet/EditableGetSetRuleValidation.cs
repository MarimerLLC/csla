//-----------------------------------------------------------------------
// <copyright file="EditableGetSetRuleValidation.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

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
      BusinessRules.AddRule(new Rules.CommonRules.Required(_idProperty));

      // MemberBackedId
      BusinessRules.AddRule(new Rules.CommonRules.Required(_memberBackedIdProperty));

      // MemberBackedIdWithNoRelationshipTypes
      BusinessRules.AddRule(new Rules.CommonRules.Required(_memberBackedIdWithNoRelationshipTypesProperty));
    }

    #endregion

    #region Properties

    public static readonly PropertyInfo<String> _memberBackedIdProperty = RegisterProperty<String>(p => p.MemberBackedId, RelationshipTypes.PrivateField);
    private String _memberBackedId = _memberBackedIdProperty.DefaultValue;
    public String MemberBackedId
    {
      get { return GetProperty(_memberBackedIdProperty, _memberBackedId); }
      set { SetProperty(_memberBackedIdProperty, ref _memberBackedId, value); }
    }

    public static readonly PropertyInfo<String> _memberBackedIdWithNoRelationshipTypesProperty = RegisterProperty<String>(p => p.MemberBackedIdWithNoRelationshipTypes, string.Empty);
    private String _memberBackedIdWithNoRelationshipTypes = _memberBackedIdWithNoRelationshipTypesProperty.DefaultValue;
    public String MemberBackedIdWithNoRelationshipTypes
    {
      get { return GetProperty(_memberBackedIdWithNoRelationshipTypesProperty, _memberBackedIdWithNoRelationshipTypes); }
      set { SetProperty(_memberBackedIdWithNoRelationshipTypesProperty, ref _memberBackedIdWithNoRelationshipTypes, value); }
    }

    public static readonly PropertyInfo<String> _idProperty = RegisterProperty<String>(p => p.Id, string.Empty);
    public String Id
    {
      get { return GetProperty(_idProperty); }
      set { SetProperty(_idProperty, value); }
    }

    #endregion

    #region Data Access

    [RunLocal]
    [Create]
    protected void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }

    #endregion

    #region Factory Methods

    public static EditableGetSetRuleValidation NewEditableGetSetValidation(IDataPortal<EditableGetSetRuleValidation> dataPortal)
    {
      return dataPortal.Create();
    }

    #endregion
  }
}