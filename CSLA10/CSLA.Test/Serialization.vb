<TestFixture()> _
Public Class Serialization

  <Test()> _
  Public Sub Clone()
    Session.Clear()
    Dim root As New SerializationRoot
    root = CType(root.Clone, SerializationRoot)
    Assert.AreEqual(3, Session.Count, "Incorrect number of events fired - should have been 3")
    Assert.AreEqual(True, Session("Serialized"), "Serialized not called")
    Assert.AreEqual(True, Session("Deserialized"), "Deserialized not called")
    Assert.AreEqual(True, Session("Serializing"), "Serializing not called")
  End Sub

  <Test()> _
  Public Sub SerializableEvents()
    Session.Clear()
    Dim root As New SerializationRoot
    Dim handler As New TestEventSink

    AddHandler root.IsDirtyChanged, AddressOf OnIsDirtyChanged
    AddHandler root.IsDirtyChanged, AddressOf SharedOnIsDirtyChanged
    AddHandler root.IsDirtyChanged, AddressOf PublicSharedOnIsDirtyChanged
    AddHandler root.IsDirtyChanged, AddressOf handler.OnIsDirtyChanged
    handler.Reg(root)

    root.Data = "abc"
    Assert.AreEqual("abc", root.Data, "Data value not set")

    Assert.AreEqual("OnIsDirtyChanged", Session("OnIsDirtyChanged"), "Didn't call local handler")
    Assert.AreEqual("SharedOnIsDirtyChanged", Session("SharedOnIsDirtyChanged"), "Didn't call shared handler")
    Assert.AreEqual("PublicSharedOnIsDirtyChanged", Session("PublicSharedOnIsDirtyChanged"), "Didn't call public shared handler")
    Assert.AreEqual("Test.OnIsDirtyChanged", Session("Test.OnIsDirtyChanged"), "Didn't call serializable handler")
    Assert.AreEqual("Test.PrivateOnIsDirtyChanged", Session("Test.PrivateOnIsDirtyChanged"), "Didn't call serializable Private handler")

    root = CType(root.Clone, SerializationRoot)

    Session.Clear()
    root.Data = "xyz"
    Assert.AreEqual("xyz", root.Data, "Data value not set")

    Assert.AreEqual(Nothing, Session("OnIsDirtyChanged"), "Called local handler after clone")
    Assert.AreEqual(Nothing, Session("SharedOnIsDirtyChanged"), "Called shared handler after clone")
    Assert.AreEqual("PublicSharedOnIsDirtyChanged", Session("PublicSharedOnIsDirtyChanged"), "Didn't call public shared handler after clone")
    Assert.AreEqual("Test.OnIsDirtyChanged", Session("Test.OnIsDirtyChanged"), "Didn't call serializable handler after clone")
    Assert.AreEqual(Nothing, Session("Test.PrivateOnIsDirtyChanged"), "Called serializable Private handler after clone")

  End Sub

  Private Sub OnIsDirtyChanged(ByVal sender As Object, ByVal e As EventArgs)
    Session("OnIsDirtyChanged") = "OnIsDirtyChanged"
  End Sub

  Private Shared Sub SharedOnIsDirtyChanged(ByVal sender As Object, ByVal e As EventArgs)
    Session("SharedOnIsDirtyChanged") = "SharedOnIsDirtyChanged"
  End Sub

  Public Shared Sub PublicSharedOnIsDirtyChanged(ByVal sender As Object, ByVal e As EventArgs)
    Session("PublicSharedOnIsDirtyChanged") = "PublicSharedOnIsDirtyChanged"
  End Sub

End Class

<Serializable()> _
Public Class TestEventSink

  Public Sub Reg(ByVal obj As BusinessBase)
    AddHandler obj.IsDirtyChanged, AddressOf PrivateOnIsDirtyChanged
  End Sub

  Private Sub PrivateOnIsDirtyChanged(ByVal sender As Object, ByVal e As EventArgs)
    Session("Test.PrivateOnIsDirtyChanged") = "Test.PrivateOnIsDirtyChanged"
  End Sub

  Public Sub OnIsDirtyChanged(ByVal sender As Object, ByVal e As EventArgs)
    Session("Test.OnIsDirtyChanged") = "Test.OnIsDirtyChanged"
  End Sub

End Class


<Serializable()> _
Public Class SerializationRoot
  Inherits BusinessBase

  Private mData As String = ""

  Public Property Data() As String
    Get
      Return mData
    End Get
    Set(ByVal Value As String)
      If mData <> Value Then
        mData = Value
        MarkDirty()
      End If
    End Set
  End Property

  Protected Overrides Sub Serialized()
    Session.Add("Serialized", True)
  End Sub

  Protected Overrides Sub Serializing()
    Session.Add("Serializing", True)
  End Sub

  Protected Overrides Sub Deserialized()
    Session.Add("Deserialized", True)
  End Sub

End Class