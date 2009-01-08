Namespace Security

  ''' <summary>
  ''' Criteria class for passing a
  ''' username/password pair to a
  ''' custom identity class.
  ''' </summary>
  <Serializable()> _
  Public Class UsernameCriteria

    Private _userName As String
    ''' <summary>
    ''' Gets the username.
    ''' </summary>
    Public Property Username() As String
      Get
        Return _userName
      End Get
      Private Set(ByVal value As String)
        _userName = value
      End Set
    End Property

    Private _passWord As String
    ''' <summary>
    ''' Gets the password.
    ''' </summary>
    Public Property Password() As String
      Get
        Return _passWord
      End Get
      Private Set(ByVal value As String)
        _passWord = value
      End Set
    End Property

    Public Sub New(ByVal username As String, ByVal password As String)
      Me.Username = username
      Me.Password = password
    End Sub

  End Class

End Namespace

