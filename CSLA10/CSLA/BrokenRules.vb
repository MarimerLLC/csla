Imports System.Collections.Specialized

''' <summary>
''' Tracks the business rules broken within a business object.
''' </summary>
<Serializable()> _
Public Class BrokenRules

#Region " Rule structure "

  ''' <summary>
  ''' Stores details about a specific broken business rule.
  ''' </summary>
  <Serializable()> _
  Public Structure Rule
    Private mRule As String
    Private mDescription As String
    Private mProperty As String

    Friend Sub New(ByVal Rule As String, ByVal Description As String)
      mRule = Rule
      mDescription = Description
    End Sub

    Friend Sub New(ByVal Rule As String, ByVal Description As String, ByVal [Property] As String)
      mRule = Rule
      mDescription = Description
      mProperty = [Property]
    End Sub

    ''' <summary>
    ''' Provides access to the name of the broken rule.
    ''' </summary>
    ''' <remarks>
    ''' This value is actually readonly, not readwrite. Any new
    ''' value set into this property is ignored. The property is only
    ''' readwrite because that is required to support data binding
    ''' within Web Forms.
    ''' </remarks>
    ''' <value>The name of the rule.</value>
    Public Property Rule() As String
      Get
        Return mRule
      End Get
      Set(ByVal Value As String)
        ' the property must be read-write for Web Forms data binding
        ' to work, but we really don't want to allow the value to be
        ' changed dynamically so we ignore any attempt to set it
      End Set
    End Property

    ''' <summary>
    ''' Provides access to the description of the broken rule.
    ''' </summary>
    ''' <remarks>
    ''' This value is actually readonly, not readwrite. Any new
    ''' value set into this property is ignored. The property is only
    ''' readwrite because that is required to support data binding
    ''' within Web Forms.
    ''' </remarks>
    ''' <value>The description of the rule.</value>
    Public Property Description() As String
      Get
        Return mDescription
      End Get
      Set(ByVal Value As String)
        ' the property must be read-write for Web Forms data binding
        ' to work, but we really don't want to allow the value to be
        ' changed dynamically so we ignore any attempt to set it
      End Set
    End Property

    ''' <summary>
    ''' Provides access to the property affected by the broken rule.
    ''' </summary>
    ''' <remarks>
    ''' This value is actually readonly, not readwrite. Any new
    ''' value set into this property is ignored. The property is only
    ''' readwrite because that is required to support data binding
    ''' within Web Forms.
    ''' </remarks>
    ''' <value>The property affected by the rule.</value>
    Public Property [Property]() As String
      Get
        Return mProperty
      End Get
      Set(ByVal Value As String)
        ' the property must be read-write for Web Forms data binding
        ' to work, but we really don't want to allow the value to be
        ' changed dynamically so we ignore any attempt to set it
      End Set
    End Property
  End Structure

#End Region

#Region " RulesCollection "

  ''' <summary>
  ''' A collection of currently broken rules.
  ''' </summary>
  ''' <remarks>
  ''' This collection is readonly and can be safely made available
  ''' to code outside the business object such as the UI. This allows
  ''' external code, such as a UI, to display the list of broken rules
  ''' to the user.
  ''' </remarks>
  <Serializable()> _
  Public Class RulesCollection
    Inherits CSLA.Core.BindableCollectionBase

    Private mLegal As Boolean = False

    ''' <summary>
    ''' Returns a <see cref="T:CSLA.BrokenRules.Rule" /> object
    ''' containing details about a specific broken business rule.
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Default Public ReadOnly Property Item(ByVal Index As Integer) As Rule
      Get
        Return CType(list.Item(Index), Rule)
      End Get
    End Property

    ''' <summary>
    ''' Returns the first <see cref="T:CSLA.BrokenRules.Rule" /> object
    ''' corresponding to the specified property.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' When a rule is marked as broken, the business developer can provide
    ''' an optional Property parameter. This parameter is the name of the
    ''' Property on the object that is most affected by the rule. Data binding
    ''' may later use the IDataErrorInfo interface to query the object for
    ''' details about errors corresponding to specific properties, and this
    ''' value will be returned as a result of that query.
    ''' </para><para>
    ''' Code in a business object or UI can also use this value to retrieve
    ''' the first broken rule in <see cref="T:CSLA.BrokenRules" /> that corresponds
    ''' to a specfic Property on the object.
    ''' </para>
    ''' </remarks>
    ''' <param name="Property">The name of the property affected by the rule.</param>
    Public ReadOnly Property RuleForProperty(ByVal [Property] As String) As Rule
      Get
        Dim item As Rule

        For Each item In list
          If item.Property = [Property] Then
            Return item
          End If
        Next
        Return New Rule()
      End Get
    End Property

    Friend Sub New()
      AllowEdit = False
      AllowRemove = False
      AllowNew = False
    End Sub

    Friend Sub Add(ByVal Rule As String, ByVal Description As String)
      Remove(Rule)
      mLegal = True
      list.Add(New Rule(Rule, Description))
      mLegal = False
    End Sub

    Friend Sub Add(ByVal Rule As String, ByVal Description As String, ByVal [Property] As String)
      Remove(Rule)
      mLegal = True
      list.Add(New Rule(Rule, Description, [Property]))
      mLegal = False
    End Sub

    Friend Sub Remove(ByVal Rule As String)
      Dim index As Integer

      ' we loop through using a numeric counter because
      ' the base class Remove requires a numberic index
      mLegal = True
      For index = 0 To list.Count - 1
        If CType(list.Item(index), Rule).Rule = Rule Then
          list.Remove(list.Item(index))
          Exit For
        End If
      Next
      mLegal = False
    End Sub

    Friend Function Contains(ByVal Rule As String) As Boolean
      Dim index As Integer

      For index = 0 To list.Count - 1
        If CType(list.Item(index), Rule).Rule = Rule Then
          Return True
        End If
      Next
      Return False
    End Function

    ''' <summary>
    ''' Prevents clearing the collection.
    ''' </summary>
    Protected Overrides Sub OnClear()
      If Not mLegal Then
        Throw New NotSupportedException("Clear is an invalid operation")
      End If
    End Sub

    ''' <summary>
    ''' Prevents insertion of items into the collection.
    ''' </summary>
    Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)
      If Not mLegal Then
        Throw New NotSupportedException("Insert is an invalid operation")
      End If
    End Sub

    ''' <summary>
    ''' Prevents removal of items from the collection.
    ''' </summary>
    Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)
      If Not mLegal Then
        Throw New NotSupportedException("Remove is an invalid operation")
      End If
    End Sub

    ''' <summary>
    ''' Prevents changing items in the collection.
    ''' </summary>
    Protected Overrides Sub OnSet(ByVal index As Integer, _
        ByVal oldValue As Object, ByVal newValue As Object)
      If Not mLegal Then
        Throw New NotSupportedException("Changing an element is an invalid operation")
      End If
    End Sub
  End Class

#End Region

  Private mBrokenRules As New RulesCollection()
  <NonSerialized(), NotUndoable()> _
  Private mTarget As Object

#Region " Rule Manager "

  ''' <summary>
  ''' Sets the target object so the Rules Manager functionality
  ''' has a reference to the object containing the data to
  ''' be validated.
  ''' </summary>
  ''' <remarks>
  ''' The object here is typically your business object. In your
  ''' business class you'll implement a method to set up your
  ''' business rules. As you do so, you need to call this method
  ''' to give BrokenRules a reference to your business object
  ''' so it has access to your object's data.
  ''' </remarks>
  ''' <param name="target">A reference to the object containing
  ''' the data to be validated.</param>
  Public Sub SetTargetObject(ByVal target As Object)
    mTarget = target
  End Sub

#Region " RuleHandler delegate "

  ''' <summary>
  ''' Delegate that defines the method signature for all rule handler methods.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' When implementing a rule handler, you must conform to the method signature
  ''' defined by this delegate. You should also apply the Description attribute
  ''' to your method to provide a meaningful description for your rule.
  ''' </para><para>
  ''' The method implementing the rule must return True if the data is valid and
  ''' return False if the data is invalid.
  ''' </para>
  ''' </remarks>
  Public Delegate Function RuleHandler(ByVal target As Object, ByVal e As RuleArgs) As Boolean

#End Region

#Region " RuleArgs class "

  ''' <summary>
  ''' Object providing extra information to methods that
  ''' implement business rules.
  ''' </summary>
  Public Class RuleArgs
    Private mPropertyName As String

    ''' <summary>
    ''' The (optional) name of the property to be validated.
    ''' </summary>
    Public ReadOnly Property PropertyName() As String
      Get
        Return mPropertyName
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    Public Sub New()

    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    Public Sub New(ByVal propertyName As String)
      mPropertyName = propertyName
    End Sub

#Region " Empty "

    Private Shared mEmptyArgs As New RuleArgs()

    ''' <summary>
    ''' Returns an empty RuleArgs object.
    ''' </summary>
    Public Shared ReadOnly Property Empty() As RuleArgs
      Get
        Return mEmptyArgs
      End Get
    End Property

#End Region

  End Class

#End Region

#Region " Description attribute "

  ''' <summary>
  ''' Defines the description of a business rule.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' The description in this attribute is used by BusinessRules
  ''' as information that is provided to the UI or other consumer
  ''' about which rules are broken. These descriptions are intended
  ''' for end-user display.
  ''' </para><para>
  ''' The description value is a .NET format string, and it can include
  ''' the following tokens in addition to literal text:
  ''' </para><para>
  ''' {0} - the RuleName value
  ''' </para><para>
  ''' {1} - the PropertyName value
  ''' </para><para>
  ''' {2} - the full type name of the target object
  ''' </para><para>
  ''' {3} - the ToString value of the target object
  ''' </para><para>
  ''' You can use these tokens in your description string and the
  ''' appropriate values will be substituted for the tokens at
  ''' runtime.
  ''' </para>
  ''' </remarks>
  <AttributeUsage(AttributeTargets.Method)> _
  Public Class DescriptionAttribute
    Inherits Attribute

    Private mText As String = ""

    ''' <summary>
    ''' Initializes the attribute with a description.
    ''' </summary>
    Public Sub New(ByVal description As String)
      mText = description
    End Sub

    ''' <summary>
    ''' Returns the description value of the attribute.
    ''' </summary>
    Public Overrides Function ToString() As String
      Return mText
    End Function

  End Class

#End Region

#Region " RuleMethod Class "

  ''' <summary>
  ''' Tracks all information for a rule.
  ''' </summary>
  Private Class RuleMethod
    Private mHandler As RuleHandler
    Private mTarget As Object
    Private mRuleName As String
    Private mArgs As RuleArgs
    Private mDescription As String

    ''' <summary>
    ''' Returns the name of the method implementing the rule
    ''' and the property, field or column name to which the
    ''' rule applies.
    ''' </summary>
    Public Overrides Function ToString() As String
      If RuleArgs.PropertyName Is Nothing Then
        Return mHandler.Method.Name

      Else
        Return mHandler.Method.Name & "!" & RuleArgs.PropertyName
      End If
    End Function

    ''' <summary>
    ''' Returns the delegate to the method implementing the rule.
    ''' </summary>
    Public ReadOnly Property Handler() As RuleHandler
      Get
        Return mHandler
      End Get
    End Property

    ''' <summary>
    ''' Returns the user-friendly name of the rule.
    ''' </summary>
    Public ReadOnly Property RuleName() As String
      Get
        Return mRuleName
      End Get
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
    ''' Returns the formatted description of the rule.
    ''' </summary>
    Public ReadOnly Property Description() As String
      Get
        Return String.Format(mDescription, RuleName, RuleArgs.PropertyName, TypeName(mTarget), mTarget.ToString)
      End Get
    End Property

    ''' <summary>
    ''' Retrieves the description text from the Description
    ''' attribute on a RuleHandler method.
    ''' </summary>
    Private Function GetDescription(ByVal handler As RuleHandler) As String

      Dim attrib() As Object = handler.Method.GetCustomAttributes(GetType(DescriptionAttribute), False)
      If attrib.Length > 0 Then
        Return attrib(0).ToString

      Else
        Return "{2}.{0}:<no description>"
      End If

    End Function

    ''' <summary>
    ''' Creates and initializes the rule.
    ''' </summary>
    ''' <param name="target">Reference to the object containing the data to validate.</param>
    ''' <param name="handler">The address of the method implementing the rule.</param>
    ''' <param name="ruleName">The user-friendly name of the rule.</param>
    ''' <param name="ruleArgs">A RuleArgs object containing data related to the rule.</param>
    Public Sub New(ByVal target As Object, ByVal handler As RuleHandler, ByVal ruleName As String, ByVal ruleArgs As RuleArgs)

      mTarget = target
      mHandler = handler
      mDescription = GetDescription(handler)
      mRuleName = ruleName
      mArgs = ruleArgs

    End Sub

    ''' <summary>
    ''' Creates and initializes the rule.
    ''' </summary>
    ''' <param name="target">Reference to the object containing the data to validate.</param>
    ''' <param name="handler">The address of the method implementing the rule.</param>
    ''' <param name="ruleName">The user-friendly name of the rule.</param>
    ''' <param name="propertyName">The field, property or column to which the rule applies.</param>
    Public Sub New(ByVal target As Object, ByVal handler As RuleHandler, ByVal ruleName As String, ByVal propertyName As String)

      mTarget = target
      mHandler = handler
      mDescription = GetDescription(handler)
      mRuleName = ruleName
      mArgs = New RuleArgs(propertyName)

    End Sub

    ''' <summary>
    ''' Invokes the rule to validate the data.
    ''' </summary>
    ''' <returns>True if the data is valid, False if the data is invalid.</returns>
    Public Function Invoke() As Boolean
      Return mHandler.Invoke(mTarget, mArgs)
    End Function

  End Class

#End Region

#Region " RulesList property "

  <NonSerialized(), NotUndoable()> _
  Private mRulesList As HybridDictionary

  Private ReadOnly Property RulesList() As HybridDictionary
    Get
      If mRulesList Is Nothing Then
        mRulesList = New HybridDictionary()
      End If
      Return mRulesList
    End Get
  End Property

#End Region

#Region " Adding Rules "

  ''' <summary>
  ''' Returns the ArrayList containing rules for a rule name. If
  ''' no ArrayList exists one is created and returned.
  ''' </summary>
  Private Function GetRulesForName(ByVal ruleName As String) As ArrayList

    ' get the ArrayList (if any) from the Hashtable
    Dim list As ArrayList = CType(RulesList.Item(ruleName), ArrayList)
    If list Is Nothing Then
      ' there is no list for this name - create one
      list = New ArrayList()
      RulesList.Add(ruleName, list)
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
  ''' The ruleName is used to group all the rules that apply
  ''' to a specific field, property or concept. All rules applying
  ''' to the field or property should have the same rule name. When
  ''' rules are checked, they can be checked globally or for a 
  ''' specific ruleName.
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
  ''' <param name="ruleName">
  ''' A user-friendly identifier for the field/property 
  ''' to which the rule applies.
  ''' </param>
  Public Sub AddRule(ByVal handler As RuleHandler, ByVal ruleName As String)

    ' get the ArrayList (if any) from the Hashtable
    Dim list As ArrayList = GetRulesForName(ruleName)

    ' we have the list, add our new rule
    list.Add(New RuleMethod(mTarget, handler, ruleName, RuleArgs.Empty))

  End Sub

  ''' <summary>
  ''' Adds a rule to the list of rules to be enforced.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' A rule is implemented by a method which conforms to the 
  ''' method signature defined by the RuleHandler delegate.
  ''' </para><para>
  ''' The ruleName is used to group all the rules that apply
  ''' to a specific field, property or concept. All rules applying
  ''' to the field or property should have the same rule name. When
  ''' rules are checked, they can be checked globally or for a 
  ''' specific ruleName.
  ''' </para>
  ''' </remarks>
  ''' <param name="handler">The method that implements the rule.</param>
  ''' <param name="ruleName">
  ''' A user-friendly identifier for the field/property 
  ''' to which the rule applies.
  ''' </param>
  ''' <param name="ruleArgs">A RuleArgs object containing data
  ''' to be passed to the method implementing the rule.</param>
  Public Sub AddRule(ByVal handler As RuleHandler, ByVal ruleName As String, ByVal ruleArgs As RuleArgs)

    ' get the ArrayList (if any) from the Hashtable
    Dim list As ArrayList = GetRulesForName(ruleName)

    ' we have the list, add our new rule
    list.Add(New RuleMethod(mTarget, handler, ruleName, ruleArgs))

  End Sub

  ''' <summary>
  ''' Adds a rule to the list of rules to be enforced.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' A rule is implemented by a method which conforms to the 
  ''' method signature defined by the RuleHandler delegate.
  ''' </para><para>
  ''' The ruleName is used to group all the rules that apply
  ''' to a specific field, property or concept. All rules applying
  ''' to the field or property should have the same rule name. When
  ''' rules are checked, they can be checked globally or for a 
  ''' specific ruleName.
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
  ''' <param name="ruleName">
  ''' A user-friendly identifier for the field/property 
  ''' to which the rule applies.
  ''' </param>
  ''' <param name="propertyName">
  ''' The property name on the target object where the rule implementation can retrieve
  ''' the value to be validated.
  ''' </param>
  Public Sub AddRule(ByVal handler As RuleHandler, ByVal ruleName As String, ByVal propertyName As String)

    ' get the ArrayList (if any) from the Hashtable
    Dim list As ArrayList = GetRulesForName(ruleName)

    ' we have the list, add our new rule
    list.Add(New RuleMethod(mTarget, handler, ruleName, propertyName))

  End Sub

#End Region

#Region " Checking Rules "

  ''' <summary>
  ''' Checks all the rules for a specific ruleName.
  ''' </summary>
  ''' <param name="ruleName">The ruleName to be validated.</param>
  Public Sub CheckRules(ByVal ruleName As String)
    Dim list As ArrayList

    ' get the list of rules to check
    list = CType(RulesList.Item(ruleName), ArrayList)
    If list Is Nothing Then Exit Sub

    ' now check the rules
    Dim rule As RuleMethod
    For Each rule In list
      If rule.Invoke() Then
        UnBreakRule(rule)
      Else
        BreakRule(rule)
      End If
    Next

  End Sub

  ''' <summary>
  ''' Checks all the rules for a target object.
  ''' </summary>
  Public Sub CheckRules()

    ' get the rules for each rule name
    Dim de As DictionaryEntry
    For Each de In RulesList

      Dim list As ArrayList

      list = CType(de.Value, ArrayList)

      ' now check the rules
      Dim rule As RuleMethod
      For Each rule In list
        If rule.Invoke() Then
          UnBreakRule(rule)
        Else
          BreakRule(rule)
        End If
      Next
    Next

  End Sub

  Private Sub UnBreakRule(ByVal rule As RuleMethod)
    If rule.RuleArgs.PropertyName Is Nothing Then
      Assert(rule.ToString, "", False)
    Else
      Assert(rule.ToString, "", rule.RuleArgs.PropertyName, False)
    End If
  End Sub

  Private Sub BreakRule(ByVal rule As RuleMethod)
    If rule.RuleArgs.PropertyName Is Nothing Then
      Assert(rule.ToString, rule.Description, True)
    Else
      Assert(rule.ToString, rule.Description, rule.RuleArgs.PropertyName, True)
    End If
  End Sub

#End Region

#End Region ' Rule Manager

#Region " Assert methods "

  ''' <summary>
  ''' This method is called by business logic within a business class to
  ''' indicate whether a business rule is broken.
  ''' </summary>
  ''' <remarks>
  ''' Rules are identified by their names. The description field is merely a 
  ''' comment that is used for display to the end user. When a rule is marked as
  ''' broken, it is recorded under the rule name value. To mark the rule as not
  ''' broken, the same rule name must be used.
  ''' </remarks>
  ''' <param name="Rule">The name of the business rule.</param>
  ''' <param name="Description">The description of the business rule.</param>
  ''' <param name="IsBroken">True if the value is broken, False if it is not broken.</param>
  Public Sub Assert(ByVal Rule As String, ByVal Description As String, ByVal IsBroken As Boolean)
    If IsBroken Then
      mBrokenRules.Add(Rule, Description)
    Else
      mBrokenRules.Remove(Rule)
    End If
  End Sub

  ''' <summary>
  ''' This method is called by business logic within a business class to
  ''' indicate whether a business rule is broken.
  ''' </summary>
  ''' <remarks>
  ''' Rules are identified by their names. The description field is merely a 
  ''' comment that is used for display to the end user. When a rule is marked as
  ''' broken, it is recorded under the rule name value. To mark the rule as not
  ''' broken, the same rule name must be used.
  ''' </remarks>
  ''' <param name="Rule">The name of the business rule.</param>
  ''' <param name="Description">The description of the business rule.</param>
  ''' <param name="Property">The property affected by the business rule.</param>
  ''' <param name="IsBroken">True if the value is broken, False if it is not broken.</param>
  Public Sub Assert(ByVal Rule As String, ByVal Description As String, ByVal [Property] As String, ByVal IsBroken As Boolean)
    If IsBroken Then
      mBrokenRules.Add(Rule, Description, [Property])
    Else
      mBrokenRules.Remove(Rule)
    End If
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
  Public ReadOnly Property IsValid() As Boolean
    Get
      Return mBrokenRules.Count = 0
    End Get
  End Property

  ''' <summary>
  ''' Returns a value indicating whether a particular business rule
  ''' is currently broken.
  ''' </summary>
  ''' <param name="Rule">The name of the rule to check.</param>
  ''' <returns>A value indicating whether the rule is currently broken.</returns>
  Public Function IsBroken(ByVal Rule As String) As Boolean
    Return mBrokenRules.Contains(Rule)
  End Function

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
  Public Function GetBrokenRules() As RulesCollection
    Return mBrokenRules
  End Function

  ''' <summary>
  ''' Returns the text of all broken rule descriptions, each
  ''' separated by cr/lf.
  ''' </summary>
  ''' <returns>The text of all broken rule descriptions.</returns>
  Public Overrides Function ToString() As String
    Dim obj As New System.Text.StringBuilder()
    Dim item As Rule
    Dim first As Boolean = True

    For Each item In mBrokenRules
      If first Then
        first = False
      Else
        obj.Append(vbCrLf)
      End If
      obj.Append(item.Description)
    Next
    Return obj.ToString
  End Function

#End Region

End Class
