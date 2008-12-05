Imports System

Namespace Validation

  ''' <summary>
  ''' Tracks all information for a rule.
  ''' </summary>
  Friend Interface IRuleMethod
    ''' <summary>
    ''' Gets the priority of the rule method.
    ''' </summary>
    ''' <value>The priority value</value>
    ''' <remarks>
    ''' Priorities are processed in descending
    ''' order, so priority 0 is processed
    ''' before priority 1, etc.
    ''' </remarks>
    ReadOnly Property Priority() As Integer
    ''' <summary>
    ''' Gets the name of the rule.
    ''' </summary>
    ''' <remarks>
    ''' The rule's name must be unique and is used
    ''' to identify a broken rule in the BrokenRules
    ''' collection.
    ''' </remarks>
    ReadOnly Property RuleName() As String
    ''' <summary>
    ''' Returns the name of the field, property or column
    ''' to which the rule applies.
    ''' </summary>
    ReadOnly Property RuleArgs() As RuleArgs
    ''' <summary>
    ''' Invokes the rule to validate the data.
    ''' </summary>
    ''' <returns>
    ''' <see langword="true" /> if the data is valid, 
    ''' <see langword="false" /> if the data is invalid.
    ''' </returns>
    Function Invoke(ByVal target As Object) As Boolean
  End Interface


End Namespace
