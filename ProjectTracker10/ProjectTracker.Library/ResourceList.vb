Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class ResourceList
  Inherits CSLA.ReadOnlyCollectionBase

#Region " Data Structure "

  <Serializable()> _
  Public Structure ResourceInfo
    ' this has private members, public properties because
    ' ASP.NET can't databind to public members of a structure...
    Private mID As String
    Private mName As String

    Public Property ID() As String
      Get
        Return mID
      End Get
      Set(ByVal Value As String)
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

    Public Overloads Function Equals(ByVal info As ResourceInfo) As Boolean
      Return mID = info.ID
    End Function
  End Structure

#End Region

#Region " Business Properties and Methods "

  Default Public ReadOnly Property Item(ByVal Index As Integer) As ResourceInfo
    Get
      Return CType(list.Item(Index), ResourceInfo)
    End Get
  End Property

#End Region

#Region " Contains "

  Public Overloads Function Contains( _
    ByVal item As ResourceInfo) As Boolean

    Dim child As ResourceInfo

    For Each child In list
      If child.Equals(item) Then
        Return True
      End If
    Next

    Return False

  End Function

#End Region

#Region " Shared Methods "

  Public Shared Function EmptyList() As ResourceList
    Return New ResourceList
  End Function

  Public Shared Function GetResourceList() As ResourceList
    Return CType(DataPortal.Fetch(New Criteria), ResourceList)
  End Function

  <Serializable()> _
  Public Class Criteria
    ' no criteria - we retrieve all resources
  End Class

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
        .CommandText = "getResources"

        Dim dr As New SafeDataReader(.ExecuteReader)
        Try
          While dr.Read()
            Dim info As ResourceInfo
            info.ID = dr.GetString(0)
            info.Name = dr.GetString(1) & ", " & dr.GetString(2)
            innerlist.Add(info)
          End While

        Finally
          dr.Close()
        End Try

      End With

    Finally
      cn.Close()
    End Try
  End Sub

#End Region

End Class
