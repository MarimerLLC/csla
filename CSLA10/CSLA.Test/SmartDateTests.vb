<TestFixture()> _
Public Class SmartDateTests

  <Test()> _
  Public Sub Add()
    Dim d2 As New SmartDate
    Dim d3 As SmartDate

    d2.Date = #1/1/2005#
    d3 = New SmartDate(d2.Add(New TimeSpan(30, 0, 0, 0)))
    Assert.AreEqual(DateAdd(DateInterval.Day, 30, d2.Date), d3.Date, "Dates should be equal")

  End Sub

  <Test()> _
  Public Sub Subtract()
    Dim d2 As New SmartDate
    Dim d3 As SmartDate

    d2.Date = #1/1/2005#
    d3 = New SmartDate(d2.Subtract(New TimeSpan(30, 0, 0, 0)))
    Assert.AreEqual(DateAdd(DateInterval.Day, -30, d2.Date), d3.Date, "Dates should be equal")

  End Sub

  <Test()> _
  Public Sub Comparison()
    Dim d2 As New SmartDate(True)
    Dim d3 As New SmartDate(False)

    Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal")
    Assert.IsTrue(SmartDate.Equals(d2, d3), "Empty dates should be equal (shared)")
    Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal (unary)")

    d2.Date = #1/1/2005#
    d3 = New SmartDate(d2.Date, d2.EmptyIsMin)
    Assert.AreEqual(d2, d3, "Assigned dates should be equal")

    d3.Date = #2/2/2005#
    Assert.AreEqual(1, d3.CompareTo(d2), "Should be greater than")
    Assert.AreEqual(-1, d2.CompareTo(d3), "Should be less than")
    Assert.IsFalse(d2.CompareTo(d3) = 0, "Should not be equal")

    Assert.IsTrue(d3.Equals("2/2/2005"), "Should be equal to string date")
    Assert.IsTrue(d3.Equals(#2/2/2005#), "Should be equal to DateTime")

  End Sub

  <Test()> _
  Public Sub Empty()
    Dim d2 As New SmartDate
    Dim d3 As SmartDate

    d3 = New SmartDate(d2.Add(New TimeSpan(30, 0, 0, 0)))
    Assert.AreEqual(d2, d3, "Dates should be equal")

    d3 = New SmartDate(d2.Subtract(New TimeSpan(30, 0, 0, 0)))
    Assert.AreEqual(d2, d3, "Dates should be equal")

  End Sub

End Class
