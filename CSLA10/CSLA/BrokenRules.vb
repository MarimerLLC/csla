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

    Friend Sub New(ByVal Rule As String, ByVal Description As String)
      mRule = Rule
      mDescription = Description
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

    Protected Overrides Sub OnClear()
      If Not mLegal Then
        Throw New NotSupportedException("Clear is an invalid operation")
      End If
    End Sub

    Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)
      If Not mLegal Then
        Throw New NotSupportedException("Insert is an invalid operation")
      End If
    End Sub

    Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)
      If Not mLegal Then
        Throw New NotSupportedException("Remove is an invalid operation")
      End If
    End Sub

    Protected Overrides Sub OnSet(ByVal index As Integer, _
        ByVal oldValue As Object, ByVal newValue As Object)
      If Not mLegal Then
        Throw New NotSupportedException("Changing an element is an invalid operation")
      End If
    End Sub
  End Class

#End Region

  Private mRules As New RulesCollection()

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
      mRules.Add(Rule, Description)
    Else
      mRules.Remove(Rule)
    End If
  End Sub

  ''' <summary>
  ''' Returns a value indicating whether there are any broken rules
  ''' at this time. If there are broken rules, the business object
  ''' is assumed to be invalid and False is returned. If there are no
  ''' broken business rules True is returned.
  ''' </summary>
  ''' <returns>A value indicating whether any rules are broken.</returns>
  Public ReadOnly Property IsValid() As Boolean
    Get
      Return mRules.Count = 0
    End Get
  End Property

  ''' <summary>
  ''' Returns a value indicating whether a particular business rule
  ''' is currently broken.
  ''' </summary>
  ''' <param name="Rule">The name of the rule to check.</param>
  ''' <returns>A value indicating whether the rule is currently broken.</returns>
  Public Function IsBroken(ByVal Rule As String) As Boolean
    Return mRules.Contains(Rule)
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
    Return mRules
  End Function

  ''' <summary>
  ''' Returns the text of all broken rule descriptions, each
  ''' separated by cr/lf.
  ''' </summary>
  ''' <returns>The text of all broken rule descriptions.</returns>
  Public Overrides Function ToString() As String
    Dim obj As New System.Text.StringBuilder()
    Dim item As Rule

    For Each item In mRules
      obj.Append(item.Description)
      obj.Append(vbCrLf)
    Next
    Return obj.ToString
  End Function

End Class
