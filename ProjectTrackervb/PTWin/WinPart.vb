Public Class WinPart
  Inherits System.Windows.Forms.UserControl

  Protected Overridable Function GetIdValue() As Object

    Return Nothing

  End Function

  Public Overrides Function Equals(ByVal obj As Object) As Boolean

    If Me.DesignMode Then
      Return MyBase.Equals(obj)
    Else
      Dim id As Object = GetIdValue()
      If Me.GetType.Equals(obj.GetType) AndAlso id IsNot Nothing Then
        Return CType(obj, WinPart).GetIdValue.Equals(id)

      Else
        Return False
      End If
    End If

  End Function

  Public Overrides Function GetHashCode() As Integer

    Dim id As Object = GetIdValue()
    If id IsNot Nothing Then
      Return id.GetHashCode

    Else
      Return MyBase.GetHashCode
    End If

  End Function

  Public Overrides Function ToString() As String

    Dim id As Object = GetIdValue()
    If id IsNot Nothing Then
      Return id.ToString

    Else
      Return MyBase.ToString
    End If

  End Function

#Region " CloseWinPart "

  Public Event CloseWinPart As EventHandler

  Protected Sub Close()

    RaiseEvent CloseWinPart(Me, EventArgs.Empty)

  End Sub

#End Region

#Region " CurrentPrincipalChanged "

  Protected Event CurrentPrincipalChanged As EventHandler

  Protected Friend Overridable Sub OnCurrentPrincipalChanged( _
    ByVal sender As Object, ByVal e As EventArgs)

    RaiseEvent CurrentPrincipalChanged(sender, e)

  End Sub

#End Region

End Class
