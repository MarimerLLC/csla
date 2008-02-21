Namespace Validation

  ''' <summary>
  ''' Maintains rule methods for a business object
  ''' or business object type.
  ''' </summary>
  Friend Class ValidationRulesManager

    Private _rulesList As  _
      Generic.Dictionary(Of String, RulesList)

    Friend ReadOnly Property RulesDictionary() As Generic.Dictionary(Of String, RulesList)
      Get
        If _rulesList Is Nothing Then
          _rulesList = New Generic.Dictionary(Of String, RulesList)
        End If
        Return _rulesList
      End Get
    End Property

    ''' <summary>
    ''' Returns the list containing rules for a rule name. If
    ''' no list exists one is created and returned.
    ''' </summary>
    Friend Function GetRulesForProperty( _
      ByVal propertyName As String, _
      ByVal createList As Boolean) As RulesList

      ' get the list (if any) from the dictionary
      Dim list As RulesList = Nothing
      RulesDictionary.TryGetValue(propertyName, list)

      If createList AndAlso list Is Nothing Then
        ' there is no list for this name - create one
        list = New RulesList
        RulesDictionary.Add(propertyName, list)
      End If
      Return list

    End Function

#Region " Adding Rules "

    ''' <summary>
    ''' Adds a rule to the list of rules to be enforced.
    ''' </summary>
    Public Sub AddRule(ByVal handler As RuleHandler, ByVal args As RuleArgs, ByVal priority As Integer)

      ' get the list of rules for the property
      Dim list As List(Of IRuleMethod) = GetRulesForProperty(args.PropertyName, True).GetList(False)

      ' we have the list, add our new rule
      list.Add(New RuleMethod(handler, args, priority))

    End Sub

    ''' <summary>
    ''' Adds a rule to the list of rules to be enforced.
    ''' </summary>
    Public Sub AddRule(Of T, R As RuleArgs)( _
      ByVal handler As RuleHandler(Of T, R), ByVal args As R, ByVal priority As Integer)

      ' get the list of rules for the property
      Dim list As List(Of IRuleMethod) = GetRulesForProperty(args.PropertyName, True).GetList(False)

      ' we have the list, add our new rule
      list.Add(New RuleMethod(Of T, R)(handler, args, priority))

    End Sub

#End Region

#Region " Adding Dependencies "

    ''' <summary>
    ''' Adds a property to the list of dependencies for
    ''' the specified property
    ''' </summary>
    ''' <param name="propertyName">
    ''' The name of the property.
    ''' </param>
    ''' <param name="dependantPropertyName">
    ''' The name of the dependant property.
    ''' </param>
    ''' <remarks>
    ''' When rules are checked for propertyName, they will
    ''' also be checked for any dependant properties associated
    ''' with that property.
    ''' </remarks>
    Public Sub AddDependentProperty(ByVal propertyName As String, ByVal dependantPropertyName As String)

      ' get the list of rules for the property
      Dim list As List(Of String) = GetRulesForProperty(propertyName, True).GetDependancyList(True)

      ' we have the list, add the dependency
      list.Add(dependantPropertyName)

    End Sub

#End Region

  End Class

End Namespace