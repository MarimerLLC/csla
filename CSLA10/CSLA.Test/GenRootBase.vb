<Serializable()> _
Public Class GenRootBase
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
    Inherits CriteriaBase

    Public Data As String

    Public Sub New()
      MyBase.New(GetType(GenRoot))
      Data = "<new>"
    End Sub

    Public Sub New(ByVal Data As String)
      MyBase.New(GetType(GenRoot))
      Me.Data = Data
    End Sub
  End Class

  Public Shared Function NewRoot() As GenRoot
    Return DirectCast(DataPortal.Create(New Criteria()), GenRoot)
  End Function

  Public Shared Function GetRoot(ByVal Data As String) As GenRoot
    Return DirectCast(DataPortal.Fetch(New Criteria(Data)), GenRoot)
  End Function

  Public Shared Sub DeleteRoot(ByVal Data As String)
    DataPortal.Delete(New Criteria(Data))
  End Sub


  Protected Sub New()
    ' prevent direct creation
  End Sub

  Protected Overrides Sub DataPortal_Create(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mData = crit.Data
    Session.Add("GenRoot", "Created")
  End Sub

  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mData = crit.Data
    MarkOld()
    Session.Add("GenRoot", "Fetched")
  End Sub

  Protected Overrides Sub DataPortal_Update()
    If IsDeleted Then
      ' we would delete here
      Session.Add("GenRoot", "Deleted")
      MarkNew()
    Else
      If IsNew Then
        ' we would insert here
        Session.Add("GenRoot", "Inserted")

      Else
        ' we would update here
        Session.Add("GenRoot", "Updated")
      End If
      MarkOld()
    End If
  End Sub

  Protected Overrides Sub DataPortal_Delete(ByVal Criteria As Object)
    ' we would delete here
    Session.Add("GenRoot", "Deleted")
  End Sub

End Class
