Imports Csla

<Serializable()> _
Public Class ThreadCheck
  Inherits BusinessBase(Of ThreadCheck)

  Public Shared ThreadProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Thread)
  ''' <Summary>
  ''' Gets and sets the Thread value.
  ''' </Summary>
  Public Property Thread() As Integer
    Get
      Return GetProperty(ThreadProperty)
    End Get
    Set(ByVal value As Integer)
      SetProperty(ThreadProperty, value)
    End Set
  End Property

  Private Shared CreateThreadProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.CreateThread)
  ''' <Summary>
  ''' Gets and sets the CreateThread value.
  ''' </Summary>
  Public Property CreateThread() As Integer
    Get
      Return GetProperty(Of Integer)(CreateThreadProperty)
    End Get
    Set(ByVal value As Integer)
      SetProperty(Of Integer)(CreateThreadProperty, value)
    End Set
  End Property

  Private Shared DataProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Data)
  ''' <Summary>
  ''' Gets and sets the Data value.
  ''' </Summary>
  Public Property Data() As String
    Get
      Return GetProperty(Of String)(DataProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(Of String)(DataProperty, value)
    End Set
  End Property

  Protected Overrides Sub DataPortal_Create()
    CreateThread = System.Threading.Thread.CurrentThread.ManagedThreadId
  End Sub

  Private Overloads Sub DataPortal_Create(ByVal e As SingleCriteria(Of ThreadCheck, String))
    CreateThread = System.Threading.Thread.CurrentThread.ManagedThreadId
    Data = e.Value
  End Sub

End Class
