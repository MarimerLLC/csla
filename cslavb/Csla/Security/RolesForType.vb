Namespace Security

  Friend Class RolesForType

    Private _allowCreateRoles As List(Of String)
    Friend ReadOnly Property AllowCreateRoles() As List(Of String)
      Get
        Return _allowCreateRoles
      End Get
    End Property

    Private _denyCreateRoles As List(Of String)
    Friend ReadOnly Property DenyCreateRoles() As List(Of String)
      Get
        Return _denyCreateRoles
      End Get
    End Property

    Private _allowGetRoles As List(Of String)
    Friend ReadOnly Property AllowGetRoles() As List(Of String)
      Get
        Return _allowGetRoles
      End Get
    End Property

    Private _denyGetRoles As List(Of String)
    Friend ReadOnly Property DenyGetRoles() As List(Of String)
      Get
        Return _denyGetRoles
      End Get
    End Property

    Private _allowEditRoles As List(Of String)
    Friend ReadOnly Property AllowEditRoles() As List(Of String)
      Get
        Return _allowEditRoles
      End Get
    End Property

    Private _denyEditRoles As List(Of String)
    Friend ReadOnly Property DenyEditRoles() As List(Of String)
      Get
        Return _denyEditRoles
      End Get
    End Property

    Private _allowDeleteRoles As List(Of String)
    Friend ReadOnly Property AllowDeleteRoles() As List(Of String)
      Get
        Return _allowDeleteRoles
      End Get
    End Property

    Private _denyDeleteRoles As List(Of String)
    Friend ReadOnly Property DenyDeleteRoles() As List(Of String)
      Get
        Return _denyDeleteRoles
      End Get
    End Property

    ''' <summary>
    ''' Specify the roles allowed to get (fetch)
    ''' a given type of business object.
    ''' </summary>
    ''' <param name="roles">List of roles.</param>
    Friend Sub AllowGet(ByVal ParamArray roles() As String)
      If _allowGetRoles Is Nothing Then
        _allowGetRoles = New List(Of String)()
      End If
      _allowGetRoles.AddRange(roles)
    End Sub

    ''' <summary>
    ''' Specify the roles not allowed to get (fetch)
    ''' a given type of business object.
    ''' </summary>
    ''' <param name="roles">List of roles.</param>
    Friend Sub DenyGet(ByVal ParamArray roles() As String)
      If _denyGetRoles Is Nothing Then
        _denyGetRoles = New List(Of String)()
      End If
      _denyGetRoles.AddRange(roles)
    End Sub

    ''' <summary>
    ''' Specify the roles allowed to edit (save)
    ''' a given type of business object.
    ''' </summary>
    ''' <param name="roles">List of roles.</param>
    Friend Sub AllowEdit(ByVal ParamArray roles() As String)
      If _allowEditRoles Is Nothing Then
        _allowEditRoles = New List(Of String)()
      End If
      _allowEditRoles.AddRange(roles)
    End Sub

    ''' <summary>
    ''' Specify the roles not allowed to edit (save)
    ''' a given type of business object.
    ''' </summary>
    ''' <param name="roles">List of roles.</param>
    Friend Sub DenyEdit(ByVal ParamArray roles() As String)
      If _denyEditRoles Is Nothing Then
        _denyEditRoles = New List(Of String)()
      End If
      _denyEditRoles.AddRange(roles)
    End Sub

    ''' <summary>
    ''' Specify the roles allowed to create
    ''' a given type of business object.
    ''' </summary>
    ''' <param name="roles">List of roles.</param>
    Friend Sub AllowCreate(ByVal ParamArray roles() As String)
      If _allowCreateRoles Is Nothing Then
        _allowCreateRoles = New List(Of String)()
      End If
      _allowCreateRoles.AddRange(roles)
    End Sub

    ''' <summary>
    ''' Specify the roles not allowed to create
    ''' a given type of business object.
    ''' </summary>
    ''' <param name="roles">List of roles.</param>
    Friend Sub DenyCreate(ByVal ParamArray roles() As String)
      If _denyCreateRoles Is Nothing Then
        _denyCreateRoles = New List(Of String)()
      End If
      _denyCreateRoles.AddRange(roles)
    End Sub

    ''' <summary>
    ''' Specify the roles allowed to delete
    ''' a given type of business object.
    ''' </summary>
    ''' <param name="roles">List of roles.</param>
    Friend Sub AllowDelete(ByVal ParamArray roles() As String)
      If _allowDeleteRoles Is Nothing Then
        _allowDeleteRoles = New List(Of String)()
      End If
      _allowDeleteRoles.AddRange(roles)
    End Sub

    ''' <summary>
    ''' Specify the roles not allowed to delete
    ''' a given type of business object.
    ''' </summary>
    ''' <param name="roles">List of roles.</param>
    Friend Sub DenyDelete(ByVal ParamArray roles() As String)
      If _denyDeleteRoles Is Nothing Then
        _denyDeleteRoles = New List(Of String)()
      End If
      _denyDeleteRoles.AddRange(roles)
    End Sub

  End Class

End Namespace