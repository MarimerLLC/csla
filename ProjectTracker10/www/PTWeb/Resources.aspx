<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Resources.aspx.vb" Inherits="PTWeb.Resources" enableViewState="False"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
  <HEAD>
    <title>Resources</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <H1>Resources</H1>
      <P>
        <asp:HyperLink id="HyperLink1" runat="server" NavigateUrl="Default.aspx">Home</asp:HyperLink>&nbsp;&nbsp;&nbsp;
        <asp:LinkButton id="btnNewResource" runat="server">Add new resource</asp:LinkButton></P>
      <P>
        <asp:DataGrid id="dgResources" runat="server" AutoGenerateColumns="False" BorderColor="#3366CC"
          BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="4">
          <SelectedItemStyle Font-Bold="True" ForeColor="#CCFF99" BackColor="#009999"></SelectedItemStyle>
          <ItemStyle ForeColor="#003399" BackColor="White"></ItemStyle>
          <HeaderStyle Font-Bold="True" ForeColor="#CCCCFF" BackColor="#003399"></HeaderStyle>
          <FooterStyle ForeColor="#003399" BackColor="#99CCCC"></FooterStyle>
          <Columns>
            <asp:BoundColumn Visible="False" DataField="ID" HeaderText="Resource ID"></asp:BoundColumn>
            <asp:ButtonColumn DataTextField="Name" HeaderText="Resource name" CommandName="Select"></asp:ButtonColumn>
            <asp:ButtonColumn Text="Remove" CommandName="Delete"></asp:ButtonColumn>
          </Columns>
          <PagerStyle HorizontalAlign="Left" ForeColor="#003399" BackColor="#99CCCC" Mode="NumericPages"></PagerStyle>
        </asp:DataGrid></P>
    </form>
  </body>
</HTML>
