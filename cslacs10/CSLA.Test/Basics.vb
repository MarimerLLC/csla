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
  Public Sub AddCancelMultipleChild()
    Session.Clear()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    Session.Clear()
    root = root.Save()
    Assert.AreEqual(1, root.Children.Count)
    Assert.AreEqual("1", root.Children(0).Data)
    Assert.AreEqual(False, root.IsNew, "Object should not be new")
    Assert.AreEqual(False, root.IsDirty, "Object should not be dirty")
    Assert.AreEqual(False, root.IsDeleted, "Object should not be marked for deletion")

    Session.Clear()
    root.BeginEdit()
    root.Children.Add("2")
    Assert.AreEqual(2, root.Children.Count, "Should have 2 children after add")
    root.BeginEdit()
    root.Children.Add("3")
    Assert.AreEqual(3, root.Children.Count, "Should have 3 children after add")
    root.CancelEdit()
    Assert.AreEqual(2, root.Children.Count, "Should have 2 children after cancel")
    root.CancelEdit()
    Assert.AreEqual(1, root.Children.Count, "Should have 1 child after cancel")
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
    Dim form As New FormSimulator(root)
    Dim listener As New SerializableListener(root)
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

  <Test()> _
  Public Sub BasicEquality()

    Session.Clear()
    Dim r1 As Root = Root.NewRoot
    r1.Data = "abc"
    Assert.AreEqual(True, r1.Equals(r1), "Objects should be equal on instance compare")
    Assert.AreEqual(True, Equals(r1, r1), "Objects should be equal on static compare")

    Session.Clear()
    Dim r2 As Root = Root.NewRoot
    r2.Data = "xyz"
    Assert.AreEqual(False, r1.Equals(r2), "Objects should not be equal")
    Assert.AreEqual(False, Equals(r1, r2), "Objects should not be equal")

    Assert.AreEqual(False, r1.Equals(Nothing), "Objects should not be equal")
    Assert.AreEqual(False, Equals(r1, Nothing), "Objects should not be equal")
    Assert.AreEqual(False, Equals(Nothing, r2), "Objects should not be equal")

  End Sub

  <Test()> _
  Public Sub ChildEquality()

    Session.Clear()
    Dim root As root = root.NewRoot
    root.Children.Add("abc")
    root.Children.Add("xyz")
    root.Children.Add("123")
    Dim c1 As Child = root.Children(0)
    Dim c2 As Child = root.Children(1)
    Dim c3 As Child = root.Children(2)
    root.Children.Remove(c3)

    Assert.AreEqual(True, c1.Equals(c1), "Objects should be equal")
    Assert.AreEqual(True, Equals(c1, c1), "Objects should be equal")

    Assert.AreEqual(False, c1.Equals(c2), "Objects should not be equal")
    Assert.AreEqual(False, Equals(c1, c2), "Objects should not be equal")

    Assert.AreEqual(False, c1.Equals(Nothing), "Objects should not be equal")
    Assert.AreEqual(False, Equals(c1, Nothing), "Objects should not be equal")
    Assert.AreEqual(False, Equals(Nothing, c2), "Objects should not be equal")

    Assert.AreEqual(True, root.Children.Contains(c1), "Collection should contain c1")
    Assert.AreEqual(True, root.Children.Contains(c2), "Collection should contain c2")
    Assert.AreEqual(False, root.Children.Contains(c3), "Collection should not contain c3")
    Assert.AreEqual(True, root.Children.ContainsDeleted(c3), "Deleted collection should contain c3")

  End Sub

End Class

Public Class FormSimulator

  Private WithEvents obj As BusinessBase

  Public Sub New(ByVal obj As BusinessBase)
    Me.obj = obj
  End Sub

  Private Sub obj_IsDirtyChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles obj.IsDirtyChanged

  End Sub
End Class

<Serializable()> _
Public Class SerializableListener

  Private WithEvents obj As BusinessBase

  Public Sub New(ByVal obj As BusinessBase)
    Me.obj = obj
  End Sub

  Public Sub obj_IsDirtyChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles obj.IsDirtyChanged

  End Sub
End Class