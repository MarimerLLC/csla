<TestFixture()> _
Public Class IO

  <Test()> _
  Public Sub SaveNewRoot()
    Session.Clear()
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
  End Sub

  <Test()> _
  Public Sub SaveOldRoot()
    Session.Clear()
    Dim root As root
    root = root.GetRoot("old")

    root.Data = "saved"
    Assert.AreEqual("saved", root.Data)
    Assert.AreEqual(True, root.IsDirty, "IsDirty")
    Assert.AreEqual(True, root.IsValid, "IsValid")

    Session.Clear()
    root = CType(root.Save, root)

    Assert.IsNotNull(root)
    Assert.AreEqual("Updated", Session("Root"))
    Assert.AreEqual("saved", root.Data)
    Assert.AreEqual(False, root.IsNew, "IsNew")
    Assert.AreEqual(False, root.IsDeleted, "IsDeleted")
    Assert.AreEqual(False, root.IsDirty, "IsDirty")
  End Sub

  <Test()> _
  Public Sub LoadRoot()
    Session.Clear()
    Dim root As root
    root = root.GetRoot("loaded")
    Assert.IsNotNull(root)
    Assert.AreEqual("Fetched", Session("Root"))
    Assert.AreEqual("loaded", root.Data)
    Assert.AreEqual(False, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(False, root.IsDirty)
    Assert.AreEqual(True, root.IsValid)
  End Sub

  <Test()> _
  Public Sub DeleteNewRoot()
    Session.Clear()
    Dim root As root
    root = root.NewRoot

    Session.Clear()
    root.Delete()
    Assert.AreEqual(True, root.IsNew)
    Assert.AreEqual(True, root.IsDeleted)
    Assert.AreEqual(True, root.IsDirty)

    root = CType(root.Save, root)
    Assert.IsNotNull(root)
    Assert.AreEqual("Deleted", Session("Root"))
    Assert.AreEqual(True, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(True, root.IsDirty)
  End Sub

  <Test()> _
  Public Sub DeleteOldRoot()
    Session.Clear()
    Dim root As root
    root = root.GetRoot("old")

    Session.Clear()
    root.Delete()
    Assert.AreEqual(False, root.IsNew)
    Assert.AreEqual(True, root.IsDeleted)
    Assert.AreEqual(True, root.IsDirty)

    root = CType(root.Save, root)
    Assert.IsNotNull(root)
    Assert.AreEqual("Deleted", Session("Root"))
    Assert.AreEqual(True, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(True, root.IsDirty)
  End Sub

  <Test()> _
  Public Sub DeleteRootImmediate()
    Session.Clear()
    Root.DeleteRoot("test")
    Assert.AreEqual("Deleted", Session("Root"))
  End Sub

End Class
