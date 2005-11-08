<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ProjectEdit.aspx.vb" Inherits="ProjectEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-size: x-large">
      Project Edit<br />
    
    </div>
    <div>
      &nbsp;<br />
      <br />
      <asp:LinkButton ID="ProjectListButton" runat="server">Project list</asp:LinkButton><br />
      <br />
      <asp:DetailsView ID="DetailsView1" runat="server" Height="50px" Width="536px" AutoGenerateRows="False" DataSourceID="ProjectDataSource" AllowPaging="True" CellPadding="4" ForeColor="#333333" GridLines="None">
        <Fields>
          <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" />
          <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
          <asp:BoundField DataField="Ended" HeaderText="Ended" SortExpression="Ended" />
          <asp:BoundField DataField="Started" HeaderText="Started" SortExpression="Started" />
          <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
          <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ShowInsertButton="True" />
        </Fields>
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
      </asp:DetailsView>
      &nbsp;&nbsp;&nbsp;<br />
      <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
        CellPadding="4" DataSourceID="ResourcesDataSource" ForeColor="#333333" GridLines="None" Width="432px">
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <Columns>
          <asp:BoundField DataField="ResourceId" HeaderText="ResourceId" ReadOnly="True" SortExpression="ResourceId" Visible="False" />
          <asp:BoundField DataField="FullName" HeaderText="Name" ReadOnly="True" SortExpression="FullName" />
          <asp:BoundField DataField="Assigned" HeaderText="Assigned" ReadOnly="True" SortExpression="Assigned" />
          <asp:TemplateField HeaderText="Role" SortExpression="Role">
            <EditItemTemplate>
              <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="RoleListDataSource"
                DataTextField="Value" DataValueField="Key" SelectedValue='<%# Bind("Role") %>'>
              </asp:DropDownList>
            </EditItemTemplate>
            <ItemTemplate>
              <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="RoleListDataSource"
                DataTextField="Value" DataValueField="Key" Enabled="False" SelectedValue='<%# Bind("Role") %>'>
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
      <br />
      <csla:CslaDataSource ID="ProjectDataSource" runat="server" 
          TypeName="ProjectTracker.Library.Project" TypeAssemblyName="ProjectTracker.Library">
      </csla:CslaDataSource> 
      <csla:CslaDataSource ID="ResourcesDataSource" runat="server" 
          TypeName="ProjectTracker.Library.ProjectResources" TypeAssemblyName="ProjectTracker.Library">
      </csla:CslaDataSource> 
      <csla:CslaDataSource ID="RoleListDataSource" runat="server" 
          TypeName="ProjectTracker.Library.RoleList" TypeAssemblyName="ProjectTracker.Library">
      </csla:CslaDataSource> 
      <br />
    </div>
      <br />
      &nbsp;
    </form>
</body>
</html>
