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
      mData = Value
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

  Private Sub New()
    ' prevent direct creation
  End Sub

  Protected Overrides Sub DataPortal_Create(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mData = crit.Data
  End Sub

  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    Dim crit As Criteria = DirectCast(Criteria, Criteria)
    mData = crit.Data
    MarkOld()
  End Sub

  Protected Overrides Sub DataPortal_Update()
    If IsDeleted Then
      ' we would delete here
      MarkNew()
    Else
      If IsNew Then
        ' we would insert here

      Else
        ' we would update here
      End If
      MarkOld()
    End If
  End Sub

  Protected Overrides Sub DataPortal_Delete(ByVal Criteria As Object)
    ' we would delete here
  End Sub

End Class
