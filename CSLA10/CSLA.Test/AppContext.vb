Imports System.Threading

<TestFixture()> _
Public Class AppContext

  <Test()> _
  Public Sub NoContext()
    Session.Clear()

    ApplicationContext.Clear()

    Dim root As SimpleRoot = SimpleRoot.GetSimpleRoot("simple")

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("CSLA.ClientContext")
    Assert.IsNull(Thread.GetData(slot), "ClientContext should be Nothing")

    slot = Thread.GetNamedDataSlot("CSLA.GlobalContext")
    Assert.IsNull(Thread.GetData(slot), "GlobalContext should be Nothing")

  End Sub

  <Test()> _
  Public Sub ClientContext()
    Session.Clear()

    ApplicationContext.ClientContext.Add("clientcontext", "client context data")
    Assert.AreEqual("client context data", ApplicationContext.ClientContext.Item("clientcontext"))

    Dim root As root
    root = root.NewRoot()

    root.Data = "saved"
    Assert.AreEqual("saved", root.Data)
    Assert.AreEqual(True, root.IsDirty)
    Assert.AreEqual(True, root.IsValid)

    Session.Clear()
    root = CType(root.Save, root)

    Assert.IsNotNull(root)
    Assert.AreEqual("Inserted", Session("Root"))
    Assert.AreEqual("saved", root.Data)
    Assert.AreEqual(False, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(False, root.IsDirty)

    Assert.AreEqual("client context data", ApplicationContext.ClientContext.Item("clientcontext"))
    Assert.AreEqual("client context data", Session("clientcontext"))

  End Sub

  <Test()> _
  Public Sub GlobalContext()
    Session.Clear()

    ApplicationContext.GlobalContext.Item("globalcontext") = "global context data"
    Assert.AreEqual("global context data", ApplicationContext.GlobalContext.Item("globalcontext"), "First")

    Dim root As root
    root = root.NewRoot()

    root.Data = "saved"
    Assert.AreEqual("saved", root.Data)
    Assert.AreEqual(True, root.IsDirty)
    Assert.AreEqual(True, root.IsValid)

    Session.Clear()
    root = CType(root.Save, root)

    Assert.IsNotNull(root)
    Assert.AreEqual("Inserted", Session("Root"))
    Assert.AreEqual("saved", root.Data)
    Assert.AreEqual(False, root.IsNew)
    Assert.AreEqual(False, root.IsDeleted)
    Assert.AreEqual(False, root.IsDirty)

    Assert.AreEqual("new global value", ApplicationContext.GlobalContext.Item("globalcontext"), "Second")
    Assert.AreEqual("global context data", Session("globalcontext"), "Third")

  End Sub

  <Test()> _
  Public Sub DataPortalEvents()
    Session.Clear()

    ApplicationContext.Clear()

    ApplicationContext.GlobalContext("global") = "global"

    AddHandler DataPortal.OnDataPortalInvoke, AddressOf OnDataPortalInvoke
    AddHandler DataPortal.OnDataPortalInvokeComplete, AddressOf OnDataPortalInvokeComplete

    Dim root As root = root.GetRoot("testing")

    RemoveHandler DataPortal.OnDataPortalInvoke, AddressOf OnDataPortalInvoke
    RemoveHandler DataPortal.OnDataPortalInvokeComplete, AddressOf OnDataPortalInvokeComplete

    Assert.AreEqual("global", Session("ClientInvoke"), "Client invoke incorrect")
    Assert.AreEqual("global", Session("ClientInvokeComplete"), "Client invoke complete incorrect")
    Assert.AreEqual("global", Session("dpinvoke"), "Server invoke incorrect")
    Assert.AreEqual("global", Session("dpinvokecomplete"), "Server invoke complete incorrect")

  End Sub

  Private Sub OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

    Session("ClientInvoke") = ApplicationContext.GlobalContext("global")

  End Sub

  Private Sub OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

    Session("ClientInvokeComplete") = ApplicationContext.GlobalContext("global")

  End Sub


End Class
