Imports System.Security.Principal
Imports System.Data.SqlClient

''' <summary>
''' 
''' </summary>
Namespace Security

  ''' <summary>
  ''' Implements a custom Identity class that supports
  ''' CSLA .NET data access via the DataPortal.
  ''' </summary>
  <Serializable()> _
  Public Class BusinessIdentity
    Inherits ReadOnlyBase

    Implements IIdentity

    Private mUsername As String
    Private mRoles As New ArrayList

#Region " IIdentity "

    ''' <summary>
    ''' Implements the IsAuthenticated property defined by IIdentity.
    ''' </summary>
    Public ReadOnly Property IsAuthenticated() As Boolean _
        Implements IIdentity.IsAuthenticated
      Get
        Return Len(mUsername) > 0
      End Get
    End Property

    ''' <summary>
    ''' Implements the AuthenticationType property defined by IIdentity.
    ''' </summary>
    Public ReadOnly Property AuthenticationType() As String _
        Implements IIdentity.AuthenticationType
      Get
        Return "CSLA"
      End Get
    End Property

    ''' <summary>
    ''' Implements the Name property defined by IIdentity.
    ''' </summary>
    Public ReadOnly Property Name() As String _
        Implements IIdentity.Name
      Get
        Return mUsername
      End Get
    End Property

#End Region

    Friend Function IsInRole(ByVal Role As String) As Boolean
      Return mRoles.Contains(Role)
    End Function

#Region " Create and Load "

    Friend Shared Function LoadIdentity(ByVal UserName As String, ByVal Password As String) As BusinessIdentity
      Return CType(DataPortal.Fetch(New Criteria(UserName, Password)), BusinessIdentity)
    End Function

    <Serializable()> _
    Private Class Criteria
      Public Username As String
      Public Password As String

      Public Sub New(ByVal Username As String, ByVal Password As String)
        Me.Username = Username
        Me.Password = Password
      End Sub
    End Class

    Private Sub New()
      ' prevent direct creation
    End Sub

#End Region

#Region " Data access "

    ''' <summary>
    ''' Retrieves the identity data for a specific user.
    ''' </summary>
    Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
      Dim crit As Criteria = CType(Criteria, Criteria)

      mRoles.Clear()

      Dim cn As New SqlConnection(db("Security"))
      Dim cm As New SqlCommand()
      cn.Open()
      Try
        cm.Connection = cn
        cm.CommandText = "Login"
        cm.CommandType = CommandType.StoredProcedure
        cm.Parameters.Add("@user", crit.Username)
        cm.Parameters.Add("@pw", crit.Password)
        Dim dr As SqlDataReader = cm.ExecuteReader()
        Try
          If dr.Read() Then
            mUsername = crit.Username

            If dr.NextResult Then
              While dr.Read
                mRoles.Add(dr.GetString(0))
              End While
            End If

          Else
            mUsername = ""
          End If

        Finally
          dr.Close()
        End Try

      Finally
        cn.Close()
      End Try
    End Sub

#End Region

  End Class

End Namespace
