<TestFixture()> _
Public Class RulesManager

  <Test()> _
  Public Sub BreakRequiredRule()
    Session.Clear()
    Dim root As HasRulesManager = HasRulesManager.NewHasRulesManager
    Assert.AreEqual(False, root.IsValid, "Should not be valid")
    Assert.AreEqual(1, root.GetBrokenRulesCollection.Count)
    Assert.AreEqual("Name required", root.GetBrokenRulesCollection.Item(0).Description)
  End Sub

  <Test()> _
  Public Sub BreakLengthRule()
    Session.Clear()
    Dim root As HasRulesManager = HasRulesManager.NewHasRulesManager
    root.Name = "12345678901"
    Assert.AreEqual(False, root.IsValid, "Should not be valid")
    Assert.AreEqual(1, root.GetBrokenRulesCollection.Count)
    Assert.AreEqual("Name too long", root.GetBrokenRulesCollection.Item(0).Description)
  End Sub

  <Test()> _
  Public Sub UnBreakLengthRule()
    Session.Clear()
    Dim root As HasRulesManager = HasRulesManager.NewHasRulesManager
    root.Name = "12345678901"
    Assert.AreEqual(False, root.IsValid, "Should not be valid")
    Assert.AreEqual(1, root.GetBrokenRulesCollection.Count)
    Assert.AreEqual("Name too long", root.GetBrokenRulesCollection.Item(0).Description)

    root.Name = "1234567890"
    Assert.AreEqual(True, root.IsValid, "Should be valid")
    Assert.AreEqual(0, root.GetBrokenRulesCollection.Count)
  End Sub

End Class
