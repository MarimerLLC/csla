<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Login.aspx.vb" Inherits="PTWeb.Login"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
  <HEAD>
    <title>Login</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <H1>Project Tracker Login</H1>
      <P>
        <TABLE id="Table1" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD>Username</TD>
            <TD>
              <asp:TextBox id="txtUsername" runat="server" Width="152px"></asp:TextBox>&nbsp;
              <asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ErrorMessage="Username required" ControlToValidate="txtUsername"></asp:RequiredFieldValidator></TD>
          </TR>
          <TR>
            <TD>Password</TD>
            <TD>
              <asp:TextBox id="txtPassword" runat="server" TextMode="Password" Width="152px"></asp:TextBox>&nbsp;
              <asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" ErrorMessage="Password required" ControlToValidate="txtPassword"></asp:RequiredFieldValidator></TD>
          </TR>
        </TABLE>
      </P>
      <P>
        <asp:Button id="btnLogin" runat="server" Text="Login"></asp:Button></P>
    </form>
  </body>
</HTML>
