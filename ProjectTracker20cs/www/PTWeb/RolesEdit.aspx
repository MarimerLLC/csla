<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="RolesEdit.aspx.cs" Inherits="RolesEdit" title="Project Roles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div>
      <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="MainView" runat="server">
      <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"
        DataSourceID="RolesDataSource" ForeColor="#333333" GridLines="None" DataKeyNames="Id">
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" />
          <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
          <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
        </Columns>
        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
      </asp:GridView>
          <asp:LinkButton ID="AddRoleButton" runat="server" OnClick="AddRoleButton_Click">Add role</asp:LinkButton><br />
        </asp:View>
        <asp:View ID="InsertView" runat="server">
          <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4"
            DataSourceID="RolesDataSource" DefaultMode="Insert" ForeColor="#333333" GridLines="None"
            Height="50px" Width="125px" DataKeyNames="Id" OnItemInserted="DetailsView1_ItemInserted" OnModeChanged="DetailsView1_ModeChanged">
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
        &nbsp;&nbsp;<csla:CslaDataSource ID="RolesDataSource" runat="server" TypeAssemblyName="ProjectTracker.Library"
          TypeName="ProjectTracker.Library.Admin.Roles" OnDeleteObject="RolesDataSource_DeleteObject" OnInsertObject="RolesDataSource_InsertObject" OnSelectObject="RolesDataSource_SelectObject" OnUpdateObject="RolesDataSource_UpdateObject">
        </csla:CslaDataSource>
      </div>
</asp:Content>