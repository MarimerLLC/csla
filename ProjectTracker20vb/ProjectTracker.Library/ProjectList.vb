Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class ProjectList
  Inherits ReadOnlyListBase(Of ProjectList, ProjectInfo)

#Region " ProjectInfo Class "

  <Serializable()> _
  Public Class ProjectInfo
    Private mId As Guid
    Private mName As String

    Public Property Id() As Guid
      Get
        Return mId
      End Get
      Set(ByVal Value As Guid)
        mId = Value
      End Set
    End Property

    Public Property Name() As String
      Get
        Return mName
      End Get
      Set(ByVal Value As String)
        mName = Value
      End Set
    End Property

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
      If TypeOf obj Is ProjectInfo Then
        Return mID.Equals(CType(obj, ProjectInfo).ID)
      Else
        Return False
      End If
    End Function

    Public Overrides Function GetHashCode() As Integer
      Return mId.GetHashCode
    End Function
  End Class

#End Region

#Region " Criteria "

  <Serializable()> _
  Private Class Criteria
    ' no criteria - we retrieve all projects
  End Class

  <Serializable()> _
  Private Class FilteredCriteria
    Private mName As String
    Public ReadOnly Property Name() As String
      Get
        Return mName
      End Get
    End Property

    Public Sub New(ByVal name As String)
      mName = name
    End Sub
  End Class

#End Region

#Region " Constructors "

  Private Sub New()
    ' prevent direct creation
  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function GetProjectList() As ProjectList

    Return DataPortal.Fetch(Of ProjectList)(New Criteria)

  End Function

  Public Shared Function GetProjectList(ByVal name As String) As ProjectList

    Return DataPortal.Fetch(Of ProjectList)(New FilteredCriteria(name))

  End Function

#End Region

#Region " Data Access "

  Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

    Dim filter As String = ""
    If TypeOf criteria Is FilteredCriteria Then
      filter = CType(criteria, FilteredCriteria).Name
    End If

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "getProjects"
          Using dr As New SafeDataReader(.ExecuteReader)
            IsReadOnly = False
            While dr.Read()
              Dim info As New ProjectInfo
              info.ID = dr.GetGuid(0)
              info.Name = dr.GetString(1)
              ' apply filter if necessary
              If Len(filter) = 0 OrElse info.Name.IndexOf(filter) = 0 Then
                Me.Add(info)
              End If
            End While
            IsReadOnly = True
            dr.Close()
          End Using
        End With
      End Using
      cn.Close()
    End Using

  End Sub

#End Region

End Class
