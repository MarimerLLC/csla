<%@ Page language="c#" Codebehind="AssignToProject.aspx.cs" AutoEventWireup="false" Inherits="PTWebcs.AssignToProject" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>AssignToProject</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body MS_POSITIONING="FlowLayout">
    <FORM id="Form1" method="post" runat="server">
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
    </FORM>
  </body>
</HTML>
