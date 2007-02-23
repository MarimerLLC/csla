Namespace Validation

  ''' <summary>
  ''' Object providing extra information to methods that
  ''' implement business rules.
  ''' </summary>
  Public Class RuleArgs

    Private mPropertyName As String
    Private mDescription As String
    Private mSeverity As RuleSeverity = RuleSeverity.Error
    Private mStopProcessing As Boolean

    ''' <summary>
    ''' The name of the property to be validated.
    ''' </summary>
    Public ReadOnly Property PropertyName() As String
      Get
        Return mPropertyName
      End Get
    End Property

    ''' <summary>
    ''' Set by the rule handler method to describe the broken
    ''' rule.
    ''' </summary>
    ''' <value>A human-readable description of
    ''' the broken rule.</value>
    ''' <remarks>
    ''' Setting this property only has an effect if
    ''' the rule method returns <see langword="false" />.
    ''' </remarks>
    Public Property Description() As String
      Get
        Return mDescription
      End Get
      Set(ByVal Value As String)
        mDescription = Value
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the severity of the broken rule.
    ''' </summary>
    ''' <value>The severity of the broken rule.</value>
    ''' <remarks>
    ''' Setting this property only has an effect if
    ''' the rule method returns <see langword="false" />.
    ''' </remarks>
    Public Property Severity() As RuleSeverity
      Get
        Return mSeverity
      End Get
      Set(ByVal value As RuleSeverity)
        mSeverity = value
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether this
    ''' broken rule should stop the processing of subsequent
    ''' rules for this property.
    ''' </summary>
    ''' <value><see langword="true" /> if no further
    ''' rules should be process for this property.</value>
    ''' <remarks>
    ''' Setting this property only has an effect if
    ''' the rule method returns <see langword="false" />.
    ''' </remarks>
    Public Property StopProcessing() As Boolean
      Get
        Return mStopProcessing
      End Get
      Set(ByVal value As Boolean)
        mStopProcessing = value
      End Set
    End Property


    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    Public Sub New(ByVal propertyName As String)
      mPropertyName = propertyName
    End Sub

    ''' <summary>
    ''' Returns a string representation of the object.
    ''' </summary>
    Public Overrides Function ToString() As String
      Return mPropertyName
    End Function

  End Class

End Namespace
