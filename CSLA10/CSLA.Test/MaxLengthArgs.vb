Imports CSLA.BrokenRules

Public Class MaxLengthArgs
  Inherits RuleArgs

  Private mMax As Integer

  Public ReadOnly Property MaxLength() As Integer
    Get
      Return mMax
    End Get
  End Property

  Public Sub New(ByVal maxLength As Integer)
    mMax = maxLength
  End Sub

  Public Sub New(ByVal propertyName As String, ByVal maxLength As Integer)
    MyBase.New(propertyName)
    mMax = maxLength
  End Sub
End Class
