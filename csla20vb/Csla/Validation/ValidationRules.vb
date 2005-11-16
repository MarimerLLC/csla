Namespace Validation

  ''' <summary>
  ''' Tracks the business rules broken within a business object.
  ''' </summary>
  <Serializable()> _
  Public Class ValidationRules

#Region " RuleMethod Class "

    ''' <summary>
    ''' Tracks all information for a rule.
    ''' </summary>
    Private Class RuleMethod
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
        mRuleName = mHandler.Method.Name & "!" & propertyName
        mArgs = New RuleArgs(propertyName)

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
        mRuleName = mHandler.Method.Name & "!" & args.PropertyName
        mArgs = args

      End Sub

      '''' <summary>
      '''' Creates and initializes the rule.
      '''' </summary>
      '''' <param name="target">Reference to the object containing the data to validate.</param>
      '''' <param name="handler">The address of the method implementing the rule.</param>
      '''' <param name="args">A RuleArgs object.</param>
      '''' <param name="ruleName">A unique name for this rule.</param>
      'Public Sub New(ByVal target As Object, ByVal handler As RuleHandler, _
      '  ByVal args As RuleArgs, ByVal ruleName As String)

      '  mTarget = target
      '  mHandler = handler
      '  mRuleName = ruleName
      '  mArgs = args

      'End Sub

      ''' <summary>
      ''' Invokes the rule to validate the data.
      ''' </summary>
      ''' <returns>True if the data is valid, False if the data is invalid.</returns>
      Public Function Invoke() As Boolean
        Return mHandler.Invoke(mTarget, mArgs)
      End Function

    End Class

#End Region

    Private mBrokenRules As BrokenRulesCollection
    <NonSerialized()> _
    Private mTarget As Object
    <NonSerialized()> _
    Private mRulesList As _
      Generic.Dictionary(Of String, List(Of RuleMethod))

    Friend Sub New(ByVal businessObject As Object)

      SetTarget(businessObject)

    End Sub

    Friend Sub SetTarget(ByVal businessObject As Object)

      mTarget = businessObject

    End Sub

    Private ReadOnly Property BrokenRulesList() As BrokenRulesCollection
      Get
        If mBrokenRules Is Nothing Then
          mBrokenRules = New BrokenRulesCollection
        End If
        Return mBrokenRules
      End Get
    End Property

    Private ReadOnly Property RulesList() As Generic.Dictionary(Of String, List(Of RuleMethod))
      Get
        If mRulesList Is Nothing Then
          mRulesList = New Generic.Dictionary(Of String, List(Of RuleMethod))
        End If
        Return mRulesList
      End Get
    End Property

#Region " Adding Rules "

    ''' <summary>
    ''' Returns the list containing rules for a rule name. If
    ''' no list exists one is created and returned.
    ''' </summary>
    Private Function GetRulesForProperty( _
      ByVal propertyName As String) As List(Of RuleMethod)

      ' get the list (if any) from the dictionary
      Dim list As List(Of RuleMethod) = Nothing
      If RulesList.ContainsKey(propertyName) Then
        list = RulesList.Item(propertyName)
      End If

      If list Is Nothing Then
        ' there is no list for this name - create one
        list = New List(Of RuleMethod)
        RulesList.Add(propertyName, list)
      End If
      Return list

    End Function

    ''' <summary>
    ''' Adds a rule to the list of rules to be enforced.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' A rule is implemented by a method which conforms to the 
    ''' method signature defined by the RuleHandler delegate.
    ''' </para><para>
    ''' The propertyName may be used by the method that implements the rule
    ''' in order to retrieve the value to be validated. If the rule
    ''' implementation is inside the target object then it probably has
    ''' direct access to all data. However, if the rule implementation
    ''' is outside the target object then it will need to use reflection
    ''' or CallByName to dynamically invoke this property to retrieve
    ''' the value to be validated.
    ''' </para>
    ''' </remarks>
    ''' <param name="handler">The method that implements the rule.</param>
    ''' <param name="propertyName">
    ''' The property name on the target object where the rule implementation can retrieve
    ''' the value to be validated.
    ''' </param>
    Public Sub AddRule(ByVal handler As RuleHandler, ByVal propertyName As String)

      ' get the list of rules for the property
      Dim list As List(Of RuleMethod) = GetRulesForProperty(propertyName)

      ' we have the list, add our new rule
      list.Add(New RuleMethod(mTarget, handler, propertyName))

    End Sub

    ''' <summary>
    ''' Adds a rule to the list of rules to be enforced.
    ''' </summary>
    ''' <remarks>
    ''' A rule is implemented by a method which conforms to the 
    ''' method signature defined by the RuleHandler delegate.
    ''' </remarks>
    ''' <param name="handler">The method that implements the rule.</param>
    ''' <param name="args">
    ''' A RuleArgs object specifying the property name and other arguments
    ''' passed to the rule method
    ''' </param>
    Public Sub AddRule(ByVal handler As RuleHandler, ByVal args As RuleArgs)

      ' get the list of rules for the property
      Dim list As List(Of RuleMethod) = GetRulesForProperty(args.PropertyName)

      ' we have the list, add our new rule
      list.Add(New RuleMethod(mTarget, handler, args))

    End Sub

    '''' <summary>
    '''' Adds a rule to the list of rules to be enforced.
    '''' </summary>
    '''' <remarks>
    '''' <para>
    '''' A rule is implemented by a method which conforms to the 
    '''' method signature defined by the RuleHandler delegate.
    '''' </para><para>
    '''' The ruleName is used to group all the rules that apply
    '''' to a specific field, property or concept. All rules applying
    '''' to the field or property should have the same rule name. When
    '''' rules are checked, they can be checked globally or for a 
    '''' specific ruleName.
    '''' </para><para>
    '''' The propertyName may be used by the method that implements the rule
    '''' in order to retrieve the value to be validated. If the rule
    '''' implementation is inside the target object then it probably has
    '''' direct access to all data. However, if the rule implementation
    '''' is outside the target object then it will need to use reflection
    '''' or CallByName to dynamically invoke this property to retrieve
    '''' the value to be validated.
    '''' </para>
    '''' </remarks>
    '''' <param name="handler">The method that implements the rule.</param>
    '''' <param name="args">
    '''' A RuleArgs object specifying the property name and other arguments
    '''' passed to the rule method
    '''' </param>
    '''' <param name="ruleName">Unique name for the rule.</param>
    'Public Sub AddRule(ByVal handler As RuleHandler, ByVal args As RuleArgs, ByVal ruleName As String)

    '  ' get the list of rules for the property
    '  Dim list As List(Of RuleMethod) = GetRulesForProperty(args.PropertyName)

    '  ' we have the list, add our new rule
    '  list.Add(New RuleMethod(mTarget, handler, args, ruleName))

    'End Sub

#End Region

#Region " Checking Rules "

    Public Sub CheckRules(ByVal propertyName As String)

      Dim list As List(Of RuleMethod)
      ' get the list of rules to check
      If RulesList.ContainsKey(propertyName) Then
        list = RulesList.Item(propertyName)
        If list Is Nothing Then Exit Sub

        ' now check the rules
        Dim rule As RuleMethod
        For Each rule In list
          If rule.Invoke() Then
            BrokenRulesList.Remove(rule.ToString)
          Else
            BrokenRulesList.Add( _
              rule.RuleName, _
              rule.RuleArgs.Description, _
              rule.RuleArgs.PropertyName)
          End If
        Next
      End If

    End Sub

    Public Sub CheckRules()

      ' get the rules for each rule name
      Dim de As Generic.KeyValuePair(Of String, List(Of RuleMethod))
      For Each de In RulesList

        Dim list As List(Of RuleMethod) = _
          de.Value

        ' now check the rules
        Dim rule As RuleMethod
        For Each rule In list
          If rule.Invoke() Then
            BrokenRulesList.Remove(rule.ToString)
          Else
            BrokenRulesList.Add( _
              rule.RuleName, _
              rule.RuleArgs.Description, _
              rule.RuleArgs.PropertyName)
          End If
        Next
      Next

    End Sub

#End Region

#Region " Status retrieval "

    ''' <summary>
    ''' Returns a value indicating whether there are any broken rules
    ''' at this time. If there are broken rules, the business object
    ''' is assumed to be invalid and False is returned. If there are no
    ''' broken business rules True is returned.
    ''' </summary>
    ''' <returns>A value indicating whether any rules are broken.</returns>
    Friend ReadOnly Property IsValid() As Boolean
      Get
        Return BrokenRulesList.Count = 0
      End Get
    End Property

    ''' <summary>
    ''' Returns a reference to the readonly collection of broken
    ''' business rules.
    ''' </summary>
    ''' <remarks>
    ''' The reference returned points to the actual collection object.
    ''' This means that as rules are marked broken or unbroken over time,
    ''' the underlying data will change. Because of this, the UI developer
    ''' can bind a display directly to this collection to get a dynamic
    ''' display of the broken rules at all times.
    ''' </remarks>
    ''' <returns>A reference to the collection of broken rules.</returns>
    Public Function GetBrokenRules() As BrokenRulesCollection
      Return BrokenRulesList
    End Function

#End Region

  End Class

End Namespace
