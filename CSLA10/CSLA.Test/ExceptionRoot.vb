<Serializable()> _
Public Class ExceptionRoot
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

  Public Shared Function NewExceptionRoot() As ExceptionRoot
    Dim crit As Criteria = New Criteria
    Dim result As Object = DataPortal.Create(crit)
    Return DirectCast(result, ExceptionRoot)
  End Function

  Public Shared Function GetExceptionRoot(ByVal Data As String) As ExceptionRoot
    Return DirectCast(DataPortal.Fetch(New Criteria(Data)), ExceptionRoot)
  End Function

  Public Shared Sub DeleteExceptionRoot(ByVal Data As String)
    DataPortal.Delete(New Criteria(Data))
  End Sub


  Private Sub New()
    ' prevent direct creation
  End Sub

  Protected Overrides Sub DataPortal_Create(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mData = crit.Data
    Session.Add("Root", "Created")
    ApplicationContext.GlobalContext.Item("create") = "create"
    Throw New ApplicationException("Fail create")

  End Sub

  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mData = crit.Data
    MarkOld()
    Session.Add("Root", "Fetched")
    ApplicationContext.GlobalContext.Item("create") = "create"
    Throw New ApplicationException("Fail fetch")
  End Sub

  Protected Overrides Sub DataPortal_Update()

    If IsDeleted Then
      ' we would delete here
      Session("Root") = "Deleted"
      MarkNew()
    Else
      If IsNew Then
        ' we would insert here
        Session("Root") = "Inserted"

      Else
        ' we would update here
        Session("Root") = "Updated"
      End If
      MarkOld()
    End If
    ApplicationContext.GlobalContext.Item("create") = "create"
    Throw New ApplicationException("Fail update")
  End Sub

  Protected Overrides Sub DataPortal_Delete(ByVal Criteria As Object)
    ' we would delete here
    Session.Add("Root", "Deleted")
    ApplicationContext.GlobalContext.Item("create") = "create"
    Throw New ApplicationException("Fail delete")
  End Sub

End Class
