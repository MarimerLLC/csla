Imports System.Data.SqlClient

<Serializable()> _
Public Class ClientProjectInfo
  Inherits ReadOnlyBase(Of ClientProjectInfo)

#Region " Business Methods "

  Private _id As Integer
  Public ReadOnly Property Id() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _id
    End Get
  End Property

  Private _clientId As Integer
  Public ReadOnly Property ClientId() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _clientId
    End Get
  End Property

  Private _name As String = ""
  Public ReadOnly Property Name() As String
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _name
    End Get
  End Property

  Private _description As String = ""
  Public ReadOnly Property Description() As String
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _description
    End Get
  End Property

  Protected Overrides Function GetIdValue() As Object

    Return _id

  End Function

#End Region

#Region " Factory Methods "

  Friend Shared Function GetProject(ByVal dr As Csla.Data.SafeDataReader) As ClientProjectInfo
    Return New ClientProjectInfo(dr)
  End Function

  Private Sub New()

  End Sub

  Private Sub New(ByVal dr As Csla.Data.SafeDataReader)
    Me.New()
    Fetch(dr)
  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal dr As Csla.Data.SafeDataReader)

    _id = dr.GetInt32("id")
    _clientId = dr.GetInt32("clientId")
    _name = dr.GetString("name")
    _description = dr.GetString("description")

  End Sub

#End Region

End Class
