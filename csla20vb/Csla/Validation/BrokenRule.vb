Namespace Validation

  ''' <summary>
  ''' Stores details about a specific broken business rule.
  ''' </summary>
  <Serializable()> _
  Public Class BrokenRule
    Private mRuleName As String
    Private mDescription As String
    Private mProperty As String

    Friend Sub New(ByVal rule As RuleMethod)
      mRuleName = rule.RuleName
      mDescription = rule.RuleArgs.Description
      mProperty = rule.RuleArgs.PropertyName
    End Sub

    ''' <summary>
    ''' Provides access to the name of the broken rule.
    ''' </summary>
    ''' <value>The name of the rule.</value>
    Public ReadOnly Property RuleName() As String
      Get
        Return mRuleName
      End Get
    End Property

    ''' <summary>
    ''' Provides access to the description of the broken rule.
    ''' </summary>
    ''' <value>The description of the rule.</value>
    Public ReadOnly Property Description() As String
      Get
        Return mDescription
      End Get
    End Property

    ''' <summary>
    ''' Provides access to the property affected by the broken rule.
    ''' </summary>
    ''' <value>The property affected by the rule.</value>
    Public ReadOnly Property [Property]() As String
      Get
        Return mProperty
      End Get
    End Property

  End Class

End Namespace
