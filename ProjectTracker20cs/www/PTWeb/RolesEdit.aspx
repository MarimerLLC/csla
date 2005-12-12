<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RolesEdit.aspx.cs" Inherits="RolesEdit" title="Project Roles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div>
      <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="MainView" runat="server">
      <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
        DataSourceID="RolesDataSource" DataKeyNames="Id">
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" />
          <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
          <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
        </Columns>
      </asp:GridView>
          <asp:LinkButton ID="AddRoleButton" runat="server" OnClick="AddRoleButton_Click">Add role</asp:LinkButton><br />
        </asp:View>
        <asp:View ID="InsertView" runat="server">
          <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False"
            DataSourceID="RolesDataSource" DefaultMode="Insert"
            Height="50px" Width="125px" DataKeyNames="Id" OnItemInserted="DetailsView1_ItemInserted" OnModeChanged="DetailsView1_ModeChanged">
            <Fields>
              <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
              <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
              <asp:CommandField ShowInsertButton="True" />
            </Fields>
          </asp:DetailsView>
        </asp:View>
      </asp:MultiView><br />
        &nbsp;&nbsp;<csla:CslaDataSource ID="RolesDataSource" runat="server" TypeAssemblyName="ProjectTracker.Library"
          TypeName="ProjectTracker.Library.Admin.Roles" OnDeleteObject="RolesDataSource_DeleteObject" OnInsertObject="RolesDataSource_InsertObject" OnSelectObject="RolesDataSource_SelectObject" OnUpdateObject="RolesDataSource_UpdateObject">
        </csla:CslaDataSource>
      </div>
</asp:Content>