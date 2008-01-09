<Serializable()> _
Public Class ResourceInfo
  Inherits ReadOnlyBase(Of ResourceInfo)

  Private mId As Integer
  Private mName As String

  Public ReadOnly Property Id() As Integer
    Get
      Return mId
    End Get
  End Property

  Public ReadOnly Property Name() As String
    Get
      Return mName
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return mId
  End Function

  Public Overrides Function ToString() As String
    Return mName
  End Function

  Friend Sub New(ByVal resource As DalLinq.Resource)
    mId = resource.Id
    mName = String.Format("{0}, {1}", resource.LastName, resource.FirstName)
  End Sub

End Class
