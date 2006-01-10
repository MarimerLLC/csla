Imports System.Data.SqlClient
Imports CSLA.Data

<Serializable()> _
Public Class ResourceList
  Inherits ReadOnlyListBase(Of ResourceList, ResourceInfo)

#Region " ResourceInfo Class "

  <Serializable()> _
  Public Class ResourceInfo
    Inherits ReadOnlyBase(Of ResourceInfo)

    Private mId As Integer
    Private mName As String

    Public Property Id() As Integer
      Get
        Return mId
      End Get
      Friend Set(ByVal Value As Integer)
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

    Protected Overrides Function GetIdValue() As Object
      Return mId
    End Function

    Private Sub New()
      ' require use of factory methods
    End Sub

    Friend Sub New(ByVal dr As SafeDataReader)
      mId = dr.GetInt32("Id")
      mName = String.Format("{0}, {1}", _
        dr.GetString("LastName"), dr.GetString("FirstName"))
    End Sub

  End Class

#End Region

#Region " Factory Methods "

  Public Shared Function EmptyList() As ResourceList

    Return New ResourceList

  End Function

  Public Shared Function GetResourceList() As ResourceList

    Return DataPortal.Fetch(Of ResourceList)(New Criteria)

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class Criteria
    ' no criteria - we retrieve all resources
  End Class


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
              Dim info As New ResourceInfo(dr)
              Me.Add(info)
            End While

            IsReadOnly = True
          End Using
        End With
      End Using
    End Using

  End Sub

#End Region

End Class
