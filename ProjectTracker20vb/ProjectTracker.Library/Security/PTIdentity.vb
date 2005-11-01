Imports System.Data.SqlClient
Imports System.Security.Principal

Namespace Security

  <Serializable()> _
  Public Class PTIdentity
    Inherits ReadOnlyBase(Of PTIdentity)

    Implements IIdentity

#Region " IsInRole "

    Private mRoles As New List(Of String)

    Friend Function IsInRole(ByVal role As String) As Boolean

      Return mRoles.Contains(role)

    End Function

#End Region

#Region " IIdentity "

    Private mIsAuthenticated As Boolean
    Private mName As String = ""

    Public ReadOnly Property AuthenticationType() As String Implements System.Security.Principal.IIdentity.AuthenticationType
      Get
        Return "Csla"
      End Get
    End Property

    Public ReadOnly Property IsAuthenticated() As Boolean Implements System.Security.Principal.IIdentity.IsAuthenticated
      Get
        Return mIsAuthenticated
      End Get
    End Property

    Public ReadOnly Property Name() As String Implements System.Security.Principal.IIdentity.Name
      Get
        Return mName
      End Get
    End Property

#End Region

#Region " Constructors "

    Private Sub New()

    End Sub

#End Region

#Region " Criteria "

    <Serializable()> _
    Private Class Criteria

      Private mUsername As String
      Public ReadOnly Property Username() As String
        Get
          Return mUsername
        End Get
      End Property

      Private mPassword As String
      Public ReadOnly Property Password() As String
        Get
          Return mPassword
        End Get
      End Property

      Public Sub New(ByVal username As String, ByVal password As String)
        mUsername = username
        mPassword = password
      End Sub
    End Class

#End Region

#Region " Factory Methods "

    Friend Shared Function UnauthenticatedIdentity() As PTIdentity

      Return New PTIdentity

    End Function

    Friend Shared Function GetIdentity(ByVal username As String, ByVal password As String) As PTIdentity

      Return DataPortal.Fetch(Of PTIdentity)(New Criteria(username, password))

    End Function

#End Region

#Region " Data Access "

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = CType(criteria, Criteria)

      Using cn As New SqlConnection(DataBase.SecurityConn)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandText = "Login"
          cm.CommandType = CommandType.StoredProcedure
          cm.Parameters.AddWithValue("@user", crit.Username)
          cm.Parameters.AddWithValue("@pw", crit.Password)
          Using dr As SqlDataReader = cm.ExecuteReader()
            If dr.Read() Then
              mName = crit.Username
              mIsAuthenticated = True
              If dr.NextResult Then
                While dr.Read
                  mRoles.Add(dr.GetString(0))
                End While
              End If

            Else
              mName = ""
              mIsAuthenticated = False
            End If
            dr.Close()
          End Using
        End Using
        cn.Close()
      End Using

    End Sub

#End Region

  End Class

End Namespace
