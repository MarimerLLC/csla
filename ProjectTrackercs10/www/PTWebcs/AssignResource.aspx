<%@ Page language="c#" Codebehind="AssignResource.aspx.cs" AutoEventWireup="false" Inherits="PTWebcs.AssignResource" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>AssignResource</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body MS_POSITIONING="FlowLayout">
    <FORM id="Form1" method="post" runat="server">
      <H1>Assign Resource</H1>
      <P>Choose a role for the resource</P>
      <P>
        <asp:ListBox id="lstRoles" runat="server" Height="128px"></asp:ListBox></P>
      <P>Assign a resource to assign to the project</P>
      <P>
        <asp:DataGrid id="dgResources" runat="server" AutoGenerateColumns="False" EnableViewState="False">
          <Columns>
            <asp:BoundColumn Visible="False" DataField="ID" HeaderText="Resource ID"></asp:BoundColumn>
            <asp:ButtonColumn DataTextField="Name" HeaderText="Resource name" CommandName="Select"></asp:ButtonColumn>
          </Columns>
        </asp:DataGrid></P>
      <P>
        <asp:Button id="btnCancel" runat="server" Text="Cancel"></asp:Button></P>
    </FORM>
  </body>
</HTML>
