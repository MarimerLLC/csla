Imports System.Security.Principal

Namespace Security

  <Serializable()> _
  Public Class PTIdentity
    Inherits ReadOnlyBase(Of PTIdentity)

    Implements IIdentity

#Region " Business Methods "

    Protected Overrides Function GetIdValue() As Object

      Return mName

    End Function

#Region " IsInRole "

    Private mRoles As New List(Of String)

    Friend Function IsInRole(ByVal role As String) As Boolean

      Return mRoles.Contains(role)

    End Function

#End Region

#Region " IIdentity "

    Private mIsAuthenticated As Boolean
    Private mName As String = ""

    Public ReadOnly Property AuthenticationType() As String _
      Implements System.Security.Principal.IIdentity.AuthenticationType
      Get
        Return "Csla"
      End Get
    End Property

    Public ReadOnly Property IsAuthenticated() As Boolean _
      Implements System.Security.Principal.IIdentity.IsAuthenticated
      Get
        Return mIsAuthenticated
      End Get
    End Property

    Public ReadOnly Property Name() As String _
      Implements System.Security.Principal.IIdentity.Name
      Get
        Return mName
      End Get
    End Property

#End Region

#End Region

#Region " Factory Methods "

    Friend Shared Function UnauthenticatedIdentity() As PTIdentity

      Return New PTIdentity

    End Function

    Friend Shared Function GetIdentity( _
      ByVal username As String, ByVal password As String) As PTIdentity

      Return DataPortal.Fetch(Of PTIdentity)(New Criteria(username, password))

    End Function

    Private Sub New()
      ' require use of factory methods
    End Sub

#End Region

#Region " Data Access "

    <Serializable()> _
    Private Class Criteria

      Private mUsername As String
      Private mPassword As String

      Public ReadOnly Property Username() As String
        Get
          Return mUsername
        End Get
      End Property

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

    Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

      Using cn As New SqlConnection(Database.SecurityConnection)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandText = "Login"
          cm.CommandType = CommandType.StoredProcedure
          cm.Parameters.AddWithValue("@user", criteria.Username)
          cm.Parameters.AddWithValue("@pw", criteria.Password)
          Using dr As SqlDataReader = cm.ExecuteReader()
            If dr.Read() Then
              mName = criteria.Username
              mIsAuthenticated = True
              If dr.NextResult Then
                While dr.Read
                  mRoles.Add(dr.GetString(0))
                End While
              End If

            Else
              mName = ""
              mIsAuthenticated = False
              mRoles.Clear()
            End If
          End Using
        End Using
      End Using

    End Sub

#End Region

  End Class

End Namespace
