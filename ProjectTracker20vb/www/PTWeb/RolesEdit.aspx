<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RolesEdit.aspx.vb" Inherits="RolesEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Edit Roles</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <h1>Edit Roles</h1><br />
      <br />
      <br />
      <asp:MultiView ID="MultiView3" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
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
      <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="RoleEdit.aspx?id=0&mode=insert">Add role</asp:HyperLink><br />
      <asp:ObjectDataSource ID="RolesDataSource" runat="server" SelectMethod="GetRoles"
        TypeName="RolesData" DataObjectTypeName="ProjectTracker.Library.Admin.Role" DeleteMethod="Delete"></asp:ObjectDataSource>
        </asp:View>
        <asp:View ID="View2" runat="server">
          <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataSourceID="RoleDataSource"
            Height="50px" Width="125px">
            <Fields>
              <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
              <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
              <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ShowInsertButton="True" />
            </Fields>
          </asp:DetailsView>
          <asp:ObjectDataSource ID="RoleDataSource" runat="server" DataObjectTypeName="ProjectTracker.Library.Admin.Role"
            DeleteMethod="Delete" InsertMethod="InsertRole" SelectMethod="GetRole" TypeName="RolesData"
            UpdateMethod="UpdateRole">
            <SelectParameters>
              <asp:QueryStringParameter Name="id" QueryStringField="id" Type="Int32" />
            </SelectParameters>
          </asp:ObjectDataSource>
        </asp:View>
      </asp:MultiView><br />
      <br />
      &nbsp;<br />
    
    </div>
    </form>
</body>
</html>
