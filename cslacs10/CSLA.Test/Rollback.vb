<TestFixture()> _
Public Class Rollback

  <Test()> _
  Public Sub NoFail()
    Session.Clear()
    Dim root As RollbackRoot
    root = root.NewRoot()

    root.BeginEdit()
    root.Data = "saved"
    Assert.AreEqual("saved", root.Data, "data is 'saved'")
    Assert.AreEqual(False, root.Fail, "fail is false")
    Assert.AreEqual(True, root.IsDirty, "isdirty is true")
    Assert.AreEqual(True, root.IsValid, "isvalid is true")
    Assert.AreEqual(True, root.IsNew, "isnew is true")

    Session.Clear()
    Dim tmp As RollbackRoot = DirectCast(root.Clone, RollbackRoot)
    Try
      root.ApplyEdit()
      root = DirectCast(root.Save, RollbackRoot)

    Catch
      root = tmp
      Assert.Fail("exception occurred")
    End Try

    Assert.IsNotNull(root, "obj is not null")
    Assert.AreEqual("Inserted", Session("Root"), "obj was inserted")
    Assert.AreEqual("saved", root.Data, "data is 'saved'")
    Assert.AreEqual(False, root.IsNew, "isnew is false")
    Assert.AreEqual(False, root.IsDeleted, "isdeleted is false")
    Assert.AreEqual(False, root.IsDirty, "isdirty is false")

  End Sub

  <Test()> _
  Public Sub YesFail()
    Session.Clear()
    Dim root As RollbackRoot
    root = root.NewRoot()

    root.BeginEdit()
    root.Data = "saved"
    root.Fail = True
    Assert.AreEqual("saved", root.Data, "data is 'saved'")
    Assert.AreEqual(True, root.Fail, "fail is true")
    Assert.AreEqual(True, root.IsDirty, "isdirty is true")
    Assert.AreEqual(True, root.IsValid, "isvalid is true")
    Assert.AreEqual(True, root.IsNew, "isnew is true")

    Session.Clear()
    Dim tmp As RollbackRoot = DirectCast(root.Clone, RollbackRoot)
    Try
      root.ApplyEdit()
      root = DirectCast(root.Save, RollbackRoot)
      Assert.Fail("exception didn't occur")

    Catch
      root = tmp
    End Try

    Assert.IsNotNull(root, "obj is not null")
    Assert.AreEqual("Inserted", Session("Root"), "obj was inserted")
    Assert.AreEqual("saved", root.Data, "data is 'saved'")
    Assert.AreEqual(True, root.IsNew, "isnew is true")
    Assert.AreEqual(False, root.IsDeleted, "isdeleted is false")
    Assert.AreEqual(True, root.IsDirty, "isdirty is true")

  End Sub

  <Test()> _
  Public Sub YesFailCancel()
    Session.Clear()
    Dim root As RollbackRoot
    root = root.NewRoot()
    Assert.AreEqual(True, root.IsDirty, "isdirty is true")
    Assert.AreEqual("<new>", root.Data, "data is '<new>'")

    root.BeginEdit()
    root.Data = "saved"
    root.Fail = True
    Assert.AreEqual("saved", root.Data, "data is 'saved'")
    Assert.AreEqual(True, root.Fail, "fail is true")
    Assert.AreEqual(True, root.IsDirty, "isdirty is true")
    Assert.AreEqual(True, root.IsValid, "isvalid is true")
    Assert.AreEqual(True, root.IsNew, "isnew is true")

    Session.Clear()
    Dim tmp As RollbackRoot = DirectCast(root.Clone, RollbackRoot)
    Try
      root.ApplyEdit()
      root = DirectCast(root.Save, RollbackRoot)
      Assert.Fail("exception didn't occur")

    Catch
      root = tmp
      root.CancelEdit()
    End Try

    Assert.IsNotNull(root, "obj is not null")
    Assert.AreEqual("Inserted", Session("Root"), "obj was inserted")
    Assert.AreEqual("<new>", root.Data, "data is '<new>'")
    Assert.AreEqual(True, root.IsNew, "isnew is true")
    Assert.AreEqual(False, root.IsDeleted, "isdeleted is false")
    Assert.AreEqual(True, root.IsDirty, "isdirty is true")

  End Sub

End Class
