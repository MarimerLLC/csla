Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class ProjectList
  Inherits CSLA.ReadOnlyCollectionBase

#Region " Data Structure "

  <Serializable()> _
  Public Structure ProjectInfo
    ' this has private members, public properties because
    ' ASP.NET can't databind to public members of a structure...
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

    Public Overloads Function Equals(ByVal info As ProjectInfo) As Boolean
      Return mID.Equals(info.ID)
    End Function
  End Structure

#End Region

#Region " Business Properties and Methods "

  Default Public ReadOnly Property Item(ByVal Index As Integer) As ProjectInfo
    Get
      Return CType(list.Item(Index), ProjectInfo)
    End Get
  End Property

#End Region

#Region " Contains "

  Public Overloads Function Contains( _
    ByVal item As ProjectInfo) As Boolean

    Dim child As ProjectInfo

    For Each child In list
      If child.Equals(item) Then
        Return True
      End If
    Next

    Return False

  End Function

#End Region

#Region " Shared Methods "

  Public Shared Function GetProjectList() As ProjectList
    Return CType(DataPortal.Fetch(New Criteria), ProjectList)
  End Function

#End Region

#Region " Criteria "

  <Serializable()> _
  Public Class Criteria
    ' no criteria - we retrieve all projects
  End Class

#End Region

#Region " Constructors "

  Protected Sub New()
    ' prevent direct creation
  End Sub

#End Region

#Region " Data Access "

  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    Dim cn As New SqlConnection(db("PTracker"))
    Dim cm As New SqlCommand

    cn.Open()
    Try
      With cm
        .Connection = cn
        .CommandType = CommandType.StoredProcedure
        .CommandText = "getProjects"

        Dim dr As New SafeDataReader(.ExecuteReader)
        Try
          Locked = False
          While dr.Read()
            Dim info As ProjectInfo
            info.ID = dr.GetGuid(0)
            info.Name = dr.GetString(1)
            List.Add(info)
          End While

        Finally
          Locked = True
          dr.Close()
        End Try

      End With

    Finally
      cn.Close()
    End Try
  End Sub

#End Region

End Class
