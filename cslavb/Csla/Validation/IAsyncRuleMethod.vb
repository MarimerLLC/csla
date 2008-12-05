Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Validation
  Friend Interface IAsyncRuleMethod
    Inherits IRuleMethod
    ReadOnly Property AsyncRuleArgs() As AsyncRuleArgs
    ReadOnly Property Severity() As RuleSeverity
    Overloads Sub Invoke(ByVal target As Object, ByVal complete As AsyncRuleCompleteHandler)
  End Interface
End Namespace
