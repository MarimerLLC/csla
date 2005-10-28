<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RolesEdit.aspx.vb" Inherits="RolesEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Edit Roles</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <h1>Edit Roles</h1>
      <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="MainView" runat="server">
      <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"
        DataSourceID="RolesDataSource" ForeColor="#333333" GridLines="None" DataKeyNames="Id">
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False" />
          <asp:BoundField DataField="Name" HeaderText="Role" SortExpression="Name" />
          <asp:CommandField ShowEditButton="True" />
          <asp:CommandField ShowDeleteButton="True" />
        </Columns>
        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
      </asp:GridView>
          <asp:LinkButton ID="AddRoleButton" runat="server">Add role</asp:LinkButton><br />
        </asp:View>
        <asp:View ID="InsertView" runat="server">
          <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4"
            DataSourceID="RolesDataSource" DefaultMode="Insert" ForeColor="#333333" GridLines="None"
            Height="50px" Width="125px">
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
            <Fields>
              <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
              <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
              <asp:CommandField ShowInsertButton="True" />
            </Fields>
            <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
          </asp:DetailsView>
        </asp:View>
      </asp:MultiView><br />
      <asp:ObjectDataSource ID="RolesDataSource" runat="server" SelectMethod="GetRoles"
        TypeName="RolesData" DataObjectTypeName="ProjectTracker.Library.Admin.Role" DeleteMethod="Delete" InsertMethod="InsertRole" OldValuesParameterFormatString="original_{0}" UpdateMethod="UpdateRole"></asp:ObjectDataSource>
    
    </div>
    </form>
</body>
</html>
