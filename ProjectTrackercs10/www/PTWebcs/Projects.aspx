<%@ Page language="c#" Codebehind="Projects.aspx.cs" AutoEventWireup="false" Inherits="PTWebcs.Projects" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>Projects</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body MS_POSITIONING="FlowLayout">
    <FORM id="Form1" method="post" runat="server">
      <H1>Projects</H1>
      <P>
        <asp:HyperLink id="HyperLink2" runat="server" NavigateUrl="Default.aspx">Home</asp:HyperLink>&nbsp;&nbsp;&nbsp;
        <asp:LinkButton id="btnNewProject" runat="server">Add new project</asp:LinkButton></P>
      <P>
        <asp:DataGrid id="dgProjects" runat="server" AutoGenerateColumns="False" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="4" EnableViewState="False">
          <SelectedItemStyle Font-Bold="True" ForeColor="#CCFF99" BackColor="#009999"></SelectedItemStyle>
          <ItemStyle ForeColor="#003399" BackColor="White"></ItemStyle>
          <HeaderStyle Font-Bold="True" Wrap="False" ForeColor="#CCCCFF" BackColor="#003399"></HeaderStyle>
          <FooterStyle ForeColor="#003399" BackColor="#99CCCC"></FooterStyle>
          <Columns>
            <asp:BoundColumn Visible="False" DataField="ID" HeaderText="Project ID"></asp:BoundColumn>
            <asp:ButtonColumn DataTextField="Name" HeaderText="Project Name" CommandName="Select"></asp:ButtonColumn>
            <asp:ButtonColumn Text="Remove" CommandName="Delete"></asp:ButtonColumn>
          </Columns>
          <PagerStyle HorizontalAlign="Left" ForeColor="#003399" BackColor="#99CCCC" Mode="NumericPages"></PagerStyle>
        </asp:DataGrid></P>
    </FORM>
  </body>
</HTML>
