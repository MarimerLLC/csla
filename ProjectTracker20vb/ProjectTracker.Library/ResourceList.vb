Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class ResourceList
  Inherits ReadOnlyListBase(Of ResourceList, ResourceInfo)

#Region " ResourceInfo Class "

  <Serializable()> _
  Public Class ResourceInfo

    Private mId As String
    Private mName As String

    Public Property Id() As String
      Get
        Return mId
      End Get
      Friend Set(ByVal Value As String)
        mId = Value
      End Set
    End Property

    Public Property Name() As String
      Get
        Return mName
      End Get
      Friend Set(ByVal Value As String)
        mName = Value
      End Set
    End Property

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean

      If TypeOf obj Is ResourceInfo Then
        Return CType(obj, ResourceInfo).Id = mId

      Else
        Return False
      End If

    End Function

    Public Overrides Function ToString() As String

      Return Name

    End Function

    Public Overrides Function GetHashCode() As Integer
      Return mId.GetHashCode
    End Function

  End Class

#End Region

#Region " Shared Methods "

  Public Shared Function EmptyList() As ResourceList

    Return New ResourceList

  End Function

  Public Shared Function GetResourceList() As ResourceList

    Return DataPortal.Fetch(Of ResourceList)(New Criteria)

  End Function

  <Serializable()> _
  Private Class Criteria
    ' no criteria - we retrieve all resources
  End Class

  Private Sub New()
    ' prevent direct creation
  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    Using cn As New SqlConnection(DataBase.DbConn)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "getResources"

          Using dr As New SafeDataReader(.ExecuteReader)
            IsReadOnly = False
            While dr.Read()
              Dim info As New ResourceInfo
              info.Id = dr.GetString("id")
              info.Name = _
                dr.GetString("LastName") & _
                ", " & dr.GetString("FirstName")
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
