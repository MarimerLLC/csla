<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Default.aspx.vb" Inherits="PTWeb._Default" enableViewState="False"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
  <HEAD>
    <title>Default</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <H1>Project Tracker</H1>
      <P>Welcome
        <asp:Label id="lblName" runat="server">User</asp:Label>.</P>
      <P>
        <asp:HyperLink id="HyperLink1" runat="server" NavigateUrl="Projects.aspx">Work with Projects</asp:HyperLink></P>
      <P>
        <asp:HyperLink id="HyperLink2" runat="server" NavigateUrl="Resources.aspx">Work with Resources</asp:HyperLink></P>
    </form>
  </body>
</HTML>
