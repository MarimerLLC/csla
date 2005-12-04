<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ResourceEdit.aspx.vb" Inherits="ResourceEdit" title="Resource Information" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <div>
        <asp:LinkButton ID="ResourceListButton" runat="server">Resource list</asp:LinkButton><br />
        <br />
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False"
          DataSourceID="ResourceDataSource" Height="50px"
          Width="440px" DataKeyNames="Id">
          <Fields>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False" />
            <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" />
            <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ShowInsertButton="True" />
          </Fields>
        </asp:DetailsView>
        <br />
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
          DataSourceID="AssignmentsDataSource" DataKeyNames="ProjectID">
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
        </asp:GridView>
        <br />
      <csla:CslaDataSource ID="ResourceDataSource" runat="server"
        TypeName="ProjectTracker.Library.Resource" TypeAssemblyName="ProjectTracker.Library"></csla:CslaDataSource>
      <csla:CslaDataSource ID="AssignmentsDataSource" runat="server"
        TypeName="ProjectTracker.Library.ResourceAssignments" TypeAssemblyName="ProjectTracker.Library"></csla:CslaDataSource>
      <csla:CslaDataSource ID="RoleListDataSource" runat="server"
        TypeName="ProjectTracker.Library.RoleList" TypeAssemblyName="ProjectTracker.Library"></csla:CslaDataSource>
       </div>
</asp:Content>