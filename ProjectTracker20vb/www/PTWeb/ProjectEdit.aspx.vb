
Partial Class ProjectEdit
  Inherits System.Web.UI.Page

  'Protected Sub CslaDataSource1_Deleting(ByVal sender As Object, ByVal e As System.EventArgs) Handles CslaDataSource1.UpdateObject

  '  CType(sender, DataControls.CslaDataSource).BusinessObject = Session("currentObject")

  'End Sub

  'Protected Sub CslaDataSource1_Inserting(ByVal sender As Object, ByVal e As System.EventArgs) Handles CslaDataSource1.UpdateObject

  '  CType(sender, DataControls.CslaDataSource).BusinessObject = Session("currentObject")

  'End Sub

  'Protected Sub CslaDataSource1_Selecting(ByVal sender As Object, ByVal e As System.EventArgs) Handles CslaDataSource1.SelectObject

  '  Dim idString As String = Request.QueryString("id")
  '  Dim obj As ProjectTracker.Library.Project
  '  If Len(idString) > 0 Then
  '    Dim id As Guid
  '    id = New Guid(idString)
  '    obj = ProjectTracker.Library.Project.GetProject(id)

  '  Else
  '    obj = ProjectTracker.Library.Project.NewProject
  '  End If
  '  Session("currentObject") = obj
  '  CType(sender, DataControls.CslaDataSource).BusinessObject = obj

  'End Sub

  'Protected Sub CslaDataSource1_Updating(ByVal sender As Object, ByVal e As System.EventArgs) Handles CslaDataSource1.UpdateObject

  '  CType(sender, DataControls.CslaDataSource).BusinessObject = Session("currentObject")

  'End Sub


  Protected Sub CslaDataSource1_SelectObject(ByVal sender As Object, ByVal e As DataControls.SelectObjectArgs) Handles CslaDataSource1.SelectObject

    Dim idString As String = Request.QueryString("id")
    Dim obj As ProjectTracker.Library.Project
    If Len(idString) > 0 Then
      Dim id As Guid
      id = New Guid(idString)
      obj = ProjectTracker.Library.Project.GetProject(id)

    Else
      obj = ProjectTracker.Library.Project.NewProject
    End If
    Session("currentObject") = obj
    e.BusinessObject = obj

  End Sub

End Class
