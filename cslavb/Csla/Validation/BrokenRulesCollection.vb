Imports System
Imports System.Collections.Generic
Imports Csla.Serialization
Imports Csla.Serialization.Mobile

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
  Partial Public Class BrokenRulesCollection
    Inherits Core.ReadOnlyBindingList(Of BrokenRule)

    Private _errorCount As Integer
    Private _warningCount As Integer
    Private _infoCount As Integer
    Private _customList As Boolean

    ''' <summary>
    ''' Gets the number of broken rules in
    ''' the collection that have a severity
    ''' of Error.
    ''' </summary>
    ''' <value>An integer value.</value>
    Public ReadOnly Property ErrorCount() As Integer
      Get
        Return _errorCount
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
        Return _warningCount
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
        Return _infoCount
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

    Friend Overloads Sub Add(ByVal rule As IAsyncRuleMethod, ByVal result As AsyncRuleResult)
      Remove(rule)
      IsReadOnly = False
      Dim Item As BrokenRule = New BrokenRule(rule, result)
      IncrementCount(Item)
      Add(Item)
      IsReadOnly = True
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

      IncrementRevision()
      Select Case item.Severity
        Case RuleSeverity.Error
          _errorCount += 1
        Case RuleSeverity.Warning
          _warningCount += 1
        Case Else
          _infoCount += 1
      End Select

    End Sub

    Private Sub DecrementCount(ByVal item As BrokenRule)

      IncrementRevision()
      Select Case item.Severity
        Case RuleSeverity.Error
          _errorCount -= 1
        Case RuleSeverity.Warning
          _warningCount -= 1
        Case Else
          _infoCount -= 1
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

#Region " Custom List "

    ''' <summary>
    ''' Gets an empty collection on which the Merge()
    ''' method may be called to merge in data from
    ''' other BrokenRuleCollection objects.
    ''' </summary>
    Public Shared Function CreateCollection() As BrokenRulesCollection

      Dim result As New BrokenRulesCollection
      result._customList = True
      Return result

    End Function

    ''' <summary>
    ''' Merges data from the supplied list into this
    ''' list, changing the rule names to be unique
    ''' based on the source value.
    ''' </summary>
    ''' <param name="source">
    ''' A unique source name for each list being
    ''' merged into this master list.
    ''' </param>
    ''' <param name="list">
    ''' A list to merge into this master list.
    ''' </param>
    ''' <remarks>
    ''' This method is intended to allow merging of
    ''' all BrokenRulesCollection objects in a business
    ''' object graph into a single list. To this end,
    ''' no attempt is made to remove duplicates during
    ''' the merge process. Also, the source parameter
    ''' value must be unqiue for each object instance
    ''' in the graph, or rule name collisions may
    ''' occur.
    ''' </remarks>
    Public Sub Merge(ByVal source As String, ByVal list As BrokenRulesCollection)

      If Not _customList Then
        Throw New NotSupportedException(My.Resources.BrokenRulesMergeException)
      End If
      IsReadOnly = False
      For Each item As BrokenRule In list
        Dim newItem As New BrokenRule(source, item)
        IncrementCount(newItem)
        Add(newItem)
      Next
      IsReadOnly = True

    End Sub

#End Region

#Region " Revision "

    Private _revision As Integer

    Private Sub IncrementRevision()

      _revision = (_revision + 1) Mod Integer.MaxValue

    End Sub

    ''' <summary>
    ''' Gets the current revision number of
    ''' the collection.
    ''' </summary>
    ''' <remarks>
    ''' The revision value changes each time an
    ''' item is added or removed from the collection.
    ''' </remarks>
    Public ReadOnly Property Revision() As Integer
      Get
        Return _revision
      End Get
    End Property

#End Region


#Region "MobileObject overrides"

    ''' <summary>
    ''' Override this method to insert your field values
    ''' into the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    Protected Overrides Sub OnGetState(ByVal info As SerializationInfo)
      info.AddValue("_errorCount", _errorCount)
      info.AddValue("_warningCount", _warningCount)
      info.AddValue("_infoCount", _infoCount)
      info.AddValue("_customList", _customList)

      MyBase.OnGetState(info)
    End Sub

    ''' <summary>
    ''' Override this method to retrieve your field values
    ''' from the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    Protected Overrides Sub OnSetState(ByVal info As SerializationInfo)
      _errorCount = info.GetValue(Of Integer)("_errorCount")
      _warningCount = info.GetValue(Of Integer)("_warningCount")
      _infoCount = info.GetValue(Of Integer)("_infoCount")
      _customList = info.GetValue(Of Boolean)("_customList")

      MyBase.OnSetState(info)
    End Sub


#End Region


  End Class

End Namespace
