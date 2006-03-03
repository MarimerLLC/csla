<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" 
  AutoEventWireup="false" CodeFile="RolesEdit.aspx.vb" 
  Inherits="RolesEdit" title="Project Roles" %>
<asp:Content ID="Content1" 
  ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div>
      <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="MainView" runat="server">
      <asp:GridView ID="GridView1" runat="server" 
        AutoGenerateColumns="False"
        DataSourceID="RolesDataSource" 
        DataKeyNames="Id">
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" 
            ReadOnly="True" SortExpression="Id" />
          <asp:BoundField DataField="Name" HeaderText="Name" 
            SortExpression="Name" />
          <asp:CommandField ShowDeleteButton="True" 
            ShowEditButton="True" />
        </Columns>
      </asp:GridView>
          <asp:LinkButton ID="AddRoleButton" runat="server">Add role</asp:LinkButton><br />
        </asp:View>
        <asp:View ID="InsertView" runat="server">
        <asp:DetailsView ID="DetailsView1" runat="server" 
          AutoGenerateRows="False" DataSourceID="RolesDataSource" 
          DefaultMode="Insert" Height="50px" Width="125px" 
          DataKeyNames="Id">
          <Fields>
            <asp:BoundField DataField="Id" HeaderText="Id" 
              SortExpression="Id" />
            <asp:BoundField DataField="Name" HeaderText="Name" 
              SortExpression="Name" />
            <asp:CommandField ShowInsertButton="True" />
          </Fields>
        </asp:DetailsView>
        </asp:View>
      </asp:MultiView><br />
  <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label><br />
        &nbsp;&nbsp;
    <csla:CslaDataSource ID="RolesDataSource" runat="server" 
      TypeAssemblyName="ProjectTracker.Library"
      TypeName="ProjectTracker.Library.Admin.Roles">
    </csla:CslaDataSource>
      </div>
</asp:Content>