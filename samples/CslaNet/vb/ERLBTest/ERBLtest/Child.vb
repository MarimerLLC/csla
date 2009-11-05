Imports Csla

<Serializable()> _
Public Class Child
  Inherits BusinessBase(Of Child)

  Private Shared DataProperty As PropertyInfo(Of String) = RegisterProperty(New PropertyInfo(Of String)("Data", "Data"))
  ''' <Summary>
  ''' Gets and sets the Data value.
  ''' </Summary>
  Public Property Data() As String
    Get
      Return GetProperty(DataProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(DataProperty, value)
    End Set
  End Property

  Private Shared Val1Property As PropertyInfo(Of Short) = RegisterProperty(New PropertyInfo(Of Short)("Val1", "Val1"))
  ''' <Summary>
  ''' Gets and sets the Val1 value.
  ''' </Summary>
  Public Property Val1() As Short
    Get
      Return GetProperty(Val1Property)
    End Get
    Set(ByVal value As Short)
      SetProperty(Val1Property, value)
    End Set
  End Property

  Private Shared Val2Property As PropertyInfo(Of Integer) = RegisterProperty(New PropertyInfo(Of Integer)("Val2", "Val2"))
  ''' <Summary>
  ''' Gets and sets the Val2 value.
  ''' </Summary>
  Public Property Val2() As Integer
    Get
      Return GetProperty(Val2Property)
    End Get
    Set(ByVal value As Integer)
      SetProperty(Val2Property, value)
    End Set
  End Property

#Region " Validation Rules "

  Protected Overrides Sub AddBusinessRules()
    ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.MinValue(Of Short), _
      New Csla.Validation.CommonRules.MinValueRuleArgs(Of Short)("Val1", 100))
  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function NewChild() As Child
    Return DataPortal.CreateChild(Of Child)()
  End Function

  Public Shared Function GetChild(ByVal id As String) As Child
    Return DataPortal.FetchChild(Of Child)(New SingleCriteria(Of Child, String)(id))
  End Function

  Private Sub New()
    ' Require use of factory methods
  End Sub

#End Region

  Protected Overrides Sub AcceptChangesComplete()
    Debug.WriteLine("AcceptChangesComplete() data: " & Me.Data)
    MyBase.AcceptChangesComplete()
  End Sub


#Region " Data Access "

  Private Shared _uniqueId As Integer

  Protected Overrides Sub Child_Create()
    _uniqueId += 1
    LoadProperty(DataProperty, _uniqueId.ToString)
    ValidationRules.CheckRules()
    Debug.WriteLine("Create")
  End Sub

  Private Sub Child_Fetch(ByVal criteria As SingleCriteria(Of Child, String))
    Debug.WriteLine("Fetch")
  End Sub

  Private Sub Child_Insert()
    Debug.WriteLine("Insert")
  End Sub

  Private Sub Child_Update()
    Debug.WriteLine("Update")
  End Sub

  Private Sub Child_DeleteSelf()
    Debug.WriteLine("Delete")
  End Sub

#End Region

End Class
