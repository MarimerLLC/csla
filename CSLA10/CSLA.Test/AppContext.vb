<TestFixture()> _
Public Class AppContext

  <Test()> _
  Public Sub CreateGetRoot()
    Session.Clear()

    ApplicationContext.Current.Add("context1", "my value")
    Assert.AreEqual("my value", ApplicationContext.Current.Item("context1"))

    Dim root As root
    root = root.NewRoot()

    root.Data = "saved"
    Assert.AreEqual("saved", root.Data)
    Assert.AreEqual(True, root.IsDirty)
    Assert.AreEqual(True, root.IsValid)

    Session.Clear()
    root = CType(root.Save, root)

    Assert.IsNotNull(root)
    Assert.AreEqual("Inserted", Session("Root"))
    Assert.AreEqual("saved", root.Data)
    Assert.AreEqual(False, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(False, root.IsDirty)

    Assert.AreEqual("my value", ApplicationContext.Current.Item("context1"))
    Assert.AreEqual("my value", Session("context1"))

  End Sub

End Class
