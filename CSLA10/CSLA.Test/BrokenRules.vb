<TestFixture()> _
Public Class BrokenRules

  <Test()> _
  Public Sub BreakARule()
    Session.Clear()
    Dim root As HasRules = HasRules.NewHasRules
    Assert.AreEqual(root.IsValid, False)
    Assert.AreEqual(root.GetBrokenRulesCollection.Count, 1)
  End Sub

End Class
