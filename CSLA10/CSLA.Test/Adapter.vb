<TestFixture()> _
Public Class Adapter

  <Test()> _
  Public Sub FillDataTable()
    Dim obj As Root = Root.NewRoot
    obj.Children.Add("A")
    obj.Children.Add("B")
    obj.Children.Add("C")

    Dim da As New CSLA.Data.ObjectAdapter()

    Dim ds As New DataSet()
    da.Fill(ds, obj.Children)

    Assert.AreEqual("Data", ds.Tables(0).Columns("Data").Caption, "Column caption incorrect")
    Assert.AreEqual("Guid", ds.Tables(0).Columns("Guid").Caption, "Column caption incorrect")
    Assert.AreEqual(3, ds.Tables(0).Rows.Count, "Wrong number of rows")
    Assert.AreEqual("A", ds.Tables(0).Rows(0).Item("Data"), "Child A missing")
    Assert.AreEqual("B", ds.Tables(0).Rows(1).Item("Data"), "Child B missing")
    Assert.AreEqual("C", ds.Tables(0).Rows(2).Item("Data"), "Child C missing")
    Assert.IsTrue(ds.Tables(0).Rows(0).Item("Guid").ToString.Length > 0, "Value missing")
    Assert.IsTrue(ds.Tables(0).Rows(1).Item("Guid").ToString.Length > 0, "Value missing")
    Assert.IsTrue(ds.Tables(0).Rows(2).Item("Guid").ToString.Length > 0, "Value missing")

  End Sub

End Class
