Imports CSLA.BrokenRules

<Serializable()> _
Public Class HasRulesManager
  Inherits BusinessBase

  Private mName As String = ""

  Public Property Name() As String
    Get
      Return mName
    End Get
    Set(ByVal Value As String)
      If mName <> Value Then
        mName = Value
        BrokenRules.CheckRules("Name")
        MarkDirty()
      End If
    End Set
  End Property

  Protected Overrides Sub AddBusinessRules()
    With BrokenRules
      .SetTargetObject(Me)
      .AddRule(AddressOf NameRequired, "Name")
      .AddRule(AddressOf NameLength, "Name", New MaxLengthArgs(10))
      .CheckRules()
    End With
  End Sub

  <Description("{0} required")> _
  Private Function NameRequired(ByVal target As Object, ByVal e As RuleArgs) As Boolean
    Return Len(mName) > 0
  End Function

  <Description("{0} too long")> _
  Private Function NameLength(ByVal target As Object, ByVal e As RuleArgs) As Boolean
    Return Len(mName) <= DirectCast(e, MaxLengthArgs).MaxLength
  End Function

  <Serializable()> _
  Private Class Criteria
    Public Name As String

    Public Sub New()
      Name = "<new>"
    End Sub

    Public Sub New(ByVal Name As String)
      Me.Name = Name
    End Sub
  End Class

  Public Shared Function NewHasRulesManager() As HasRulesManager
    Return DirectCast(DataPortal.Create(New Criteria()), HasRulesManager)
  End Function

  Public Shared Function GetHasRulesManager(ByVal Name As String) As HasRulesManager
    Return DirectCast(DataPortal.Fetch(New Criteria(Name)), HasRulesManager)
  End Function

  Public Shared Sub DeleteHasRulesManager(ByVal Name As String)
    DataPortal.Delete(New Criteria(Name))
  End Sub


  Private Sub New()
    ' prevent direct creation
    AddBusinessRules()
  End Sub

  Protected Overrides Sub DataPortal_Create(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mName = crit.Name
    Session.Add("HasRulesManager", "Created")
  End Sub

  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mName = crit.Name
    MarkOld()
    Session.Add("HasRulesManager", "Fetched")
  End Sub

  Protected Overrides Sub DataPortal_Update()
    If IsDeleted Then
      ' we would delete here
      Session.Add("HasRulesManager", "Deleted")
      MarkNew()
    Else
      If IsNew Then
        ' we would insert here
        Session.Add("HasRulesManager", "Inserted")

      Else
        ' we would update here
        Session.Add("HasRulesManager", "Updated")
      End If
      MarkOld()
    End If
  End Sub

  Protected Overrides Sub DataPortal_Delete(ByVal Criteria As Object)
    ' we would delete here
    Session.Add("HasRulesManager", "Deleted")
  End Sub


End Class
