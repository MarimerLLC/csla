<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ResourceEdit.aspx.cs" Inherits="ResourceEdit" title="Resource Information" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <div>
        <asp:LinkButton ID="ResourceListButton" runat="server" OnClick="ResourceListButton_Click">Resource list</asp:LinkButton><br />
        <br />
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4"
          DataSourceID="ResourceDataSource" ForeColor="#333333" GridLines="None" Height="50px"
          Width="440px" DataKeyNames="Id" OnItemDeleted="DetailsView1_ItemDeleted">
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
        TypeName="ProjectTracker.Library.Resource" TypeAssemblyName="ProjectTracker.Library" OnDeleteObject="ResourceDataSource_DeleteObject" OnInsertObject="ResourceDataSource_InsertObject" OnSelectObject="ResourceDataSource_SelectObject" OnUpdateObject="ResourceDataSource_UpdateObject"></csla:CslaDataSource>
      <csla:CslaDataSource ID="AssignmentsDataSource" runat="server"
        TypeName="ProjectTracker.Library.ResourceAssignments" TypeAssemblyName="ProjectTracker.Library" OnDeleteObject="AssignmentsDataSource_DeleteObject" OnSelectObject="AssignmentsDataSource_SelectObject" OnUpdateObject="AssignmentsDataSource_UpdateObject"></csla:CslaDataSource>
      <csla:CslaDataSource ID="RoleListDataSource" runat="server"
        TypeName="ProjectTracker.Library.RoleList" TypeAssemblyName="ProjectTracker.Library" OnSelectObject="RoleListDataSource_SelectObject"></csla:CslaDataSource>
       </div>
</asp:Content>