Imports Csla.Core
Imports Csla.Utilities
Imports System.Collections.Generic

#If SILVERLIGHT Then
    Imports url = Csla.Utilities
#End If

Namespace Validation
  Friend Class AsyncRuleMethod
    Implements IAsyncRuleMethod, IComparable, IComparable(Of IRuleMethod)

    Private _handler As AsyncRuleHandler
    Private _args As AsyncRuleArgs
    Private _ruleName As String = String.Empty
    Private _severity As RuleSeverity
    Private Const _priority As Integer = 0

    ''' <summary>
    ''' Returns the name of the method implementing the rule
    ''' and the property, field or column name to which the
    ''' rule applies.
    ''' </summary>
    Public Overrides Function ToString() As String
      Return _ruleName
    End Function

    ''' <summary>
    ''' Gets the name of the rule.
    ''' </summary>
    ''' <remarks>
    ''' The rule's name must be unique and is used
    ''' to identify a broken rule in the BrokenRules
    ''' collection.
    ''' </remarks>
    Public ReadOnly Property RuleName() As String Implements IRuleMethod.RuleName
      Get
        Return _ruleName
      End Get
    End Property

    ''' <summary>
    ''' Not implemented.
    ''' </summary>
    Public ReadOnly Property RuleArgs() As RuleArgs Implements IRuleMethod.RuleArgs
      Get
        Throw New NotSupportedException()
      End Get
    End Property

    Public ReadOnly Property AsyncRuleArgs() As AsyncRuleArgs Implements IAsyncRuleMethod.AsyncRuleArgs
      Get
        Return _args
      End Get
    End Property

    ReadOnly Property Priority() As Integer Implements IRuleMethod.Priority
      Get
        Return _priority
      End Get
    End Property

    Public ReadOnly Property Severity() As RuleSeverity Implements IAsyncRuleMethod.Severity
      Get
        Return _severity
      End Get
    End Property

    ''' <summary>
    ''' Creates and initializes the rule.
    ''' </summary>
    ''' <param name="handler">The address of the method implementing the rule.</param>
    ''' <param name="args">A RuleArgs object.</param>
    ''' <param name="severity">Severity of the rule.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal handler As AsyncRuleHandler, ByVal args As AsyncRuleArgs, ByVal severity As RuleSeverity)
      handler = handler
      _args = args
      _severity = severity
      _ruleName = String.Format("rule://{0}/{1}/{2}", _
         Uri.EscapeDataString(_handler.Method.DeclaringType.FullName), _
      _handler.Method.Name, _
      _args.Properties(0).Name)
    End Sub

    ''' <summary>
    ''' You must call the IAsyncRuleMethod overload of Invoke.
    ''' </summary>
    Public Function Invoke(ByVal target As Object) As Boolean Implements IRuleMethod.Invoke
      Throw New NotSupportedException()
    End Function

    ''' <summary>
    ''' Invokes the asynchronous rule to validate the data.
    ''' </summary>
    ''' <see langword="true"/>if the data is valid, 
    ''' <see langword="false"/>if the data is invalid.
    Public Sub Invoke(ByVal target As Object, ByVal result As AsyncRuleCompleteHandler) Implements IAsyncRuleMethod.Invoke
      Dim propertyValues As Dictionary(Of String, Object) = GetPropertyValues(target, _args.Properties)
      _handler.Invoke(New AsyncValidationRuleContext(propertyValues, _args, New AsyncRuleResult(Me), Function(r) result(Me, r)))
    End Sub

#Region "IComparable"
    Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
      Return _priority.CompareTo(CType(obj, IRuleMethod).Priority)
    End Function

    Public Function CompareTo(ByVal other As IRuleMethod) As Integer Implements System.IComparable(Of IRuleMethod).CompareTo
      Return _priority.CompareTo(other.Priority)
    End Function
#End Region

    Private Shared Function GetPropertyValues(ByVal target As Object, ByVal ParamArray properties() As IPropertyInfo) As Dictionary(Of String, Object)
      Dim propertyValues As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

      For Each p As IPropertyInfo In properties
        propertyValues.Add(p.Name, Microsoft.VisualBasic.Interaction.CallByName(target, p.Name, CallType.Get))
      Next      
      Return propertyValues

    End Function

  End Class
End Namespace