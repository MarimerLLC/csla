Namespace Security

    ''' <summary>
    ''' Interface defining an object that
    ''' checks IsInRole.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ICheckRoles
        ''' <summary>
        ''' Returns a value indicating whether the current
        ''' user is in the specified security role.
        ''' </summary>
        ''' <param name="role">
        ''' Role to check.
        ''' </param>
        Function IsInRole(ByVal role As String) As Boolean

    End Interface
End Namespace

