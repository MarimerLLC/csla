Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Csla

Namespace $rootnamespace$

  <Serializable()> _
  Public Class $safeitemname$
    Inherits CommandBase

#Region " Authorization Rules "

    Public Shared Function CanExecuteCommand() As Boolean

      'TODO: customize to check user role
      'Return Csla.ApplicationContext.User.IsInRole("")
      Return True

    End Function

#End Region

#Region " Client-side Code "

    Private _result As Boolean

    Public ReadOnly Property Result() As Boolean
      Get
        Return _result
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

    Public Shared Function Execute() As Boolean
      If Not CanExecuteCommand() Then
        Throw New System.Security.SecurityException("Not authorized to execute command")
      End If
      Dim cmd As New $safeitemname$()
      cmd.BeforeServer()
      cmd = DataPortal.Execute(Of $safeitemname$)(cmd)
      cmd.AfterServer()
      Return cmd.Result
    End Function

    Private Sub New()
      ' require use of factory methods
    End Sub

#End Region

#Region " Server-side Code "

    Protected Overrides Sub DataPortal_Execute()

      ' TODO: implement code to run on server
      ' here - and set result value(s)
      _result = True

    End Sub

#End Region

  End Class
End Namespace