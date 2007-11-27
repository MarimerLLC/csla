Namespace Validation

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
  Public Class BrokenRulesCollection
    Inherits Core.ReadOnlyBindingList(Of BrokenRule)

    Private mErrorCount As Integer
    Private mWarningCount As Integer
    Private mInfoCount As Integer

    ''' <summary>
    ''' Gets the number of broken rules in
    ''' the collection that have a severity
    ''' of Error.
    ''' </summary>
    ''' <value>An integer value.</value>
    Public ReadOnly Property ErrorCount() As Integer
      Get
        Return mErrorCount
      End Get
    End Property

    ''' <summary>
    ''' Gets the number of broken rules in
    ''' the collection that have a severity
    ''' of Warning.
    ''' </summary>
    ''' <value>An integer value.</value>
    Public ReadOnly Property WarningCount() As Integer
      Get
        Return mWarningCount
      End Get
    End Property


    ''' <summary>
    ''' Gets the number of broken rules in
    ''' the collection that have a severity
    ''' of Information.
    ''' </summary>
    ''' <value>An integer value.</value>
    Public ReadOnly Property InformationCount() As Integer
      Get
        Return mInfoCount
      End Get
    End Property

    ''' <summary>
    ''' Returns the first <see cref="BrokenRule" /> object
    ''' corresponding to the specified property with Error severity.
    ''' </summary>
    ''' <remarks>
    ''' Code in a business object or UI can also use this value to retrieve
    ''' the first broken rule in <see cref="BrokenRulesCollection" /> that corresponds
    ''' to a specfic Property on the object.
    ''' </remarks>
    ''' <param name="property">The name of the property affected by the rule.</param>
    ''' <returns>
    ''' The first BrokenRule object corresponding to the specified property, or Nothing
    ''' (null in C#) if there are no rules defined for the property.
    ''' </returns>
    Public Function GetFirstBrokenRule(ByVal [property] As String) As BrokenRule

      Return GetFirstMessage([property], RuleSeverity.Error)

    End Function

    ''' <summary>
    ''' Returns the first <see cref="BrokenRule" /> object
    ''' corresponding to the specified property.
    ''' </summary>
    ''' <remarks>
    ''' Code in a business object or UI can also use this value to retrieve
    ''' the first broken rule in <see cref="BrokenRulesCollection" /> that corresponds
    ''' to a specfic property.
    ''' </remarks>
    ''' <param name="property">The name of the property affected by the rule.</param>
    ''' <returns>
    ''' The first BrokenRule object corresponding to the specified property, or Nothing
    ''' (null in C#) if there are no rules defined for the property.
    ''' </returns>
    Public Function GetFirstMessage(ByVal [property] As String) As BrokenRule

      For Each item As BrokenRule In Me
        If item.Property = [property] Then
          Return item
        End If
      Next
      Return Nothing

    End Function

    ''' <summary>
    ''' Returns the first <see cref="BrokenRule" /> object
    ''' corresponding to the specified property and severity.
    ''' </summary>
    ''' <remarks>
    ''' Code in a business object or UI can also use this value to retrieve
    ''' the first broken rule in <see cref="BrokenRulesCollection" /> that corresponds
    ''' to a specfic property and severity on the object.
    ''' </remarks>
    ''' <param name="property">The name of the property affected by the rule.</param>
    ''' <param name="severity">The severity of the rule.</param>
    ''' <returns>
    ''' The first BrokenRule object corresponding to the specified property, or Nothing
    ''' (null in C#) if there are no rules defined for the property.
    ''' </returns>
    Public Function GetFirstMessage(ByVal [property] As String, ByVal severity As RuleSeverity) As BrokenRule

      For Each item As BrokenRule In Me
        If item.Property = [property] AndAlso item.Severity = severity Then
          Return item
        End If
      Next
      Return Nothing

    End Function

    Friend Sub New()
      ' limit creation to this assembly
    End Sub

    Friend Overloads Sub Add(ByVal rule As IRuleMethod)

      Remove(rule)

      IsReadOnly = False
      Dim item As New BrokenRule(rule)
      IncrementCount(item)
      Add(item)
      IsReadOnly = True

    End Sub

    Friend Overloads Sub Remove(ByVal rule As IRuleMethod)

      ' we loop through using a numeric counter because
      ' removing items in a foreach isn't reliable
      IsReadOnly = False
      For index As Integer = 0 To Count - 1
        If Me(index).RuleName = rule.RuleName Then
          DecrementCount(Me(index))
          RemoveAt(index)
          Exit For
        End If
      Next
      IsReadOnly = True

    End Sub

    Private Sub IncrementCount(ByVal item As BrokenRule)

      Select Case item.Severity
        Case RuleSeverity.Error
          mErrorCount += 1
        Case RuleSeverity.Warning
          mWarningCount += 1
        Case Else
          mInfoCount += 1
      End Select

    End Sub

    Private Sub DecrementCount(ByVal item As BrokenRule)

      Select Case item.Severity
        Case RuleSeverity.Error
          mErrorCount -= 1
        Case RuleSeverity.Warning
          mWarningCount -= 1
        Case Else
          mInfoCount -= 1
      End Select

    End Sub

    ''' <summary>
    ''' Returns the text of all broken rule descriptions, each
    ''' separated by a <see cref="Environment.NewLine" />.
    ''' </summary>
    ''' <returns>The text of all broken rule descriptions.</returns>
    Public Overrides Function ToString() As String

      Return ToString(Environment.NewLine)

    End Function

    ''' <summary>
    ''' Returns the text of all broken rule descriptions
    ''' for a specific severity, each
    ''' separated by a <see cref="Environment.NewLine" />.
    ''' </summary>
    ''' <param name="severity">The severity of rules to
    ''' include in the result.</param>
    ''' <returns>The text of all broken rule descriptions
    ''' matching the specified severtiy.</returns>
    Public Overloads Function ToString(ByVal severity As RuleSeverity) As String

      Return ToString(Environment.NewLine, severity)

    End Function

    ''' <summary>
    ''' Returns the text of all broken rule descriptions.
    ''' </summary>
    ''' <param name="separator">
    ''' String to place between each broken rule description.
    ''' </param>
    ''' <returns>The text of all broken rule descriptions.</returns>
    Public Overloads Function ToString(ByVal separator As String) As String

      Dim result As New System.Text.StringBuilder()
      Dim item As BrokenRule
      Dim first As Boolean = True

      For Each item In Me
        If first Then
          first = False
        Else
          result.Append(separator)
        End If
        result.Append(item.Description)
      Next
      Return result.ToString

    End Function

    ''' <summary>
    ''' Returns the text of all broken rule descriptions
    ''' for a specific severity.
    ''' </summary>
    ''' <param name="separator">
    ''' String to place between each broken rule description.
    ''' </param>
    ''' <param name="severity">The severity of rules to
    ''' include in the result.</param>
    ''' <returns>The text of all broken rule descriptions
    ''' matching the specified severtiy.</returns>
    Public Overloads Function ToString(ByVal separator As String, ByVal severity As RuleSeverity) As String

      Dim result As New System.Text.StringBuilder()
      Dim item As BrokenRule
      Dim first As Boolean = True

      For Each item In Me
        If item.Severity = severity Then
          If first Then
            first = False
          Else
            result.Append(separator)
          End If
          result.Append(item.Description)
        End If
      Next
      Return result.ToString

    End Function


    ''' <summary>
    ''' Returns a string array containing all broken
    ''' rule descriptions.
    ''' </summary>
    ''' <returns>The text of all broken rule descriptions
    ''' matching the specified severtiy.</returns>
    Public Function ToArray() As String()

      Dim result As New List(Of String)
      For Each item As BrokenRule In Me
        result.Add(item.Description)
      Next
      Return result.ToArray

    End Function

    ''' <summary>
    ''' Returns a string array containing all broken
    ''' rule descriptions.
    ''' </summary>
    ''' <param name="severity">The severity of rules
    ''' to include in the result.</param>
    ''' <returns>The text of all broken rule descriptions
    ''' matching the specified severtiy.</returns>
    Public Function ToArray(ByVal severity As RuleSeverity) As String()

      Dim result As New List(Of String)
      For Each item As BrokenRule In Me
        If item.Severity = severity Then
          result.Add(item.Description)
        End If
      Next
      Return result.ToArray

    End Function

  End Class

End Namespace
