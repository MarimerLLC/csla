<Serializable()> _
Public Class ReadOnlyRoot
  Inherits ReadOnlyBase(Of ReadOnlyRoot)

#Region " Business Methods "

  Private mId As Integer

  Public ReadOnly Property Id() As Integer
    Get
      CanReadProperty(True)
      Return mId
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object

    Return mId

  End Function

#End Region

#Region " Authorization Rules "

  Protected Overrides Sub AddAuthorizationRules()

    ' TODO: add authorization rules
    'AuthorizationRules.AllowRead("", "")

  End Sub

  Public Shared Function CanGetObject() As Boolean
    ' TODO: customize to check user role
    'Return ApplicationContext.User.IsInRole("")
    Return True
  End Function

#End Region

#Region " Factory Methods "

  Public Shared Function GetReadOnlyRoot(ByVal id As Integer) As ReadOnlyRoot
    Return DataPortal.Create(Of ReadOnlyRoot)(New Criteria(id))
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class Criteria
    Private mId As Integer
    Public ReadOnly Property Id() As Integer
      Get
        Return mId
      End Get
    End Property
    Public Sub New(ByVal id As Integer)
      mId = id
    End Sub
  End Class

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
    ' load values
  End Sub

#End Region

End Class
