<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ResourceEdit.aspx.vb" Inherits="PTWeb.ResourceEdit"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
  <HEAD>
    <title>ResourceEdit</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <H1>Edit Resource</H1>
      <P>
        <asp:HyperLink id="HyperLink1" runat="server" NavigateUrl="Default.aspx">Home</asp:HyperLink>&nbsp;&nbsp;&nbsp;
        <asp:HyperLink id="HyperLink2" runat="server" NavigateUrl="Resources.aspx">Resource list</asp:HyperLink>&nbsp;&nbsp;&nbsp;
        <asp:LinkButton id="btnNewResource" runat="server">Add new resource</asp:LinkButton></P>
      <P>
        <TABLE id="Table1" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD>ID</TD>
            <TD>
              <asp:TextBox id="txtID" runat="server" ReadOnly="True" Text="<%# mResource.ID %>">
              </asp:TextBox>&nbsp;
              <asp:RequiredFieldValidator id="RequiredFieldValidator3" runat="server" ControlToValidate="txtID" ErrorMessage="ID required"></asp:RequiredFieldValidator></TD>
          </TR>
          <TR>
            <TD>First name</TD>
            <TD>
              <asp:TextBox id="txtFirstname" runat="server" Text="<%# mResource.FirstName %>">
              </asp:TextBox>&nbsp;
              <asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ErrorMessage="First name requried" ControlToValidate="txtFirstname"></asp:RequiredFieldValidator></TD>
          </TR>
          <TR>
            <TD>Last name</TD>
            <TD>
              <asp:TextBox id="txtLastname" runat="server" Text="<%# mResource.LastName %>">
              </asp:TextBox>&nbsp;
              <asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" ErrorMessage="Last name required" ControlToValidate="txtLastname"></asp:RequiredFieldValidator></TD>
          </TR>
          <TR>
            <TD>Assignments</TD>
            <TD>
              <asp:DataGrid id="dgProjects" runat="server" AutoGenerateColumns="False" BorderColor="#3366CC"
                BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="4" EnableViewState="False">
                <SelectedItemStyle Font-Bold="True" ForeColor="#CCFF99" BackColor="#009999"></SelectedItemStyle>
                <ItemStyle ForeColor="#003399" BackColor="White"></ItemStyle>
                <HeaderStyle Font-Bold="True" ForeColor="#CCCCFF" BackColor="#003399"></HeaderStyle>
                <FooterStyle ForeColor="#003399" BackColor="#99CCCC"></FooterStyle>
                <Columns>
                  <asp:BoundColumn Visible="False" DataField="ProjectID" HeaderText="Project ID"></asp:BoundColumn>
                  <asp:BoundColumn DataField="ProjectName" HeaderText="Project name"></asp:BoundColumn>
                  <asp:BoundColumn DataField="Assigned" HeaderText="Assigned"></asp:BoundColumn>
                  <asp:ButtonColumn DataTextField="Role" HeaderText="Role" CommandName="SelectRole"></asp:ButtonColumn>
                  <asp:ButtonColumn Text="Remove" CommandName="Delete"></asp:ButtonColumn>
                  <asp:ButtonColumn Text="Details" CommandName="Select"></asp:ButtonColumn>
                </Columns>
                <PagerStyle HorizontalAlign="Left" ForeColor="#003399" BackColor="#99CCCC" Mode="NumericPages"></PagerStyle>
              </asp:DataGrid>
              <P>
                <asp:LinkButton id="btnAssign" runat="server">Assign to project</asp:LinkButton></P>
            </TD>
          </TR>
        </TABLE>
      </P>
      <P>
        <asp:Button id="btnSave" runat="server" Text="Save"></asp:Button>&nbsp;&nbsp;
        <asp:Button id="btnCancel" runat="server" Text="Cancel"></asp:Button></P>
    </form>
  </body>
</HTML>
