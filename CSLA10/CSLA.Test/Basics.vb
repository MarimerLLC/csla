<TestFixture()> _
Public Class Basics

  <Test()> _
  Public Sub CreateGetRoot()
    Session.Clear()
    Dim root As GenRoot
    root = GenRoot.NewRoot
    Assert.IsNotNull(root)
    Assert.AreEqual("<new>", root.Data)
    Assert.AreEqual("Created", Session("GenRoot"))
    Assert.AreEqual(True, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(True, root.IsDirty)
  End Sub

  <Test()> _
  Public Sub CreateRoot()
    Session.Clear()
    Dim root As root
    root = root.NewRoot
    Assert.IsNotNull(root)
    Assert.AreEqual("<new>", root.Data)
    Assert.AreEqual("Created", Session("Root"))
    Assert.AreEqual(True, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(True, root.IsDirty)
  End Sub

  <Test()> _
  Public Sub AddChild()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    Assert.AreEqual(1, root.Children.Count)
    Assert.AreEqual("1", root.Children(0).Data)
  End Sub

  <Test()> _
  Public Sub AddRemoveChild()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    root.Children.Remove(root.Children.Item(0))
    Assert.AreEqual(0, root.Children.Count)
  End Sub

  <Test()> _
  Public Sub AddRemoveAddChild()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    root.BeginEdit()
    root.Children.Remove(root.Children.Item(0))
    root.Children.Add("2")
    root.CancelEdit()
    Assert.AreEqual(1, root.Children.Count)
    Assert.AreEqual("1", root.Children(0).Data)
  End Sub

  <Test()> _
  Public Sub AddGrandChild()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    Dim child As child = root.Children(0)
    child.GrandChildren.Add("1")
    Assert.AreEqual(1, child.GrandChildren.Count)
    Assert.AreEqual("1", child.GrandChildren(0).Data)
  End Sub

  <Test()> _
  Public Sub AddRemoveGrandChild()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    Dim child As child = root.Children(0)
    child.GrandChildren.Add("1")
    child.GrandChildren.Remove(child.GrandChildren.Item(0))
    Assert.AreEqual(0, child.GrandChildren.Count)
  End Sub

  <Test()> _
  Public Sub CloneGraph()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    Dim child As child = root.Children(0)
    child.GrandChildren.Add("1")
    Assert.AreEqual(1, child.GrandChildren.Count)
    Assert.AreEqual("1", child.GrandChildren(0).Data)

    Dim clone As root = DirectCast(root.Clone, root)
    child = clone.Children(0)
    Assert.AreEqual(1, child.GrandChildren.Count)
    Assert.AreEqual("1", child.GrandChildren(0).Data)

    Assert.AreEqual("root Deserialized", CStr(Session("Deserialized")))
    Assert.AreEqual("root Serialized", CStr(Session("Serialized")))
    Assert.AreEqual("root Serializing", CStr(Session("Serializing")))

    Assert.AreEqual("GC Deserialized", CStr(Session("GCDeserialized")))
    Assert.AreEqual("GC Serialized", CStr(Session("GCSerialized")))
    Assert.AreEqual("GC Serializing", CStr(Session("GCSerializing")))

  End Sub

  <Test()> _
  Public Sub ClearChildList()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.Children.Add("A")
    root.Children.Add("B")
    root.Children.Add("C")
    root.Children.Clear()
    Assert.AreEqual(0, root.Children.Count)
  End Sub

  <Test()> _
  Public Sub NestedAddAcceptChild()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.BeginEdit()
    root.Children.Add("A")
    root.BeginEdit()
    root.Children.Add("B")
    root.BeginEdit()
    root.Children.Add("C")
    root.ApplyEdit()
    root.ApplyEdit()
    root.ApplyEdit()
    Assert.AreEqual(3, root.Children.Count)
  End Sub

  <Test()> _
  Public Sub NestedAddDeleteAcceptChild()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.BeginEdit()
    root.Children.Add("A")
    root.BeginEdit()
    root.Children.Add("B")
    root.BeginEdit()
    root.Children.Add("C")
    Dim childC As Child = root.Children.Item(2)
    root.Children.Remove(root.Children.Item(0))
    root.Children.Remove(root.Children.Item(0))
    root.Children.Remove(root.Children.Item(0))
    root.ApplyEdit()
    root.ApplyEdit()
    root.ApplyEdit()
    Assert.AreEqual(0, root.Children.Count)
    Assert.AreEqual(False, root.Children.ContainsDeleted(childC))
  End Sub

End Class
