<TestFixture()> _
Public Class Basics

  <Test()> _
  Public Sub CreateRoot()
    Dim root As root
    root = root.NewRoot
    Assert.IsNotNull(root)
    Assert.AreEqual("<new>", root.Data)
    Assert.AreEqual(True, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(True, root.IsDirty)
  End Sub

  <Test()> _
  Public Sub AddChild()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    Assert.AreEqual(1, root.Children.Count)
    Assert.AreEqual("1", root.Children(0).Data)
  End Sub

  <Test()> _
  Public Sub AddRemoveChild()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    root.Children.Remove(root.Children.Item(0))
    Assert.AreEqual(0, root.Children.Count)
  End Sub

  <Test()> _
  Public Sub AddGrandChild()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    Dim child As child = root.Children(0)
    child.GrandChildren.Add("1")
    Assert.AreEqual(1, child.GrandChildren.Count)
    Assert.AreEqual("1", child.GrandChildren(0).Data)
  End Sub

  <Test()> _
  Public Sub AddRemoveGrandChild()
    Dim root As root = root.NewRoot
    root.Children.Add("1")
    Dim child As child = root.Children(0)
    child.GrandChildren.Add("1")
    child.GrandChildren.Remove(child.GrandChildren.Item(0))
    Assert.AreEqual(0, child.GrandChildren.Count)
  End Sub

End Class
