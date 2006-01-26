Public Class RolesEdit
  Inherits WinPart

  Private mRoles As Admin.Roles

  Private Sub RolesEdit_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

  Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click

    Me.RolesBindingSource.RaiseListChangedEvents = False
    Dim temp As Admin.Roles = mRoles.Clone
    Try
      mRoles = temp.Save
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
      Me.RolesBindingSource.RaiseListChangedEvents = True
    End Try

  End Sub

  Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelButton.Click

    Me.Close()

  End Sub

End Class
