Public Class Form1

  Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Dim list As RootList = RootList.GetList
    'Dim list As ChildList = New ChildList
    
    Me.ChildListBindingSource.DataSource = list

    Dim sorted As New Csla.SortedBindingList(Of Root)(list)
    Dim bs As New BindingSource
    Me.DataGridView1.DataSource = bs
    bs.DataSource = sorted

  End Sub

  Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

    Dim list As Csla.SortedBindingList(Of Root) = CType(CType(Me.DataGridView1.DataSource, BindingSource).DataSource, Csla.SortedBindingList(Of Root))
    list.RemoveAt(3)

  End Sub

  Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

    Dim c As Child = Child.NewChild
    Dim bs As BindingSource = CType(Me.DataGridView1.DataSource, BindingSource)
    bs.Add(c)

  End Sub

End Class
