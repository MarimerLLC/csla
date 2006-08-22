Namespace Validation

  ''' <summary>
  ''' Tracks all information for a rule.
  ''' </summary>
  Friend Class RuleMethod

    Implements IRuleMethod
    Implements IComparable
    Implements IComparable(Of IRuleMethod)

    Private mHandler As RuleHandler
    Private mRuleName As String = ""
    Private mArgs As RuleArgs
    Private mPriority As Integer

    ''' <summary>
    ''' Returns the name of the method implementing the rule
    ''' and the property, field or column name to which the
    ''' rule applies.
    ''' </summary>
    Public Overrides Function ToString() As String
      Return mRuleName
    End Function

    ''' <summary>
    ''' Gets the priority of the rule method.
    ''' </summary>
    ''' <value>The priority value</value>
    ''' <remarks>
    ''' Priorities are processed in descending
    ''' order, so priority 0 is processed
    ''' before priority 1, etc.
    ''' </remarks>
    Public ReadOnly Property Priority() As Integer Implements IRuleMethod.Priority
      Get
        Return mPriority
      End Get
    End Property

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
        Return mRuleName
      End Get
    End Property

    ''' <summary>
    ''' Returns the name of the field, property or column
    ''' to which the rule applies.
    ''' </summary>
    Public ReadOnly Property RuleArgs() As RuleArgs Implements IRuleMethod.RuleArgs
      Get
        Return mArgs
      End Get
    End Property

    ''' <summary>
    ''' Creates and initializes the rule.
    ''' </summary>
    ''' <param name="handler">The address of the method implementing the rule.</param>
    ''' <param name="args">A RuleArgs object.</param>
    Public Sub New(ByVal handler As RuleHandler, _
      ByVal args As RuleArgs)

      mHandler = handler
      mArgs = args
      mRuleName = "rule://" & mHandler.Method.Name & "/" & mArgs.ToString

    End Sub

    ''' <summary>
    ''' Creates and initializes the rule.
    ''' </summary>
    ''' <param name="handler">The address of the method implementing the rule.</param>
    ''' <param name="args">A RuleArgs object.</param>
    ''' <param name="priority">
    ''' Priority for processing the rule (smaller numbers have higher priority, default=0).
    ''' </param>
    Public Sub New(ByVal handler As RuleHandler, _
      ByVal args As RuleArgs, ByVal priority As Integer)

      Me.New(handler, args)
      mPriority = priority

    End Sub

    ''' <summary>
    ''' Invokes the rule to validate the data.
    ''' </summary>
    ''' <returns>
    ''' <see langword="true" /> if the data is valid, 
    ''' <see langword="false" /> if the data is invalid.
    ''' </returns>
    Public Function Invoke(ByVal target As Object) As Boolean Implements IRuleMethod.Invoke
      Return mHandler.Invoke(target, mArgs)
    End Function

#Region " IComparable "

    Private Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo

      Return Priority.CompareTo(CType(obj, IRuleMethod).Priority)

    End Function

    Private Function CompareTo1(ByVal other As IRuleMethod) As Integer Implements System.IComparable(Of IRuleMethod).CompareTo

      Return Priority.CompareTo(other.Priority)

    End Function

#End Region

  End Class




  ''' <summary>
  ''' Tracks all information for a rule.
  ''' </summary>
  ''' <typeparam name="T">Type of the target object.</typeparam>
  ''' <typeparam name="R">Type of the arguments parameter.</typeparam>
  Friend Class RuleMethod(Of T, R As RuleArgs)

    Implements IRuleMethod
    Implements IComparable
    Implements IComparable(Of IRuleMethod)

    Private mHandler As RuleHandler(Of T, R)
    Private mRuleName As String = ""
    Private mArgs As R
    Private mPriority As Integer

    ''' <summary>
    ''' Returns the name of the method implementing the rule
    ''' and the property, field or column name to which the
    ''' rule applies.
    ''' </summary>
    Public Overrides Function ToString() As String
      Return mRuleName
    End Function

    ''' <summary>
    ''' Gets the priority of the rule method.
    ''' </summary>
    ''' <value>The priority value</value>
    ''' <remarks>
    ''' Priorities are processed in descending
    ''' order, so priority 0 is processed
    ''' before priority 1, etc.
    ''' </remarks>
    Public ReadOnly Property Priority() As Integer Implements IRuleMethod.Priority
      Get
        Return mPriority
      End Get
    End Property

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
        Return mRuleName
      End Get
    End Property

    ''' <summary>
    ''' Returns the name of the field, property or column
    ''' to which the rule applies.
    ''' </summary>
    Private ReadOnly Property IRuleMethod_RuleArgs() As RuleArgs Implements IRuleMethod.RuleArgs
      Get
        Return RuleArgs
      End Get
    End Property

    ''' <summary>
    ''' Returns the name of the field, property or column
    ''' to which the rule applies.
    ''' </summary>
    Public ReadOnly Property RuleArgs() As R
      Get
        Return mArgs
      End Get
    End Property

    ''' <summary>
    ''' Creates and initializes the rule.
    ''' </summary>
    ''' <param name="handler">The address of the method implementing the rule.</param>
    ''' <param name="args">A RuleArgs object.</param>
    Public Sub New(ByVal handler As RuleHandler(Of T, R), _
      ByVal args As R)

      mHandler = handler
      mArgs = args
      mRuleName = "rule://" & mHandler.Method.Name & "/" & mArgs.ToString

    End Sub

    ''' <summary>
    ''' Creates and initializes the rule.
    ''' </summary>
    ''' <param name="handler">The address of the method implementing the rule.</param>
    ''' <param name="args">A RuleArgs object.</param>
    ''' <param name="priority">
    ''' Priority for processing the rule (smaller numbers have higher priority, default=0).
    ''' </param>
    Public Sub New(ByVal handler As RuleHandler(Of T, R), _
      ByVal args As R, ByVal priority As Integer)

      Me.New(handler, args)
      mPriority = priority

    End Sub

    ''' <summary>
    ''' Invokes the rule to validate the data.
    ''' </summary>
    ''' <returns>True if the data is valid, False if the data is invalid.</returns>
    Private Function IRuleMethod_Invoke(ByVal target As Object) As Boolean Implements IRuleMethod.Invoke
      Return Invoke(DirectCast(target, T))
    End Function

    ''' <summary>
    ''' Invokes the rule to validate the data.
    ''' </summary>
    ''' <returns>
    ''' <see langword="true" /> if the data is valid, 
    ''' <see langword="false" /> if the data is invalid.
    ''' </returns>
    Public Function Invoke(ByVal target As T) As Boolean
      Return mHandler.Invoke(target, mArgs)
    End Function

#Region " IComparable "

    Private Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo

      Return Priority.CompareTo(CType(obj, IRuleMethod).Priority)

    End Function

    Private Function CompareTo1(ByVal other As IRuleMethod) As Integer Implements System.IComparable(Of IRuleMethod).CompareTo

      Return Priority.CompareTo(other.Priority)

    End Function

#End Region

  End Class

End Namespace