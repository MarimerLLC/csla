Namespace Validation

  ''' <summary>
  ''' Stores details about a specific broken business rule.
  ''' </summary>
  <Serializable()> _
  Public Class BrokenRule
    Private mRuleName As String
    Private mDescription As String
    Private mProperty As String

    Friend Sub New(ByVal ruleName As String, ByVal description As String, ByVal [property] As String)
      mRuleName = ruleName
      mDescription = description
      mProperty = [property]
    End Sub

    ''' <summary>
    ''' Provides access to the name of the broken rule.
    ''' </summary>
    ''' <remarks>
    ''' This value is actually readonly, not readwrite. Any new
    ''' value set into this property is ignored. The property is only
    ''' readwrite because that is required to support data binding
    ''' within Web Forms.
    ''' </remarks>
    ''' <value>The name of the rule.</value>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="value")> _
    Public Property RuleName() As String
      Get
        Return mRuleName
      End Get
      Set(ByVal value As String)
        ' the property must be read-write for Web Forms data binding
        ' to work, but we really don't want to allow the value to be
        ' changed dynamically so we ignore any attempt to set it
      End Set
    End Property

    ''' <summary>
    ''' Provides access to the description of the broken rule.
    ''' </summary>
    ''' <remarks>
    ''' This value is actually readonly, not readwrite. Any new
    ''' value set into this property is ignored. The property is only
    ''' readwrite because that is required to support data binding
    ''' within Web Forms.
    ''' </remarks>
    ''' <value>The description of the rule.</value>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="value")> _
    Public Property Description() As String
      Get
        Return mDescription
      End Get
      Set(ByVal value As String)
        ' the property must be read-write for Web Forms data binding
        ' to work, but we really don't want to allow the value to be
        ' changed dynamically so we ignore any attempt to set it
      End Set
    End Property

    ''' <summary>
    ''' Provides access to the property affected by the broken rule.
    ''' </summary>
    ''' <remarks>
    ''' This value is actually readonly, not readwrite. Any new
    ''' value set into this property is ignored. The property is only
    ''' readwrite because that is required to support data binding
    ''' within Web Forms.
    ''' </remarks>
    ''' <value>The property affected by the rule.</value>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="value")> _
    Public Property [Property]() As String
      Get
        Return mProperty
      End Get
      Set(ByVal value As String)
        ' the property must be read-write for Web Forms data binding
        ' to work, but we really don't want to allow the value to be
        ' changed dynamically so we ignore any attempt to set it
      End Set
    End Property

  End Class

End Namespace
