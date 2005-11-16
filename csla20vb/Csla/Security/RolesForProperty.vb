Imports System.Security.Principal

Namespace Security

  ''' <summary>
  ''' Maintains a list of allowed and denied
  ''' user roles for a specific property.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Friend Class RolesForProperty
    Private mReadAllowed As New List(Of String)
    Private mReadDenied As New List(Of String)
    Private mWriteAllowed As New List(Of String)
    Private mWriteDenied As New List(Of String)

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles allowed read access.
    ''' </summary>
    Public ReadOnly Property ReadAllowed() As List(Of String)
      Get
        Return mReadAllowed
      End Get
    End Property

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles denied read access.
    ''' </summary>
    Public ReadOnly Property ReadDenied() As List(Of String)
      Get
        Return mReadDenied
      End Get
    End Property

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles allowed write access.
    ''' </summary>
    Public ReadOnly Property WriteAllowed() As List(Of String)
      Get
        Return mWriteAllowed
      End Get
    End Property

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles denied write access.
    ''' </summary>
    Public ReadOnly Property WriteDenied() As List(Of String)
      Get
        Return mWriteDenied
      End Get
    End Property

    ''' <summary>
    ''' Returns True if the user is in a role
    ''' explicitly allowed read access.
    ''' </summary>
    ''' <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    ''' <returns>True if the user is allowed read access.</returns>
    ''' <remarks></remarks>
    Public Function IsReadAllowed(ByVal principal As IPrincipal) As Boolean

      Dim result As Boolean
      For Each role As String In ReadAllowed
        If principal.IsInRole(role) Then
          result = True
          Exit For
        End If
      Next
      Return result

    End Function

    ''' <summary>
    ''' Returns True if the user is in a role
    ''' explicitly denied read access.
    ''' </summary>
    ''' <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    ''' <returns>True if the user is denied read access.</returns>
    ''' <remarks></remarks>
    Public Function IsReadDenied(ByVal principal As IPrincipal) As Boolean

      Dim result As Boolean
      For Each role As String In ReadDenied
        If principal.IsInRole(role) Then
          result = True
          Exit For
        End If
      Next
      Return result

    End Function

    ''' <summary>
    ''' Returns True if the user is in a role
    ''' explicitly allowed write access.
    ''' </summary>
    ''' <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    ''' <returns>True if the user is allowed write access.</returns>
    ''' <remarks></remarks>
    Public Function IsWriteAllowed(ByVal principal As IPrincipal) As Boolean

      Dim result As Boolean
      For Each role As String In WriteAllowed
        If principal.IsInRole(role) Then
          result = True
          Exit For
        End If
      Next
      Return result

    End Function

    ''' <summary>
    ''' Returns True if the user is in a role
    ''' explicitly denied write access.
    ''' </summary>
    ''' <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    ''' <returns>True if the user is denied write access.</returns>
    ''' <remarks></remarks>
    Public Function IsWriteDenied(ByVal principal As IPrincipal) As Boolean

      Dim result As Boolean
      For Each role As String In WriteDenied
        If principal.IsInRole(role) Then
          result = True
          Exit For
        End If
      Next
      Return result

    End Function

  End Class

End Namespace
