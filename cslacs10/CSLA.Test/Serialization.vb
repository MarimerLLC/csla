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

End Class

<Serializable()> _
Public Class SerializationRoot
  Inherits BusinessBase

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