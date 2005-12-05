Public Class RolesEdit
  Inherits WinPart

  Private mRoles As Admin.Roles

  Private Sub RolesEdit_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    mRoles = Admin.Roles.GetRoles
    Me.RolesBindingSource.DataSource = mRoles

  End Sub

  Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click

    Me.RolesBindingSource.RaiseListChangedEvents = False
    Dim old As Admin.Roles = mRoles.Clone
    Try
      mRoles = mRoles.Save

    Catch ex As Exception
      mRoles = old
      MessageBox.Show(ex.ToString, "Error saving", _
        MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    End Try
    Me.RolesBindingSource.DataSource = mRoles
    Me.RolesBindingSource.RaiseListChangedEvents = True

  End Sub

  Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelButton.Click

    Me.Close()

  End Sub

End Class
