<TestFixture()> _
Public Class IO

  <Test()> _
  Public Sub LoadRoot()
    Dim root As root
    root = root.GetRoot("loaded")
    Assert.IsNotNull(root)
    Assert.AreEqual("loaded", root.Data)
    Assert.AreEqual(False, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(False, root.IsDirty)
  End Sub

End Class
