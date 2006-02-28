<Serializable()> _
Public Class ResourceInfo
  Inherits ReadOnlyBase(Of ResourceInfo)

  Private mId As Integer
  Private mName As String

  Public Property Id() As Integer
    Get
      Return mId
    End Get
    Friend Set(ByVal Value As Integer)
      mId = Value
    End Set
  End Property

  Public Property Name() As String
    Get
      Return mName
    End Get
    Friend Set(ByVal Value As String)
      mName = Value
    End Set
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return mId
  End Function

  Public Overrides Function ToString() As String
    Return mName
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

  Friend Sub New(ByVal dr As SafeDataReader)
    mId = dr.GetInt32("Id")
    mName = String.Format("{0}, {1}", _
      dr.GetString("LastName"), dr.GetString("FirstName"))
  End Sub

End Class
