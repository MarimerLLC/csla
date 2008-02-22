<Serializable()> _
Public Class ResourceInfo
  Inherits ReadOnlyBase(Of ResourceInfo)

  Private _id As Integer
  Private _name As String

  Public ReadOnly Property Id() As Integer
    Get
      Return _id
    End Get
  End Property

  Public ReadOnly Property Name() As String
    Get
      Return _name
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return _id
  End Function

  Public Overrides Function ToString() As String
    Return _name
  End Function

  Friend Sub New(ByVal resource As DalLinq.Resource)
    _id = resource.Id
    _name = String.Format("{0}, {1}", resource.LastName, resource.FirstName)
  End Sub

End Class
