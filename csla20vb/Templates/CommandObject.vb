<Serializable()> _
Public Class CommandObject
  Inherits CommandBase

#Region " Authorization Rules "

  Public Shared Function CanExecuteCommand() As Boolean

    ' to see if user is authorized
    'Return Csla.ApplicationContext.User.IsInRole("")
    Return True

  End Function

#End Region

#Region " Client-side Code "

  Private mResult As Boolean

  Public ReadOnly Property Result() As Boolean
    Get
      Return mResult
    End Get
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

  Public Shared Function TheCommand() As Boolean

    Dim cmd As New CommandObject
    cmd.BeforeServer()
    cmd = DataPortal.Execute(Of CommandObject)(cmd)
    cmd.AfterServer()
    Return cmd.Result

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Server-side Code "

  Protected Overrides Sub DataPortal_Execute()

    ' implement code to run on server
    ' here - and set result value(s)
    mResult = True

  End Sub

#End Region

End Class
