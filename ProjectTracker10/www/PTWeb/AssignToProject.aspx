<%@ Page Language="vb" AutoEventWireup="false" Codebehind="AssignToProject.aspx.vb" Inherits="PTWeb.AssignToProject"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
  <HEAD>
    <title>AssignToProject</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <H1>Assign to Project</H1>
      <P>Indicate the role the resource will play on the project</P>
      <P>
        <asp:ListBox id="lstRoles" runat="server" Height="128px"></asp:ListBox></P>
      <P>Choose the project</P>
      <P>
        <asp:DataGrid id="dgProjects" runat="server" AutoGenerateColumns="False" EnableViewState="False">
          <Columns>
            <asp:BoundColumn Visible="False" DataField="ID" HeaderText="Project id"></asp:BoundColumn>
            <asp:ButtonColumn DataTextField="Name" HeaderText="Project name" CommandName="Select"></asp:ButtonColumn>
          </Columns>
        </asp:DataGrid></P>
      <P>
        <asp:Button id="btnCancel" runat="server" Text="Cancel"></asp:Button></P>
    </form>
  </body>
</HTML>
