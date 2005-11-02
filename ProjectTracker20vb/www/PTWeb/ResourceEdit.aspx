<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ResourceEdit.aspx.vb" Inherits="ResourceEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <br />
      &nbsp;</div>
      <div>
        <asp:LinkButton ID="ResourceListButton" runat="server">Resource list</asp:LinkButton><br />
        <br />
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4"
          DataSourceID="ResourceDataSource" ForeColor="#333333" GridLines="None" Height="50px"
          Width="440px" DataKeyNames="Id">
          <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
          <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
          <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
          <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
          <Fields>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False" />
            <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" />
            <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ShowInsertButton="True" />
          </Fields>
          <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
          <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
          <AlternatingRowStyle BackColor="White" />
        </asp:DetailsView>
        <br />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"
          DataSourceID="AssignmentsDataSource" ForeColor="#333333" GridLines="None" DataKeyNames="ProjectID">
          <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
          <Columns>
            <asp:BoundField DataField="ProjectID" HeaderText="ProjectID" SortExpression="ProjectID"
              Visible="False" />
            <asp:BoundField DataField="ProjectName" HeaderText="ProjectName" SortExpression="ProjectName" />
            <asp:BoundField DataField="Assigned" HeaderText="Assigned" SortExpression="Assigned" />
            <asp:TemplateField HeaderText="Role" SortExpression="Role">
              <EditItemTemplate><asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="RoleListDataSource" DataTextField="Value" DataValueField="Key" SelectedValue='<%# Bind("Role") %>' Width="184px">
              </asp:DropDownList>
              </EditItemTemplate>
              <ItemTemplate>
                <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="RoleListDataSource" DataTextField="Value" DataValueField="Key" Enabled="False" SelectedValue='<%# Bind("Role") %>' Width="184px">
                </asp:DropDownList>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
          </Columns>
          <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
          <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
          <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
          <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
          <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
        <br />
      <csla:CslaDataSource ID="ResourceDataSource" runat="server"
        TypeName="ProjectTracker.Library.Resource" TypeAssemblyName="ProjectTracker.Library"></csla:CslaDataSource>
      <csla:CslaDataSource ID="AssignmentsDataSource" runat="server"
        TypeName="ProjectTracker.Library.ResourceAssignments" TypeAssemblyName="ProjectTracker.Library"></csla:CslaDataSource>
      <csla:CslaDataSource ID="RoleListDataSource" runat="server"
        TypeName="ProjectTracker.Library.RoleList" TypeAssemblyName="ProjectTracker.Library"></csla:CslaDataSource>
       </div>
    </form>
</body>
</html>
