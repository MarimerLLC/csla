<Serializable()> _
Public Class SimpleRoot
  Inherits BusinessBase

  Private mData As String = ""

  Public Property Data() As String
    Get
      Return mData
    End Get
    Set(ByVal Value As String)
      If mData <> Value Then
        mData = Value
        MarkDirty()
      End If
    End Set
  End Property

  <Serializable()> _
Private Class Criteria
    Public Data As String

    Public Sub New()
      Data = "<new>"
    End Sub

    Public Sub New(ByVal Data As String)
      Me.Data = Data
    End Sub
  End Class

  Public Shared Function NewSimpleRoot() As SimpleRoot
    Dim crit As Criteria = New Criteria
    Dim result As Object = DataPortal.Create(crit)
    Return DirectCast(result, SimpleRoot)
  End Function

  Public Shared Function GetSimpleRoot(ByVal Data As String) As SimpleRoot
    Return DirectCast(DataPortal.Fetch(New Criteria(Data)), SimpleRoot)
  End Function

  Public Shared Sub DeleteSimpleRoot(ByVal Data As String)
    DataPortal.Delete(New Criteria(Data))
  End Sub


  Private Sub New()
    ' prevent direct creation
  End Sub

  Protected Overrides Sub DataPortal_Create(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mData = crit.Data
    Session.Add("Root", "Created")
  End Sub

  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mData = crit.Data
    MarkOld()
    Session.Add("Root", "Fetched")
  End Sub

  Protected Overrides Sub DataPortal_Update()

    If IsDeleted Then
      ' we would delete here
      Session.Add("Root", "Deleted")
      MarkNew()
    Else
      If IsNew Then
        ' we would insert here
        Session.Add("Root", "Inserted")

      Else
        ' we would update here
        Session.Add("Root", "Updated")
      End If
      MarkOld()
    End If
  End Sub

  Protected Overrides Sub DataPortal_Delete(ByVal Criteria As Object)
    ' we would delete here
    Session.Add("Root", "Deleted")
  End Sub

End Class
