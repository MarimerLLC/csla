Namespace Validation

  ''' <summary>
  ''' Tracks all information for a rule.
  ''' </summary>
  Friend Class RuleMethod
    Private mTarget As Object
    Private mHandler As RuleHandler
    Private mRuleName As String = ""
    Private mArgs As RuleArgs

    ''' <summary>
    ''' Returns the name of the method implementing the rule
    ''' and the property, field or column name to which the
    ''' rule applies.
    ''' </summary>
    Public Overrides Function ToString() As String
      Return mRuleName
    End Function

    '''' <summary>
    '''' Returns the delegate to the method implementing the rule.
    '''' </summary>
    'Public ReadOnly Property Handler() As RuleHandler
    '  Get
    '    Return mHandler
    '  End Get
    'End Property

    ''' <summary>
    ''' Gets the name of the rule.
    ''' </summary>
    ''' <remarks>
    ''' The rule's name must be unique and is used
    ''' to identify a broken rule in the BrokenRules
    ''' collection.
    ''' </remarks>
    Public ReadOnly Property RuleName() As String
      Get
        Return mRuleName
      End Get
      'Set(ByVal value As String)
      '  mRuleName = value
      'End Set
    End Property

    ''' <summary>
    ''' Returns the name of the field, property or column
    ''' to which the rule applies.
    ''' </summary>
    Public ReadOnly Property RuleArgs() As RuleArgs
      Get
        Return mArgs
      End Get
    End Property

    ''' <summary>
    ''' Creates and initializes the rule.
    ''' </summary>
    ''' <param name="target">Reference to the object containing the data to validate.</param>
    ''' <param name="handler">The address of the method implementing the rule.</param>
    ''' <param name="propertyName">The field, property or column to which the rule applies.</param>
    Public Sub New(ByVal target As Object, ByVal handler As RuleHandler, _
      ByVal propertyName As String)

      mTarget = target
      mHandler = handler
      mArgs = New RuleArgs(propertyName)
      mRuleName = mHandler.Method.Name & "!" & mArgs.ToString

    End Sub

    ''' <summary>
    ''' Creates and initializes the rule.
    ''' </summary>
    ''' <param name="target">Reference to the object containing the data to validate.</param>
    ''' <param name="handler">The address of the method implementing the rule.</param>
    ''' <param name="args">A RuleArgs object.</param>
    Public Sub New(ByVal target As Object, ByVal handler As RuleHandler, _
      ByVal args As RuleArgs)

      mTarget = target
      mHandler = handler
      mArgs = args
      mRuleName = mHandler.Method.Name & "!" & mArgs.ToString

    End Sub

    ''' <summary>
    ''' Invokes the rule to validate the data.
    ''' </summary>
    ''' <returns>True if the data is valid, False if the data is invalid.</returns>
    Public Function Invoke() As Boolean
      Return mHandler.Invoke(mTarget, mArgs)
    End Function

  End Class

End Namespace