<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ChooseRole.aspx.vb" Inherits="PTWeb.ChooseRole"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
  <HEAD>
    <title>ChooseRole</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <H1>Choose Role</H1>
      <P>
        <TABLE id="Table1" cellSpacing="1" cellPadding="1" width="300" border="0">
          <TR>
            <TD>
              <asp:Label id="lblLabel" runat="server">Label</asp:Label></TD>
            <TD>
              <asp:Label id="lblValue" runat="server">Label</asp:Label></TD>
          </TR>
          <TR>
            <TD>Roles</TD>
            <TD>
              <asp:ListBox id="lstRoles" runat="server" Height="160px" Rows="10"></asp:ListBox></TD>
          </TR>
        </TABLE>
      </P>
      <P>
        <asp:Button id="btnUpdate" runat="server" Text="Update role"></asp:Button>&nbsp;&nbsp;&nbsp;
        <asp:Button id="btnCancel" runat="server" Text="Cancel"></asp:Button></P>
    </form>
  </body>
</HTML>
