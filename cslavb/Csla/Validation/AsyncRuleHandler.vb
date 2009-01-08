Namespace Validation

  ''' <summary>
  ''' Delegate defining an asynchronous validation rule method.
  ''' </summary>
  ''' <param name="context">
  ''' Context parameters provided to the validation rule method.
  ''' </param>
  ''' <remarks></remarks>
  Public Delegate Sub AsyncRuleHandler(ByVal context As AsyncValidationRuleContext)

End Namespace