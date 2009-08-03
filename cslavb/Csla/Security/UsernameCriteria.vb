Imports Csla
Imports Csla.Serialization

Namespace Security

  ''' <summary>
  ''' Criteria class for passing a
  ''' username/password pair to a
  ''' custom identity class.
  ''' </summary>
  <Serializable()> _
  Public Class UsernameCriteria
    Inherits CriteriaBase

    ''' <summary>
    ''' Username property definition.
    ''' </summary>
    Public Shared UsernameProperty As PropertyInfo(Of String) = _
      RegisterProperty(GetType(UsernameCriteria), New PropertyInfo(Of String)("Username", "Username"))

    ''' <summary>
    ''' Gets the username.
    ''' </summary>
    Public Property Username() As String
      Get
        Return ReadProperty(UsernameProperty)
      End Get
      Private Set(ByVal value As String)
        LoadProperty(UsernameProperty, value)
      End Set
    End Property


    ''' <summary>
    ''' Password property definition.
    ''' </summary>
    Public Shared PasswordProperty As PropertyInfo(Of String) = _
      RegisterProperty(GetType(UsernameCriteria), New PropertyInfo(Of String)("Password", "Password"))

    ''' <summary>
    ''' Gets the password.
    ''' </summary>
    Public Property Password() As String
      Get
        Return ReadProperty(PasswordProperty)
      End Get
      Private Set(ByVal value As String)
        LoadProperty(PasswordProperty, value)
      End Set        
    End Property

    ''' <summary>
    ''' Creates a new instance of the object.
    ''' </summary>
    ''' <param name="username">
    ''' Username value.
    ''' </param>
    ''' <param name="password">
    ''' Password value.
    ''' </param>
    Public Sub New(ByVal username As String, ByVal password As String)
      Me.Username = username
      Me.Password = password
    End Sub

    'Creates a new instance of the object.
#If SILVERLIGHT Then
    Public Sub New()

    End Sub
#Else
    Protected Sub New()

    End Sub
#End If

  End Class

End Namespace

