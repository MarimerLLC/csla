<Serializable()> _
Public Class Root
  Inherits BusinessBase

  Private mData As String = ""

  Private mChildren As Children = Children.NewChildren

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

  Public ReadOnly Property Children() As Children
    Get
      Return mChildren
    End Get
  End Property

  Public Overrides ReadOnly Property IsDirty() As Boolean
    Get
      Return MyBase.IsDirty OrElse mChildren.IsDirty
    End Get
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

  Public Shared Function NewRoot() As Root
    Return DirectCast(DataPortal.Create(New Criteria()), Root)
  End Function

  Public Shared Function GetRoot(ByVal Data As String) As Root
    Return DirectCast(DataPortal.Fetch(New Criteria(Data)), Root)
  End Function

  Public Shared Sub DeleteRoot(ByVal Data As String)
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

    Session.Add("clientcontext", ApplicationContext.ClientContext.Item("clientcontext"))

    Session.Add("globalcontext", ApplicationContext.GlobalContext.Item("globalcontext"))
    ApplicationContext.GlobalContext.Remove("globalcontext")
    ApplicationContext.GlobalContext.Item("globalcontext") = "new global value"

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

  Protected Overrides Sub Deserialized()
    MyBase.Deserialized()
    Session.Add("Deserialized", "root Deserialized")
  End Sub

  Protected Overrides Sub Serialized()
    MyBase.Serialized()
    Session.Add("Serialized", "root Serialized")
  End Sub

  Protected Overrides Sub Serializing()
    MyBase.Serializing()
    Session.Add("Serializing", "root Serializing")
  End Sub

End Class
