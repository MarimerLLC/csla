Namespace Admin

  <Serializable()> _
  Public Class Role
    Inherits BusinessBase(Of Role)

#Region " Business Methods "

    Private Shared IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(GetType(Role), New PropertyInfo(Of Integer)("Id"))
    Public Property Id() As Integer
      Get
        Dim result = GetProperty(Of Integer)(IdProperty)
        If result = 0 Then
          result = GetMax() + 1
          LoadProperty(Of Integer)(IdProperty, result)
        End If
        Return result
      End Get
      Set(ByVal value As Integer)
        SetProperty(Of Integer)(IdProperty, value)
      End Set
    End Property

    Private Function GetMax() As Integer

      ' generate a default id value
      Dim parent As Roles = CType(Me.Parent, Roles)
      Dim max As Integer = 0
      For Each item As Role In parent
        If Not ReferenceEquals(item, Me) AndAlso item.Id > max Then
          max = item.Id
        End If
      Next
      Return max

    End Function

    Private Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(GetType(Role), New PropertyInfo(Of String)("Name"))
    Public Property Name() As String
      Get
        Return GetProperty(NameProperty)
      End Get
      Set(ByVal value As String)
        SetProperty(Of String)(NameProperty, value)
      End Set
    End Property

    Private _timestamp(7) As Byte

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      ValidationRules.AddRule(Of Role)( _
        AddressOf NoDuplicates, IdProperty)
      ValidationRules.AddRule( _
        AddressOf Csla.Validation.CommonRules.StringRequired, NameProperty)

    End Sub

    Private Shared Function NoDuplicates(Of T As Role)( _
      ByVal target As T, _
      ByVal e As Csla.Validation.RuleArgs) As Boolean

      Dim parent As Roles = CType(target.Parent, Roles)
      If parent IsNot Nothing Then
        For Each item As Role In parent
          If item.Id = target.GetProperty(Of Integer)(IdProperty) AndAlso Not ReferenceEquals(item, target) Then
            e.Description = "Role Id must be unique"
            Return False
          End If
        Next
      End If
      Return True

    End Function

#End Region

#Region " Authorization Rules "

    Protected Overrides Sub AddAuthorizationRules()

      AuthorizationRules.AllowWrite(IdProperty, "Administrator")
      AuthorizationRules.AllowWrite(NameProperty, "Administrator")

    End Sub

#End Region

#Region " Factory Methods "

    Friend Shared Function NewRole() As Role

      Return DataPortal.CreateChild(Of Role)()

    End Function

    Friend Shared Function GetRole(ByVal data As ProjectTracker.DalLinq.getRolesResult) As Role

      Return DataPortal.FetchChild(Of Role)(data)

    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

#End Region

#Region " Data Access "

    Private Sub Child_Fetch(ByVal data As ProjectTracker.DalLinq.getRolesResult)

      LoadProperty(Of Integer)(IdProperty, data.Id)
      LoadProperty(Of String)(NameProperty, data.Name)
      _timestamp = data.LastChanged.ToArray

    End Sub

    Private Sub Child_Insert()

      Using mgr = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager("PTracker", True)
        Dim lastChanged As System.Data.Linq.Binary = _timestamp
        mgr.DataContext.addRole(GetProperty(Of Integer)(IdProperty), _
                                GetProperty(Of String)(NameProperty), _
                                lastChanged)
        _timestamp = lastChanged.ToArray
      End Using

    End Sub

    Private Sub Child_Update()

      Using mgr = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager("PTracker", True)
        Dim lastChanged As System.Data.Linq.Binary = Nothing
        mgr.DataContext.UpdateRole(GetProperty(Of Integer)(IdProperty), _
                                   GetProperty(Of String)(NameProperty), _
                                   _timestamp, _
                                   lastChanged)
        _timestamp = lastChanged.ToArray
      End Using

    End Sub

    Private Sub Child_DeleteSelf()

      Using mgr = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager("PTracker", True)
        mgr.DataContext.DeleteRole(Id)
      End Using

    End Sub

#End Region

  End Class

End Namespace
