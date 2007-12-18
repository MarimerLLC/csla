''' <summary>
''' A single-value criteria used to retrieve business
''' objects that only require one criteria value.
''' </summary>
''' <typeparam name="B">
''' Type of business object to retrieve.
''' </typeparam>
''' <typeparam name="C">
''' Type of the criteria value.
''' </typeparam>
''' <remarks></remarks>
<Serializable()> _
Public Class SingleCriteria(Of B, C)
  Inherits CriteriaBase

  Private mValue As C

  Public ReadOnly Property Value() As C
    Get
      Return mValue
    End Get
  End Property

  Public Sub New(ByVal value As C)
    MyBase.New(GetType(B))
    mValue = value
  End Sub

End Class
