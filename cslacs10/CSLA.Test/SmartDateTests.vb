<TestFixture()> _
Public Class SmartDateTests

  <Test()> _
  Public Sub Add()
    Dim d2 As New SmartDate
    Dim d3 As SmartDate

    d2.Date = #1/1/2005#
    d3 = SmartDate.op_Addition(d2, New TimeSpan(30, 0, 0, 0))
    Assert.AreEqual(DateAdd(DateInterval.Day, 30, d2.Date), d3.Date, "Dates should be equal")

  End Sub

  <Test()> _
  Public Sub Subtract()
    Dim d2 As New SmartDate
    Dim d3 As SmartDate

    d2.Date = #1/1/2005#
    d3 = SmartDate.op_Subtraction(d2, New TimeSpan(30, 0, 0, 0))
    Assert.AreEqual(DateAdd(DateInterval.Day, -30, d2.Date), d3.Date, "Dates should be equal")

  End Sub

  <Test()> _
  Public Sub Comparison()
    Dim d2 As New SmartDate(True)
    Dim d3 As New SmartDate(False)

    Assert.IsTrue(d2.Equals(d3), "Empty dates should be equal")
    Assert.IsTrue(SmartDate.op_Equality(d2, d3), "Empty dates should be equal")

    d2.Date = #1/1/2005#
    d3 = New SmartDate(d2.Date, d2.EmptyIsMin)
    Assert.AreEqual(d2, d3, "Assigned dates should be equal")

    d3.Date = #2/2/2005#
    Assert.IsTrue(SmartDate.op_GreaterThan(d3, d2), "Should be greater than")
    Assert.IsTrue(SmartDate.op_LessThan(d2, d3), "Should be less than")
    Assert.IsTrue(SmartDate.op_Inequality(d2, d3), "Should not be equal")

  End Sub

  <Test()> _
  Public Sub Empty()
    Dim d2 As New SmartDate
    Dim d3 As SmartDate

    d3 = SmartDate.op_Addition(d2, New TimeSpan(30, 0, 0, 0))
    Assert.AreEqual(d2, d3, "Dates should be equal")

    d3 = SmartDate.op_Subtraction(d2, New TimeSpan(30, 0, 0, 0))
    Assert.AreEqual(d2, d3, "Dates should be equal")

  End Sub

End Class
