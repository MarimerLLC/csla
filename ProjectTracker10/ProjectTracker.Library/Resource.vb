Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class Resource
  Inherits BusinessBase

  Private mID As String = ""
  Private mLastName As String = ""
  Private mFirstName As String = ""

  Private mAssignments As ResourceAssignments = _
    ResourceAssignments.NewResourceAssignments()

#Region " Business Properties and Methods "

  Public ReadOnly Property ID() As String
    Get
      Return mID
    End Get
  End Property

  Public Property LastName() As String
    Get
      Return mLastName
    End Get
    Set(ByVal Value As String)
      If mLastName <> Value Then
        mLastName = Value
        MarkDirty()
        BrokenRules.Assert("LNameLen", "Value too long", Len(Value) > 50)
      End If
    End Set
  End Property

  Public Property FirstName() As String
    Get
      Return mFirstName
    End Get
    Set(ByVal Value As String)
      If mFirstName <> Value Then
        mFirstName = Value
        MarkDirty()
        BrokenRules.Assert("FNameLen", "Value too long", Len(Value) > 50)
      End If
    End Set
  End Property

  Public ReadOnly Property Assignments() As ResourceAssignments
    Get
      Return mAssignments
    End Get
  End Property

  Public Overrides ReadOnly Property IsValid() As Boolean
    Get
      Return MyBase.IsValid AndAlso mAssignments.IsValid
    End Get
  End Property

  Public Overrides ReadOnly Property IsDirty() As Boolean
    Get
      Return MyBase.IsDirty OrElse mAssignments.IsDirty
    End Get
  End Property

#End Region

#Region " System.Object Overrides "

  Public Overrides Function ToString() As String
    Return mID
  End Function

  Public Overloads Function Equals(ByVal Resource As Resource) As Boolean
    Return (mID = Resource.ID)
  End Function

  Public Overrides Function GetHashCode() As Integer
    Return mID.GetHashCode
  End Function

#End Region

#Region " Shared Methods "

  ' create new object
  Public Shared Function NewResource(ByVal ID As String) As Resource
    If Not Threading.Thread.CurrentPrincipal.IsInRole("Supervisor") _
      AndAlso _
      Not Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager") Then
      Throw New System.Security.SecurityException("User not authorized to add a resource")
    End If
    Return New Resource(ID)
  End Function

  ' delete object
  Public Shared Sub DeleteResource(ByVal ID As String)
    If Not Threading.Thread.CurrentPrincipal.IsInRole("Supervisor") _
      AndAlso _
      Not Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager") _
      AndAlso _
      Not Threading.Thread.CurrentPrincipal.IsInRole("Administrator") Then
      Throw New System.Security.SecurityException("User not authorized to remove a resource")
    End If
    DataPortal.Delete(New Criteria(ID))
  End Sub

  ' load existing object
  Public Shared Function GetResource(ByVal ID As String) As Resource
    Return CType(DataPortal.Fetch(New Criteria(ID)), Resource)
  End Function

  Public Overrides Function Save() As BusinessBase
    If IsDeleted Then
      If Not Threading.Thread.CurrentPrincipal.IsInRole("Supervisor") _
         AndAlso _
         Not Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager") _
         AndAlso _
         Not Threading.Thread.CurrentPrincipal.IsInRole("Administrator") Then
        Throw New System.Security.SecurityException("User not authorized to remove a resource")
      End If

    Else
      If Not Threading.Thread.CurrentPrincipal.IsInRole("Supervisor") _
         AndAlso _
         Not Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager") Then
        Throw New System.Security.SecurityException( _
          "User not authorized to update a resource")
      End If
    End If

    Return MyBase.Save
  End Function

#End Region

#Region " Constructors "

  Private Sub New(ByVal ID As String)
    mID = ID
  End Sub

  Private Sub New()
    ' prevent direct instantiation
  End Sub

#End Region

#Region " Criteria "

  <Serializable()> _
  Private Class Criteria
    Public ID As String

    Public Sub New(ByVal ID As String)
      Me.ID = ID
    End Sub
  End Class

#End Region

#Region " Data Access "

  ' called by DataPortal to load data from the database
  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    Dim crit As Criteria = CType(Criteria, Criteria)
    Dim cn As New SqlConnection(db("PTracker"))
    Dim cm As New SqlCommand
    Dim tr As SqlTransaction

    cn.Open()
    Try
      tr = cn.BeginTransaction(IsolationLevel.ReadCommitted)
      Try
        With cm
          .Connection = cn
          .Transaction = tr
          .CommandType = CommandType.StoredProcedure
          .CommandText = "getResource"
          .Parameters.Add("@ID", crit.ID)

          Dim dr As New SafeDataReader(.ExecuteReader)
          Try
            dr.Read()
            With dr
              mID = .GetString(0)
              mLastName = .GetString(1)
              mFirstName = .GetString(2)
            End With

            ' load child objects
            dr.NextResult()
            mAssignments = ResourceAssignments.GetResourceAssignments(dr)

          Finally
            dr.Close()
          End Try
        End With

        MarkOld()

        tr.Commit()

      Catch ex As Exception
        tr.Rollback()
        Throw ex
      End Try

    Finally
      cn.Close()
    End Try
  End Sub

  ' called by DataPortal to delete/add/update data into the database
  Protected Overrides Sub DataPortal_Update()
    ' save data into db
    Dim cn As New SqlConnection(db("PTracker"))
    Dim cm As New SqlCommand
    Dim tr As SqlTransaction

    cn.Open()
    Try
      tr = cn.BeginTransaction(IsolationLevel.Serializable)
      With cm
        .Connection = cn
        .Transaction = tr
        .CommandType = CommandType.StoredProcedure
        If Me.IsDeleted Then
          ' we're being deleted
          If Not Me.IsNew Then
            ' we're not new, so get rid of our data
            .CommandText = "deleteResource"
            .Parameters.Add("@ID", mID)
            .ExecuteNonQuery()
          End If
          ' reset our status to be a new object
          MarkNew()

        Else
          ' we're not being deleted, so insert or update
          If Me.IsNew Then
            ' we're new, so insert
            .CommandText = "addResource"

          Else
            ' we're not new, so update
            .CommandText = "updateResource"

          End If

          .Parameters.Add("@ID", mID)
          .Parameters.Add("@LastName", mLastName)
          .Parameters.Add("@FirstName", mFirstName)


          .ExecuteNonQuery()

          ' update child objects
          mAssignments.Update(tr, Me)

          ' make sure we're marked as an old object
          MarkOld()

        End If

      End With

      tr.Commit()

    Catch ex As Exception
      tr.Rollback()
      Throw ex

    Finally
      cn.Close()
    End Try

  End Sub

  Protected Overrides Sub DataPortal_Delete(ByVal Criteria As Object)
    Dim crit As Criteria = CType(Criteria, Criteria)
    Dim cn As New SqlConnection(db("PTracker"))
    Dim cm As New SqlCommand
    Dim tr As SqlTransaction

    cn.Open()
    Try
      tr = cn.BeginTransaction(IsolationLevel.Serializable)
      With cm
        .Connection = cn
        .Transaction = tr
        .CommandType = CommandType.StoredProcedure

        .CommandText = "deleteResource"
        .Parameters.Add("@ID", crit.ID)
        .ExecuteNonQuery()
      End With

      tr.Commit()

    Catch ex As Exception
      tr.Rollback()
      Throw ex

    Finally
      cn.Close()
    End Try
  End Sub

#End Region

End Class
