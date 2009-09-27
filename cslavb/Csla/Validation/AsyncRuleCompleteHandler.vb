Namespace Validation
  ''' <summary>
  ''' Delegate for handling the completion of an
  ''' async validation rule.
  ''' </summary>
  ''' <param name="sender">Object calling the handler.</param>
  ''' <param name="result">Result arguments from the validation rule.</param>
  ''' <remarks>Note there is a difference in the method declaration between c# and VB.net. In AsyncRuleMethod.Invoke we
  ''' use a Lambda expression with this and in VB 9.0 the Lambda expression requires a single return value where c# does not.</remarks>
  Public Delegate Function AsyncRuleCompleteHandler(ByVal sender As Object, ByVal result As AsyncRuleResult) As Boolean

End Namespace

