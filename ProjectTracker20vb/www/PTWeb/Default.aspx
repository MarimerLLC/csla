<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Project Tracker</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <h1>Project Tracker</h1>
      <br />
      Logged in as
      <asp:LoginName ID="LoginName1" runat="server" />
      &nbsp; &nbsp; &nbsp; &nbsp;
      <asp:LoginStatus ID="LoginStatus1" runat="server" />
      <br />
      <br />
      <br />
      <asp:LinkButton ID="ProjectsButton" runat="server">Projects</asp:LinkButton><br />
      <br />
      <asp:LinkButton ID="ResourcesButton" runat="server">Resources</asp:LinkButton><br />
      <br />
      <br />
      <asp:LinkButton ID="EditRolesButton" runat="server">Edit roles</asp:LinkButton><br />
      <br />
    </div>
    </form>
</body>
</html>
