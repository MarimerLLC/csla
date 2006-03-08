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

    ''' <summary>
    ''' Returns the first <see cref="BrokenRule" /> object
    ''' corresponding to the specified property.
    ''' </summary>
    ''' <remarks>
    ''' Code in a business object or UI can also use this value to retrieve
    ''' the first broken rule in <see cref="BrokenRulesCollection" /> that corresponds
    ''' to a specfic Property on the object.
    ''' </remarks>
    ''' <param name="property">The name of the property affected by the rule.</param>
    ''' <returns>
    ''' The first BrokenRule object corresponding to the specified property, or Nothing if 
    ''' there are no rules defined for the property.
    ''' </returns>
    Public Function GetFirstBrokenRule(ByVal [property] As String) As BrokenRule

      For Each item As BrokenRule In Me
        If item.Property = [property] Then
          Return item
        End If
      Next
      Return Nothing

    End Function

    Friend Sub New()
      ' limit creation to this assembly
    End Sub

    Friend Overloads Sub Add(ByVal rule As RuleMethod)

      Remove(rule)
      IsReadOnly = False
      Add(New BrokenRule(rule))
      IsReadOnly = True

    End Sub

    Friend Overloads Sub Remove(ByVal rule As RuleMethod)

      ' we loop through using a numeric counter because
      ' removing items in a foreach isn't reliable
      IsReadOnly = False
      For index As Integer = 0 To Count - 1
        If Me(index).RuleName = rule.RuleName Then
          RemoveAt(index)
          Exit For
        End If
      Next
      IsReadOnly = True

    End Sub

    ''' <summary>
    ''' Returns the text of all broken rule descriptions, each
    ''' separated by a <see cref="Environment.NewLine" />.
    ''' </summary>
    ''' <returns>The text of all broken rule descriptions.</returns>
    Public Overrides Function ToString() As String

      Dim result As New System.Text.StringBuilder()
      Dim item As BrokenRule
      Dim first As Boolean = True

      For Each item In Me
        If first Then
          first = False
        Else
          result.Append(Environment.NewLine)
        End If
        result.Append(item.Description)
      Next
      Return result.ToString

    End Function

  End Class

End Namespace
