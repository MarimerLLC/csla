Public Class HasRules
  Inherits BusinessBase

  Private mName As String = ""

  Public Property Name() As String
    Get
      Return mName
    End Get
    Set(ByVal Value As String)
      If mName <> Value Then
        mName = Value
        BrokenRules.Assert("NameReq", "Name required", Len(mName) = 0)
        BrokenRules.Assert("NameMax", "Name too long", Len(mName) > 10)
        MarkDirty()
      End If
    End Set
  End Property

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

  Public Shared Function NewHasRules() As HasRules
    Return DirectCast(DataPortal.Create(New Criteria), HasRules)
  End Function

  Public Shared Function GetHasRules(ByVal Name As String) As HasRules
    Return DirectCast(DataPortal.Fetch(New Criteria(Name)), HasRules)
  End Function

  Public Shared Sub DeleteHasRules(ByVal Name As String)
    DataPortal.Delete(New Criteria(Name))
  End Sub


  Private Sub New()
    ' prevent direct creation
    BrokenRules.Assert("NameReq", "Name required", Len(mName) = 0)
    BrokenRules.Assert("NameMax", "Name too long", Len(mName) > 10)
  End Sub

  Protected Overrides Sub DataPortal_Create(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mName = crit.Name
    Session.Add("HasRules", "Created")
  End Sub

  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mName = crit.Name
    MarkOld()
    Session.Add("HasRules", "Fetched")
  End Sub

  Protected Overrides Sub DataPortal_Update()
    If IsDeleted Then
      ' we would delete here
      Session.Add("HasRules", "Deleted")
      MarkNew()
    Else
      If IsNew Then
        ' we would insert here
        Session.Add("HasRules", "Inserted")

      Else
        ' we would update here
        Session.Add("HasRules", "Updated")
      End If
      MarkOld()
    End If
  End Sub

  Protected Overrides Sub DataPortal_Delete(ByVal Criteria As Object)
    ' we would delete here
    Session.Add("HasRules", "Deleted")
  End Sub

End Class
