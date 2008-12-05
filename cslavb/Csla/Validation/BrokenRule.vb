Imports System
Imports Csla.Serialization
Imports Csla.Core
Imports Csla.Serialization.Mobile

Namespace Validation

  ''' <summary>
  ''' Stores details about a specific broken business rule.
  ''' </summary>
  <Serializable()> _
  Public Class BrokenRule
    Inherits MobileObject

    Private _ruleName As String
    Private _description As String
    Private _property As String
    Private _severity As RuleSeverity

    Friend Sub New(ByVal asyncRule As IAsyncRuleMethod, ByVal result As AsyncRuleResult)
      _ruleName = asyncRule.RuleName
      _description = result.Description
      _severity = result.Severity
      _property = asyncRule.AsyncRuleArgs.Properties(0).Name
    End Sub

    Friend Sub New(ByVal rule As IRuleMethod)
      _ruleName = rule.RuleName
      _description = rule.RuleArgs.Description
      _property = rule.RuleArgs.PropertyName
      _severity = rule.RuleArgs.Severity
    End Sub

    Friend Sub New(ByVal source As String, ByVal rule As BrokenRule)
      _ruleName = String.Format("rule://{0}.{1}", source, rule.RuleName.Replace("rule://", String.Empty))
      _description = rule.Description
      _property = rule.Property
      _severity = rule.Severity
    End Sub

    ''' <summary>
    ''' Provides access to the name of the broken rule.
    ''' </summary>
    ''' <value>The name of the rule.</value>
    Public ReadOnly Property RuleName() As String
      Get
        Return _ruleName
      End Get
    End Property

    ''' <summary>
    ''' Provides access to the description of the broken rule.
    ''' </summary>
    ''' <value>The description of the rule.</value>
    Public ReadOnly Property Description() As String
      Get
        Return _description
      End Get
    End Property

    ''' <summary>
    ''' Provides access to the property affected by the broken rule.
    ''' </summary>
    ''' <value>The property affected by the rule.</value>
    Public ReadOnly Property [Property]() As String
      Get
        Return _property
      End Get
    End Property

    ''' <summary>
    ''' Gets the severity of the broken rule.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Severity() As RuleSeverity
      Get
        Return _severity
      End Get
    End Property

    ''' <summary>
    ''' Override this method to insert your field values
    ''' into the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    ''' <param name="mode">
    ''' The StateMode indicating why this method was invoked.
    ''' </param>
    Protected Overrides Sub OnGetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As Core.StateMode)
      info.AddValue("_ruleName", _ruleName)
      info.AddValue("_description", _description)
      info.AddValue("_property", _property)
      info.AddValue("_severity", CType(_severity, Integer))
      MyBase.OnGetState(info, mode)
    End Sub

    ''' <summary>
    ''' Override this method to retrieve your field values
    ''' from the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    ''' <param name="mode">
    ''' The StateMode indicating why this method was invoked.
    ''' </param>
    Protected Overrides Sub OnSetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As Core.StateMode)
      _ruleName = info.GetValue(Of String)("_ruleName")
      _description = info.GetValue(Of String)("_description")
      _property = info.GetValue(Of String)("_property")
      _severity = info.GetValue(Of RuleSeverity)("_severity")
      MyBase.OnSetState(info, mode)
    End Sub

  End Class

End Namespace
