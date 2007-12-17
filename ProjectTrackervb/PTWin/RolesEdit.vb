Imports ProjectTracker.Library.Admin

Public Class RolesEdit
  Inherits WinPart

  Private mRoles As Admin.Roles

  Private Sub RolesEdit_Load( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load

    Try
      mRoles = Admin.Roles.GetRoles

    Catch ex As Csla.DataPortalException
      MessageBox.Show(ex.BusinessException.ToString, _
        "Error loading", MessageBoxButtons.OK, _
        MessageBoxIcon.Exclamation)

    Catch ex As Exception
      MessageBox.Show(ex.ToString, _
        "Error loading", MessageBoxButtons.OK, _
        MessageBoxIcon.Exclamation)
    End Try

    If mRoles IsNot Nothing Then
      Me.RolesBindingSource.DataSource = mRoles
    End If

  End Sub

  Protected Overrides Function GetIdValue() As Object

    Return "Edit Roles"

  End Function

  Private Sub SaveButton_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles SaveButton.Click

    ' stop the flow of events
    Me.RolesBindingSource.RaiseListChangedEvents = False
    ' commit edits in memory
    UnbindBindingSource(Me.RolesBindingSource, True, True)
    Try
      mRoles = mRoles.Save
      Me.Close()

    Catch ex As Csla.DataPortalException
      MessageBox.Show(ex.BusinessException.ToString, _
        "Error saving", MessageBoxButtons.OK, _
        MessageBoxIcon.Exclamation)

    Catch ex As Exception
      MessageBox.Show(ex.ToString, _
        "Error saving", MessageBoxButtons.OK, _
        MessageBoxIcon.Exclamation)

    Finally
      Me.RolesBindingSource.DataSource = mRoles

      Me.RolesBindingSource.RaiseListChangedEvents = True
      Me.RolesBindingSource.ResetBindings(False)
    End Try

  End Sub

  Private Sub CancelButton_Click( _
    ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CancelButton.Click

    Me.Close()

  End Sub

  Private Sub RolesEdit_CurrentPrincipalChanged( _
    ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles Me.CurrentPrincipalChanged

    If Not Roles.CanEditObject Then
      Me.Close()
    End If

  End Sub

End Class
