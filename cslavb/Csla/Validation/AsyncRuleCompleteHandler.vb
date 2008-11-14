Namespace Validation
    ''' <summary>
    ''' Delegate for handling the completion of an
    ''' async validation rule.
    ''' </summary>
    ''' <param name="sender">Object calling the handler.</param>
    ''' <param name="result">Result arguments from the validation rule.</param>
    ''' <remarks></remarks>
    Public Delegate Sub AsyncRuleCompleteHandler(ByVal sender As Object, ByVal result As AsyncRuleResult)

End Namespace

