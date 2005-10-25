Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class ProjectList
  Inherits ReadOnlyListBase(Of ProjectInfo)

#Region " ProjectInfo Class "

  <Serializable()> _
  Public Class ProjectInfo
    Private mID As Guid
    Private mName As String

    Public Property ID() As Guid
      Get
        Return mID
      End Get
      Set(ByVal Value As Guid)
        mID = Value
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
      Return mID.GetHashCode
    End Function
  End Class

#End Region

#Region " Criteria "

  <Serializable()> _
  Private Class Criteria
    ' no criteria - we retrieve all projects
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

#End Region

#Region " Data Access "

  Private ReadOnly Property DbConn() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("PTracker").ConnectionString
    End Get
  End Property

  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)

    Using cn As New SqlConnection(DbConn)
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
              Me.Add(info)
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
