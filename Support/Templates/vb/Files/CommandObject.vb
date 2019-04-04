<Serializable()> _
Public Class CommandObject
  Inherits CommandBase(Of CommandObject)

#Region " Authorization Rules "

  Public Shared Function CanExecuteCommand() As Boolean

    'TODO: customize to check user role
    'Return Csla.ApplicationContext.User.IsInRole("")
    Return True

  End Function

#End Region

#Region " Client-side Code "

  Public Shared ReadOnly ResultProperty As PropertyInfo(Of Boolean) = RegisterProperty(Of Boolean)(Function(p) p.Result)
  Public Property Result() As Boolean
	Get
      Return ReadProperty(ResultProperty)
    End Get
    Private Set
      LoadProperty(ResultProperty, value)
    End Set
  End Property


  Private Sub BeforeServer()
    ' implement code to run on client
    ' before server is called
  End Sub

  Private Sub AfterServer()
    ' implement code to run on client
    ' after server is called
  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function Execute() As Boolean
    If Not CanExecuteCommand() Then
      Throw New System.Security.SecurityException("Not authorized to execute command")
    End If
    Dim cmd As New CommandObject()
    cmd.BeforeServer()
    cmd = DataPortal.Execute(Of CommandObject)(cmd)
    cmd.AfterServer()
    Return cmd.Result
  End Function

#End Region

#Region " Server-side Code "

  Protected Overrides Sub DataPortal_Execute()

    ' TODO: implement code to run on server
    ' here - and set result value(s)
    _result = True

  End Sub

#End Region

End Class
