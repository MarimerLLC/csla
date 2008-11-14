Namespace Validation

    Public Class AsyncRuleResult

        Private _result As Boolean = True
        Private _description As String
        Private _severity As RuleSeverity = RuleSeverity.Error

        ''' <summary>
        ''' Result value for the validation
        ''' rule method, where true indicates
        ''' the rule was not violated.
        ''' </summary>
        Public Property Result() As Boolean
            Get
                Return _result
            End Get
            Set(ByVal value As Boolean)
                _result = value
            End Set
        End Property

        ''' <summary>
        ''' Set by the rule handler method to describe the broken
        ''' rule.
        ''' </summary>
        ''' <value>A human-readable description of the broken rule.</value>
        ''' <returns></returns>
        ''' <remarks>Setting this property only has an effect if
        ''' the rule method returns <see langword="false" />
        ''' </remarks>
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the severity of the broken rule.
        ''' </summary>
        ''' <value>The severity of the broken rule.</value>
        ''' <remarks>Setting this property only has an effect if
        ''' the rule method returns <see langword="false" />.
        ''' </remarks>
        Public Property Severity() As RuleSeverity
            Get
                Return _severity
            End Get
            Set(ByVal value As RuleSeverity)
                _severity = value
            End Set
        End Property

        Friend Sub New(ByVal rule As IAsyncRuleMethod)
            _severity = rule.Severity
        End Sub

        ''' <summary>
        ''' Returns a string representation of the object.
        ''' </summary>
        Public Overrides Function ToString() As String
            Return _description
        End Function

    End Class

End Namespace

