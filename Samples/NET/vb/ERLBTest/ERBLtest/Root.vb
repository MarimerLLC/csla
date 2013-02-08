Imports Csla

<Serializable()> _
Public Class Root
    Inherits BusinessBase(Of Root)

    Private Shared DataProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Data)
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

    Private Shared Val1Property As PropertyInfo(Of Short) = RegisterProperty(Of Short)(Function(c) c.Val1)
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

    Private Shared Val2Property As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Val2)
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
        BusinessRules.AddRule(New Csla.Rules.CommonRules.MinValue(Of Short)(Val1Property, 100))
    End Sub

#End Region

#Region " Factory Methods "

    Public Shared Function NewRoot() As Root
        Return DataPortal.Create(Of Root)()
    End Function

    Public Shared Function GetRoot(ByVal id As String) As Root
        Return DataPortal.Fetch(Of Root)(New SingleCriteria(Of Root, String)(id))
    End Function

    Public Shared Sub DeleteRoot(ByVal id As String)
        DataPortal.Delete(Of Root)(New SingleCriteria(Of Root, String)(id))
    End Sub

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

    Protected Overrides Sub DataPortal_Create()
        '_uniqueId += 1
        '_data = _uniqueId.ToString
        BusinessRules.CheckRules()
        Debug.WriteLine("Create")
    End Sub

    Private Overloads Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of Root, String))
        Debug.WriteLine("Fetch")
    End Sub

    Protected Overrides Sub DataPortal_Insert()
        _uniqueId += 1
        LoadProperty(DataProperty, _uniqueId.ToString)
        Debug.WriteLine("Insert")
        'Me._val1 = 9876
    End Sub

    Protected Overrides Sub DataPortal_Update()
        Debug.WriteLine("Update")
    End Sub

    Protected Overrides Sub DataPortal_DeleteSelf()
        DataPortal_Delete(New SingleCriteria(Of Root, String)(Data))
    End Sub

    Private Overloads Sub DataPortal_Delete(ByVal criteria As SingleCriteria(Of Root, String))
        Debug.WriteLine("Delete")
    End Sub

#End Region

End Class
