Imports System.Collections.Generic

Namespace Validation
    Public Class AsyncValidationRuleContext

        Private _propertyValues As Dictionary(Of String, Object)
        Private _inargs As AsyncRuleArgs
        Private _outargs As AsyncRuleResult
        Private _result As AsyncRuleResultHandler

        ''' <summary>
        ''' Gets a Dictionary containing the values of all properties
        ''' associated with this rule.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' The values provided by this property are copies of the original
        ''' values. This helps provide thread safety, allowing a rule method
        ''' to interact with the values safely, even though the code is running
        ''' on a background thread.
        ''' </remarks>
        Public ReadOnly Property PropertyValues() As Dictionary(Of String, Object)
            Get
                Return _propertyValues
            End Get
        End Property

        ''' <summary>
        ''' Gets the input arguments to the validation rule. 
        ''' </summary>
        ''' <remarks>
        ''' This 
        ''' property provides much of the same information as the
        ''' RuleArgs parameter does to a synchronous rule method.
        ''' </remarks>
        Public ReadOnly Property InArgs() As AsyncRuleArgs
            Get
                Return _inargs
            End Get
        End Property

        ''' <summary>
        ''' Gets the output arguments for the validation rule. The rule
        ''' should set properties on this object for return to the
        ''' validation subsystem.
        ''' </summary>
        Public ReadOnly Property OutArgs() As AsyncRuleResult
            Get
                Return _outargs
            End Get
        End Property

        ''' <summary>
        ''' Creates an instance of the object.
        ''' </summary>
        ''' <param name="propertyValues">
        ''' Dictionary containing copies of the business object
        ''' property values for the properties associated with this rule.
        ''' </param>
        ''' <param name="inargs">
        ''' Input arguments for use by the rule method.
        ''' </param>
        ''' <param name="outargs">
        ''' Default output arguments that can be changed by the rule
        ''' method.
        ''' </param>
        ''' <param name="result">
        ''' Async result handler for the async callback on completion
        ''' of the rule.
        ''' </param>
        Public Sub New(ByVal propertyValues As Dictionary(Of String, Object), ByVal inargs As AsyncRuleArgs, ByRef outargs As AsyncRuleResult, ByVal result As AsyncRuleResultHandler)
            _propertyValues = propertyValues
            _inargs = inargs
            _outargs = outargs
            _result = result
        End Sub

        ''' <summary>
        ''' Method that notifies the validation subsystem 
        ''' when the async rule method is complete. This
        ''' method <b>must be called</b>!
        ''' </summary>
        ''' <remarks>
        ''' The async rule method <b>must</b> call this
        ''' Complete() method when it is done (successfully
        ''' or unsuccessfully). This includes the case
        ''' where an exception occurred in the rule method.
        ''' <b>This method must be called no matter what
        ''' happens.</b>
        ''' </remarks>
        Public Sub Complete()
            _result(_outargs)
        End Sub

    End Class

End Namespace

