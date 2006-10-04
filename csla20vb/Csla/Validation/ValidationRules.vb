Namespace Validation

  ''' <summary>
  ''' Tracks the business rules broken within a business object.
  ''' </summary>
  <Serializable()> _
  Public Class ValidationRules

    ' list of broken rules for this business object
    Private mBrokenRules As BrokenRulesCollection
    ' threshold for short-circuiting to kick in
    Private mProcessThroughPriority As Integer
    ' reference to current business object
    <NonSerialized()> _
    Private mTarget As Object
    ' reference to per-instance rules manager for this object
    <NonSerialized()> _
    Private mInstanceRules As ValidationRulesManager
    ' reference to per-type rules manager for this object
    <NonSerialized()> _
    Private mTypeRules As ValidationRulesManager
    ' reference to the active set of rules for this object
    <NonSerialized()> _
    Private mRulesToCheck As ValidationRulesManager

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

    Private Function GetInstanceRules(ByVal createObject As Boolean) As ValidationRulesManager

      If mInstanceRules Is Nothing Then
        If createObject Then
          mInstanceRules = New ValidationRulesManager
        End If
      End If
      Return mInstanceRules

    End Function

    Private Function GetTypeRules(ByVal createObject As Boolean) As ValidationRulesManager

      If mTypeRules Is Nothing Then
        mTypeRules = SharedValidationRules.GetManager(mTarget.GetType, createObject)
      End If
      Return mTypeRules

    End Function

    Private ReadOnly Property RulesToCheck() As ValidationRulesManager
      Get
        If mRulesToCheck Is Nothing Then
          Dim instanceRules As ValidationRulesManager = GetInstanceRules(False)
          Dim typeRules As ValidationRulesManager = GetTypeRules(False)
          If instanceRules Is Nothing Then
            If typeRules Is Nothing Then
              mRulesToCheck = Nothing

            Else
              mRulesToCheck = typeRules
            End If

          ElseIf typeRules Is Nothing Then
            mRulesToCheck = instanceRules

          Else
            ' both have values - consolidate into instance rules
            mRulesToCheck = instanceRules
            For Each de As Generic.KeyValuePair(Of String, RulesList) In typeRules.RulesDictionary
              Dim rules As RulesList = mRulesToCheck.GetRulesForProperty(de.Key, True)
              Dim instanceList As List(Of IRuleMethod) = rules.GetList(False)
              instanceList.AddRange(de.Value.GetList(False))
              Dim dependancy As List(Of String) = _
                de.Value.GetDependancyList(False)
              If dependancy IsNot Nothing Then
                rules.GetDependancyList(True).AddRange(dependancy)
              End If
            Next
          End If
        End If
        Return mRulesToCheck
      End Get
    End Property

    ''' <summary>
    ''' Returns an array containing the text descriptions of all
    ''' validation rules associated with this object.
    ''' </summary>
    ''' <returns>String array.</returns>
    ''' <remarks></remarks>
    Public Function GetRuleDescriptions() As String()

      Dim result As New List(Of String)
      Dim rules As ValidationRulesManager = RulesToCheck
      For Each de As Generic.KeyValuePair(Of String, RulesList) In rules.RulesDictionary
        For Each rule As IRuleMethod In de.Value.GetList(False)
          result.Add(CObj(rule).ToString)
        Next
      Next
      Return result.ToArray

    End Function

#Region " Short-Circuiting "

    ''' <summary>
    ''' Gets or sets the priority through which
    ''' CheckRules should process before short-circuiting
    ''' processing on broken rules.
    ''' </summary>
    ''' <value>Defaults to 0.</value>
    ''' <remarks>
    ''' All rules for each property are processed by CheckRules
    ''' though this priority. Rules with lower priorities are
    ''' only processed if no previous rule has been marked as
    ''' broken.
    ''' </remarks>
    Public Property ProcessThroughPriority() As Integer
      Get
        Return mProcessThroughPriority
      End Get
      Set(ByVal value As Integer)
        mProcessThroughPriority = value
      End Set
    End Property

#End Region

#Region " Adding Instance Rules "

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
    Public Sub AddInstanceRule( _
      ByVal handler As RuleHandler, ByVal propertyName As String)

      GetInstanceRules(True).AddRule(handler, New RuleArgs(propertyName), 0)

    End Sub

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
    ''' <param name="priority">
    ''' The priority of the rule, where lower numbers are processed first.
    ''' </param>
    Public Sub AddInstanceRule( _
      ByVal handler As RuleHandler, ByVal propertyName As String, _
      ByVal priority As Integer)

      GetInstanceRules(True).AddRule(handler, New RuleArgs(propertyName), priority)

    End Sub

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
    ''' <typeparam name="T">Type of the business object to be validated.</typeparam>
    Public Sub AddInstanceRule(Of T)( _
      ByVal handler As RuleHandler(Of T, RuleArgs), ByVal propertyName As String)

      GetInstanceRules(True).AddRule(Of T, RuleArgs)(handler, New RuleArgs(propertyName), 0)

    End Sub

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
    ''' <param name="priority">
    ''' The priority of the rule, where lower numbers are processed first.
    ''' </param>
    ''' <typeparam name="T">Type of the business object to be validated.</typeparam>
    Public Sub AddInstanceRule(Of T)( _
      ByVal handler As RuleHandler(Of T, RuleArgs), ByVal propertyName As String, _
      ByVal priority As Integer)

      GetInstanceRules(True).AddRule(Of T, RuleArgs)(handler, New RuleArgs(propertyName), priority)

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
    Public Sub AddInstanceRule(ByVal handler As RuleHandler, ByVal args As RuleArgs)

      GetInstanceRules(True).AddRule(handler, args, 0)

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
    ''' <param name="priority">
    ''' The priority of the rule, where lower numbers are processed first.
    ''' </param>
    Public Sub AddInstanceRule(ByVal handler As RuleHandler, ByVal args As RuleArgs, _
      ByVal priority As Integer)

      GetInstanceRules(True).AddRule(handler, args, priority)

    End Sub

    ''' <summary>
    ''' Adds a rule to the list of rules to be enforced.
    ''' </summary>
    ''' <remarks>
    ''' A rule is implemented by a method which conforms to the 
    ''' method signature defined by the RuleHandler delegate.
    ''' </remarks>
    ''' <typeparam name="T">Type of the target object.</typeparam>
    ''' <typeparam name="R">Type of the arguments parameter.</typeparam>
    ''' <param name="handler">The method that implements the rule.</param>
    ''' <param name="args">
    ''' A RuleArgs object specifying the property name and other arguments
    ''' passed to the rule method
    ''' </param>
    Public Sub AddInstanceRule(Of T, R As RuleArgs)( _
      ByVal handler As RuleHandler(Of T, R), ByVal args As R)

      GetInstanceRules(True).AddRule(handler, args, 0)

    End Sub

    ''' <summary>
    ''' Adds a rule to the list of rules to be enforced.
    ''' </summary>
    ''' <remarks>
    ''' A rule is implemented by a method which conforms to the 
    ''' method signature defined by the RuleHandler delegate.
    ''' </remarks>
    ''' <typeparam name="T">Type of the target object.</typeparam>
    ''' <typeparam name="R">Type of the arguments parameter.</typeparam>
    ''' <param name="handler">The method that implements the rule.</param>
    ''' <param name="args">
    ''' A RuleArgs object specifying the property name and other arguments
    ''' passed to the rule method
    ''' </param>
    ''' <param name="priority">
    ''' The priority of the rule, where lower numbers are processed first.
    ''' </param>
    Public Sub AddInstanceRule(Of T, R As RuleArgs)( _
      ByVal handler As RuleHandler(Of T, R), ByVal args As R, _
      ByVal priority As Integer)

      GetInstanceRules(True).AddRule(handler, args, priority)

    End Sub

#End Region

#Region " Adding Shared Rules "

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
    Public Sub AddRule( _
      ByVal handler As RuleHandler, ByVal propertyName As String)

      ValidateHandler(handler)
      GetTypeRules(True).AddRule(handler, New RuleArgs(propertyName), 0)

    End Sub

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
    ''' <param name="priority">
    ''' The priority of the rule, where lower numbers are processed first.
    ''' </param>
    Public Sub AddRule( _
      ByVal handler As RuleHandler, ByVal propertyName As String, ByVal priority As Integer)

      ValidateHandler(handler)
      GetTypeRules(True).AddRule(handler, New RuleArgs(propertyName), priority)

    End Sub

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
    Public Sub AddRule(Of T)( _
      ByVal handler As RuleHandler(Of T, RuleArgs), ByVal propertyName As String)

      ValidateHandler(handler)
      GetTypeRules(True).AddRule(Of T, RuleArgs)(handler, New RuleArgs(propertyName), 0)

    End Sub

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
    ''' <param name="priority">
    ''' The priority of the rule, where lower numbers are processed first.
    ''' </param>
    Public Sub AddRule(Of T)( _
      ByVal handler As RuleHandler(Of T, RuleArgs), ByVal propertyName As String, ByVal priority As Integer)

      ValidateHandler(handler)
      GetTypeRules(True).AddRule(Of T, RuleArgs)(handler, New RuleArgs(propertyName), priority)

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

      ValidateHandler(handler)
      GetTypeRules(True).AddRule(handler, args, 0)

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
    ''' <param name="priority">
    ''' The priority of the rule, where lower numbers are processed first.
    ''' </param>
    Public Sub AddRule(ByVal handler As RuleHandler, ByVal args As RuleArgs, ByVal priority As Integer)

      ValidateHandler(handler)
      GetTypeRules(True).AddRule(handler, args, priority)

    End Sub

    ''' <summary>
    ''' Adds a rule to the list of rules to be enforced.
    ''' </summary>
    ''' <remarks>
    ''' A rule is implemented by a method which conforms to the 
    ''' method signature defined by the RuleHandler delegate.
    ''' </remarks>
    ''' <typeparam name="T">Type of the target object.</typeparam>
    ''' <typeparam name="R">Type of the arguments parameter.</typeparam>
    ''' <param name="handler">The method that implements the rule.</param>
    ''' <param name="args">
    ''' A RuleArgs object specifying the property name and other arguments
    ''' passed to the rule method
    ''' </param>
    Public Sub AddRule(Of T, R As RuleArgs)( _
      ByVal handler As RuleHandler(Of T, R), ByVal args As R)

      ValidateHandler(handler)
      GetTypeRules(True).AddRule(handler, args, 0)

    End Sub

    ''' <summary>
    ''' Adds a rule to the list of rules to be enforced.
    ''' </summary>
    ''' <remarks>
    ''' A rule is implemented by a method which conforms to the 
    ''' method signature defined by the RuleHandler delegate.
    ''' </remarks>
    ''' <typeparam name="T">Type of the target object.</typeparam>
    ''' <typeparam name="R">Type of the arguments parameter.</typeparam>
    ''' <param name="handler">The method that implements the rule.</param>
    ''' <param name="args">
    ''' A RuleArgs object specifying the property name and other arguments
    ''' passed to the rule method
    ''' </param>
    ''' <param name="priority">
    ''' The priority of the rule, where lower numbers are processed first.
    ''' </param>
    Public Sub AddRule(Of T, R As RuleArgs)( _
      ByVal handler As RuleHandler(Of T, R), ByVal args As R, ByVal priority As Integer)

      ValidateHandler(handler)
      GetTypeRules(True).AddRule(handler, args, priority)

    End Sub

    Private Function ValidateHandler(ByVal handler As RuleHandler) As Boolean

      Return ValidateHandler(handler.Method)

    End Function

    Private Function ValidateHandler(Of T, R As RuleArgs)(ByVal handler As RuleHandler(Of T, R)) As Boolean

      Return ValidateHandler(handler.Method)

    End Function

    Private Function ValidateHandler(ByVal method As System.Reflection.MethodInfo) As Boolean

      If Not method.IsStatic AndAlso method.DeclaringType.Equals(mTarget.GetType) Then
        Throw New InvalidOperationException( _
          String.Format("{0}: {1}", _
          My.Resources.InvalidRuleMethodException, method.Name))
      End If
      Return True

    End Function
#End Region

#Region " Adding per-type dependencies "

    ''' <summary>
    ''' Adds a property to the list of dependencies for
    ''' the specified property
    ''' </summary>
    ''' <param name="propertyName">
    ''' The name of the property.
    ''' </param>
    ''' <param name="dependantPropertyName">
    ''' The name of the depandent property.
    ''' </param>
    ''' <remarks>
    ''' When rules are checked for propertyName, they will
    ''' also be checked for any dependant properties associated
    ''' with that property.
    ''' </remarks>
    Public Sub AddDependantProperty(ByVal propertyName As String, ByVal dependantPropertyName As String)

      GetTypeRules(True).AddDependantProperty(propertyName, dependantPropertyName)

    End Sub

#End Region

#Region " Checking Rules "

    ''' <summary>
    ''' Invokes all rule methods associated with
    ''' the specified property and any 
    ''' dependant properties.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to validate.</param>
    Public Sub CheckRules(ByVal propertyName As String)

      ' get the rules dictionary
      Dim rules As ValidationRulesManager = RulesToCheck
      If rules IsNot Nothing Then
        ' get the rules list for this property
        Dim rulesList As RulesList = rules.GetRulesForProperty(propertyName, False)
        If rulesList IsNot Nothing Then
          ' get the actual list of rules (sorted by priority)
          Dim list As List(Of IRuleMethod) = rulesList.GetList(True)
          If list IsNot Nothing Then
            CheckRules(list)
          End If
          Dim dependancies As List(Of String) = rulesList.GetDependancyList(False)
          If dependancies IsNot Nothing Then
            For Each dependantProperty As String In dependancies
              CheckRules(rules, dependantProperty)
            Next
          End If
        End If
      End If

    End Sub

    Private Sub CheckRules(ByVal rules As ValidationRulesManager, ByVal propertyName As String)

      ' get the rules list for this property
      Dim rulesList As RulesList = rules.GetRulesForProperty(propertyName, False)
      If rulesList IsNot Nothing Then
        ' get the actual list of rules (sorted by priority)
        Dim list As List(Of IRuleMethod) = rulesList.GetList(True)
        If list IsNot Nothing Then
          CheckRules(list)
        End If
      End If

    End Sub

    ''' <summary>
    ''' Invokes all rule methods for all properties
    ''' in the object.
    ''' </summary>
    Public Sub CheckRules()

      Dim rules As ValidationRulesManager = RulesToCheck
      If rules IsNot Nothing Then
        For Each de As Generic.KeyValuePair(Of String, RulesList) In rules.RulesDictionary
          CheckRules(de.Value.GetList(True))
        Next
      End If

    End Sub

    ''' <summary>
    ''' Given a list
    ''' containing IRuleMethod objects, this
    ''' method executes all those rule methods.
    ''' </summary>
    Private Sub CheckRules(ByVal list As List(Of IRuleMethod))

      Dim previousRuleBroken As Boolean
      Dim shortCircuited As Boolean

      For Each rule As IRuleMethod In list
        ' see if short-circuiting should kick in
        If Not shortCircuited AndAlso (previousRuleBroken AndAlso rule.Priority > mProcessThroughPriority) Then
          shortCircuited = True
        End If

        If shortCircuited Then
          ' we're short-circuited, so just remove
          ' all remaining broken rule entries
          BrokenRulesList.Remove(rule)

        Else
          ' we're not short-circuited, so check rule
          If rule.Invoke(mTarget) Then
            ' the rule is not broken
            BrokenRulesList.Remove(rule)

          Else
            ' the rule is broken
            BrokenRulesList.Add(rule)
            Dim args As RuleArgs = rule.RuleArgs
            If args.Severity = RuleSeverity.Error Then
              previousRuleBroken = True
            End If
            If args.StopProcessing Then
              shortCircuited = True
            End If
          End If
        End If
      Next

    End Sub

#End Region

#Region " Status retrieval "

    ''' <summary>
    ''' Returns a value indicating whether there are any broken rules
    ''' at this time. 
    ''' </summary>
    ''' <returns>A value indicating whether any rules are broken.</returns>
    Friend ReadOnly Property IsValid() As Boolean
      Get
        Return BrokenRulesList.ErrorCount = 0
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
