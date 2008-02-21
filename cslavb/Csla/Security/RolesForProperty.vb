Imports System.Security.Principal

Namespace Security

  ''' <summary>
  ''' Maintains a list of allowed and denied
  ''' user roles for a specific property.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Friend Class RolesForProperty
    Private _readAllowed As New List(Of String)
    Private _readDenied As New List(Of String)
    Private _writeAllowed As New List(Of String)
    Private _writeDenied As New List(Of String)
    Private _executeAllowed As New List(Of String)
    Private _executeDenied As New List(Of String)

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles allowed read access.
    ''' </summary>
    Public ReadOnly Property ReadAllowed() As List(Of String)
      Get
        Return _readAllowed
      End Get
    End Property

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles denied read access.
    ''' </summary>
    Public ReadOnly Property ReadDenied() As List(Of String)
      Get
        Return _readDenied
      End Get
    End Property

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles allowed write access.
    ''' </summary>
    Public ReadOnly Property WriteAllowed() As List(Of String)
      Get
        Return _writeAllowed
      End Get
    End Property

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles denied write access.
    ''' </summary>
    Public ReadOnly Property WriteDenied() As List(Of String)
      Get
        Return _writeDenied
      End Get
    End Property

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles allowed execute access.
    ''' </summary>
    Public ReadOnly Property ExecuteAllowed() As List(Of String)
      Get
        Return _executeAllowed
      End Get
    End Property

    ''' <summary>
    ''' Returns a List(Of String) containing the list
    ''' of roles denied execute access.
    ''' </summary>
    Public ReadOnly Property ExecuteDenied() As List(Of String)
      Get
        Return _executeDenied
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

      Return AuthorizationRulesManager.PrincipalRoleInList(principal, ReadAllowed)

    End Function

    ''' <summary>
    ''' Returns True if the user is in a role
    ''' explicitly denied read access.
    ''' </summary>
    ''' <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    ''' <returns>True if the user is denied read access.</returns>
    ''' <remarks></remarks>
    Public Function IsReadDenied(ByVal principal As IPrincipal) As Boolean

      Return AuthorizationRulesManager.PrincipalRoleInList(principal, ReadDenied)

    End Function

    ''' <summary>
    ''' Returns True if the user is in a role
    ''' explicitly allowed write access.
    ''' </summary>
    ''' <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    ''' <returns>True if the user is allowed write access.</returns>
    ''' <remarks></remarks>
    Public Function IsWriteAllowed(ByVal principal As IPrincipal) As Boolean

      Return AuthorizationRulesManager.PrincipalRoleInList(principal, WriteAllowed)

    End Function

    ''' <summary>
    ''' Returns True if the user is in a role
    ''' explicitly denied write access.
    ''' </summary>
    ''' <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    ''' <returns>True if the user is denied write access.</returns>
    ''' <remarks></remarks>
    Public Function IsWriteDenied(ByVal principal As IPrincipal) As Boolean

      Return AuthorizationRulesManager.PrincipalRoleInList(principal, WriteDenied)

    End Function

    ''' <summary>
    ''' Returns True if the user is in a role
    ''' explicitly allowed execute access.
    ''' </summary>
    ''' <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    ''' <returns>True if the user is allowed execute access.</returns>
    ''' <remarks></remarks>
    Public Function IsExecuteAllowed(ByVal principal As IPrincipal) As Boolean

      Return AuthorizationRulesManager.PrincipalRoleInList(principal, ExecuteAllowed)

    End Function

    ''' <summary>
    ''' Returns True if the user is in a role
    ''' explicitly denied execute access.
    ''' </summary>
    ''' <param name="principal">A System.Security.Principal.IPrincipal representing the user.</param>
    ''' <returns>True if the user is denied execute access.</returns>
    ''' <remarks></remarks>
    Public Function IsExecuteDenied(ByVal principal As IPrincipal) As Boolean

      Return AuthorizationRulesManager.PrincipalRoleInList(principal, ExecuteDenied)

    End Function

  End Class

End Namespace
