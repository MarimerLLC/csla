
Partial Class ProjectList
  Inherits System.Web.UI.Page

  Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged

    Dim idString As String = GridView1.SelectedDataKey.Value.ToString
    Response.Redirect("ProjectEdit.aspx?id=" & idString)

  End Sub

End Class
