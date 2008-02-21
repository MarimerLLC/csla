Namespace Validation

  ''' <summary>
  ''' Stores details about a specific broken business rule.
  ''' </summary>
  <Serializable()> _
  Public Class BrokenRule
    Private _ruleName As String
    Private _description As String
    Private _property As String
    Private _severity As RuleSeverity

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

  End Class

End Namespace
