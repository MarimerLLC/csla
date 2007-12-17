Option Infer On

Imports System.Data.SqlClient

Namespace Admin

  <Serializable()> _
  Public Class Role
    Inherits BusinessBase(Of Role)

#Region " Business Methods "

    Private Shared IdProperty As New PropertyInfo(Of Integer)("Id")
    Private mId As Integer = IdProperty.DefaultValue
    Private mIdSet As Boolean
    Public Property Id() As Integer
      Get
        If Not mIdSet Then
          ' generate a default id value
          mIdSet = True
          Dim parent As Roles = CType(Me.Parent, Roles)
          Dim max As Integer = 0
          For Each item As Role In parent
            If item.Id > max Then
              max = item.Id
            End If
          Next
          mId = max + 1
        End If
        Return GetProperty(Of Integer)(IdProperty, mId)
      End Get
      Set(ByVal value As Integer)
        mIdSet = True
        SetProperty(Of Integer)(IdProperty, mId, value)
      End Set
    End Property

    Private Shared NameProperty As New PropertyInfo(Of String)("Name")
    Private mName As String = NameProperty.DefaultValue
    Public Property Name() As String
      Get
        Return GetProperty(NameProperty, mName)
      End Get
      Set(ByVal value As String)
        SetProperty(Of String)(NameProperty, mName, value)
      End Set
    End Property

    Private mTimestamp(7) As Byte

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      ValidationRules.AddRule(Of Role)(AddressOf NoDuplicates, IdProperty)
      ValidationRules.AddRule( _
        AddressOf Csla.Validation.CommonRules.StringRequired, NameProperty)

    End Sub

    Private Shared Function NoDuplicates(Of T As Role)(ByVal target As T, _
      ByVal e As Csla.Validation.RuleArgs) As Boolean

      Dim parent As Roles = CType(target.Parent, Roles)
      If parent IsNot Nothing Then
        For Each item As Role In parent
          If item.Id = target.mId AndAlso Not ReferenceEquals(item, target) Then
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

      Return New Role

    End Function

    Friend Shared Function GetRole(ByVal data As ProjectTracker.DalLinq.getRolesResult) As Role

      Return New Role(data)

    End Function

    Private Sub New()

      MarkAsChild()
      ValidationRules.CheckRules()

    End Sub

    Private Sub New(ByVal data As ProjectTracker.DalLinq.getRolesResult)

      MarkAsChild()
      Fetch(data)

    End Sub

#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal data As ProjectTracker.DalLinq.getRolesResult)

      mId = data.Id
      mIdSet = True
      mName = data.Name
      mTimestamp = data.LastChanged.ToArray
      MarkOld()

    End Sub

    Friend Sub Insert()

      ' if we're not dirty then don't update the database
      If Not Me.IsDirty Then Exit Sub

      Using mgr = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager("PTracker")
        Dim lastChanged As System.Data.Linq.Binary = mTimestamp
        mgr.DataContext.addRole(mId, mName, lastChanged)
        mTimestamp = lastChanged.ToArray
      End Using
      MarkOld()

    End Sub

    Friend Sub Update()

      ' if we're not dirty then don't update the database
      If Not Me.IsDirty Then Exit Sub

      Using mgr = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager("PTracker")
        Dim lastChanged As System.Data.Linq.Binary = Nothing
        mgr.DataContext.UpdateRole(mId, mName, mTimestamp, lastChanged)
        mTimestamp = lastChanged.ToArray
      End Using
      MarkOld()

    End Sub

    Friend Sub DeleteSelf()

      ' if we're not dirty then don't update the database
      If Not Me.IsDirty Then Exit Sub

      ' if we're new then don't update the database
      If Me.IsNew Then Exit Sub

      Using mgr = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager("PTracker")
        mgr.DataContext.DeleteRole(mId)
      End Using

      MarkNew()

    End Sub

#End Region

  End Class

End Namespace
