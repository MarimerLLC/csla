Imports System.Data.SqlClient

<Serializable()> _
Public Class Project
  Inherits BusinessBase

  Private mID As Guid = Guid.NewGuid
  Private mName As String = ""
  Private mStarted As New SmartDate(False)
  Private mEnded As New SmartDate
  Private mDescription As String = ""
  Private mHours As Integer = 0

  Private mResources As ProjectResources = _
    ProjectResources.NewProjectResources()

#Region " Business Properties and Methods "

  Public ReadOnly Property ID() As Guid
    Get
      Return mID
    End Get
  End Property

  Public Property Name() As String
    Get
      Return mName
    End Get
    Set(ByVal Value As String)
      If mName <> Value Then
        mName = Value
        MarkDirty()
        BrokenRules.Assert("NameLen", "Name too long", Len(Value) > 50)
        BrokenRules.Assert("NameRequired", "Project name required", _
                           Len(Value) = 0)
      End If
    End Set
  End Property

  Public Property Started() As String
    Get
      Return mStarted.Text
    End Get
    Set(ByVal Value As String)
      If mStarted.Text <> Value Then
        mStarted.Text = Value
        MarkDirty()
        If mEnded.IsEmpty Then
          BrokenRules.Assert("DateCol", "", False)
        Else
          BrokenRules.Assert("DateCol", _
                             "Start date must be prior to end date", _
                             mStarted.CompareTo(mEnded) > 0)
        End If
      End If
    End Set
  End Property

  Public Property Ended() As String
    Get
      Return mEnded.Text
    End Get
    Set(ByVal Value As String)
      If mEnded.Text <> Value Then
        mEnded.Text = Value
        MarkDirty()
        If mEnded.IsEmpty Then
          BrokenRules.Assert("DateCol", "", False)
        Else
          If mStarted.IsEmpty Then
            BrokenRules.Assert("DateCol", _
                              "Ended date must be later than started date", _
                              True)
          Else
            BrokenRules.Assert("DateCol", _
                              "Ended date must be later than started date", _
                              mEnded.CompareTo(mStarted) < 0)
          End If
        End If
      End If
    End Set
  End Property

  Public Property Description() As String
    Get
      Return mDescription
    End Get
    Set(ByVal Value As String)
      If mDescription <> Value Then
        mDescription = Value
        MarkDirty()
      End If
    End Set
  End Property

  Public ReadOnly Property Resources() As ProjectResources
    Get
      Return mResources
    End Get
  End Property

  Public Overrides ReadOnly Property IsValid() As Boolean
    Get
      Return MyBase.IsValid AndAlso mResources.IsValid
    End Get
  End Property

  Public Overrides ReadOnly Property IsDirty() As Boolean
    Get
      Return MyBase.IsDirty OrElse mResources.IsDirty
    End Get
  End Property

#End Region

#Region " System.Object Overrides "

  Public Overrides Function ToString() As String
    Return mID.ToString
  End Function

  Public Overloads Function Equals(ByVal Project As Project) As Boolean
    Return mID.Equals(Project.ID)
  End Function

  Public Overrides Function GetHashCode() As Integer
    Return mID.GetHashCode
  End Function

#End Region

#Region " Shared Methods "

  ' create new object
  Public Shared Function NewProject() As Project
    If Not Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager") Then
      Throw New System.Security.SecurityException("User not authorized to add a project")
    End If
    Return CType(DataPortal.Create(New Criteria(Guid.Empty)), Project)
  End Function

  ' load existing object by id
  Public Shared Function GetProject(ByVal ID As Guid) As Project
    Return CType(DataPortal.Fetch(New Criteria(ID)), Project)
  End Function

  ' delete object
  Public Shared Sub DeleteProject(ByVal ID As Guid)
    If Not Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager") _
      AndAlso _
       Not Threading.Thread.CurrentPrincipal.IsInRole("Administrator") Then
      Throw New System.Security.SecurityException("User not authorized to remove a project")
    End If
    DataPortal.Delete(New Criteria(ID))
  End Sub

  Public Overrides Function Save() As BusinessBase
    If IsDeleted Then
      If Not Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager") _
        AndAlso _
         Not Threading.Thread.CurrentPrincipal.IsInRole("Administrator") Then
        Throw New System.Security.SecurityException("User not authorized to remove a project")
      End If

    Else
      ' no deletion - we're adding or updating
      If Not Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager") Then
        Throw New System.Security.SecurityException("User not authorized to update a project")
      End If
    End If

    Return MyBase.Save
  End Function

#End Region

#Region " Constructors "

  Private Sub New()
    ' prevent direct instantiation
  End Sub

#End Region

#Region " Criteria "

  ' criteria for identifying existing object
  <Serializable()> _
  Private Class Criteria
    Public ID As Guid

    Public Sub New(ByVal ID As Guid)
      Me.ID = ID
    End Sub
  End Class

#End Region

#Region " Data Access "

  ' called by DataPortal so we can set defaults as needed
  Protected Overrides Sub DataPortal_Create(ByVal Criteria As Object)
    Dim crit As Criteria = CType(Criteria, Criteria)
    mID = Guid.NewGuid
    Started = CStr(Today)
    Name = ""
  End Sub

  ' called by DataPortal to load data from the database
  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    ' retrieve data from db
    Dim crit As Criteria = CType(Criteria, Criteria)
    Dim cn As New SqlConnection(db("PTracker"))
    Dim cm As New SqlCommand

    cn.Open()
    Try
      With cm
        .Connection = cn
        .CommandType = CommandType.StoredProcedure
        .CommandText = "getProject"
        .Parameters.Add("@ID", crit.ID)

        Dim dr As New SafeDataReader(.ExecuteReader)
        Try
          dr.Read()
          With dr
            mID = .GetGuid(0)
            mName = .GetString(1)
            mStarted = .GetSmartDate(2, mStarted.EmptyIsMin)
            mEnded = .GetSmartDate(3, mEnded.EmptyIsMin)
            mDescription = .GetString(4)

            ' load child objects
            .NextResult()
            mResources = ProjectResources.GetProjectResources(dr)
          End With

        Finally
          dr.Close()
        End Try
      End With
      MarkOld()

    Finally
      cn.Close()
    End Try

  End Sub

  ' called by DataPortal to delete/add/update data into the database
  <Transactional()> _
  Protected Overrides Sub DataPortal_Update()
    ' save data into db
    Dim cn As New SqlConnection(db("PTracker"))
    Dim cm As New SqlCommand

    cn.Open()
    Try
      With cm
        .Connection = cn
        .CommandType = CommandType.StoredProcedure
        If Me.IsDeleted Then
          ' we're being deleted
          If Not Me.IsNew Then
            ' we're not new, so get rid of our data
            .CommandText = "deleteProject"
            .Parameters.Add("@ID", mID.ToString)
            .ExecuteNonQuery()
          End If
          ' reset our status to be a new object
          MarkNew()

        Else
          ' we're not being deleted, so insert or update
          If Me.IsNew Then
            ' we're new, so insert
            .CommandText = "addProject"

          Else
            ' we're not new, so update
            .CommandText = "updateProject"

          End If

          .Parameters.Add("@ID", mID.ToString)
          .Parameters.Add("@Name", mName)
          .Parameters.Add("@Started", mStarted.DBValue)
          .Parameters.Add("@Ended", mEnded.DBValue)
          .Parameters.Add("@Description", mDescription)

          .ExecuteNonQuery()

          ' make sure we're marked as an old object
          MarkOld()
        End If

      End With

    Finally
      cn.Close()
    End Try

    ' update child objects
    mResources.Update(Me)

  End Sub

  <Transactional()> _
  Protected Overrides Sub DataPortal_Delete(ByVal Criteria As Object)
    Dim crit As Criteria = CType(Criteria, Criteria)
    Dim cn As New SqlConnection(db("PTracker"))
    Dim cm As New SqlCommand

    cn.Open()

    Try
      With cm
        .Connection = cn
        .CommandType = CommandType.StoredProcedure
        .CommandText = "deleteProject"
        .Parameters.Add("@ID", crit.ID.ToString)
        .ExecuteNonQuery()
      End With

    Finally
      cn.Close()
    End Try
  End Sub

#End Region

End Class
