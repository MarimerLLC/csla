<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ProjectEdit.aspx.vb" Inherits="ProjectEdit" title="Project Information" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
      &nbsp;<br />
      <br />
      <asp:LinkButton ID="ProjectListButton" runat="server">Project list</asp:LinkButton><br />
      <br />
      <asp:DetailsView ID="DetailsView1" runat="server" Height="50px" Width="536px" AutoGenerateRows="False" DataSourceID="ProjectDataSource" DataKeyNames="Id">
        <Fields>
          <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" />
          <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
          <asp:BoundField DataField="Ended" HeaderText="Ended" SortExpression="Ended" />
          <asp:BoundField DataField="Started" HeaderText="Started" SortExpression="Started" />
          <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
          <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ShowInsertButton="True" />
        </Fields>
      </asp:DetailsView>
      &nbsp;&nbsp;&nbsp;<br />
      <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataSourceID="ResourcesDataSource" Width="432px" DataKeyNames="ResourceId">
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
      </asp:GridView>
      <asp:LinkButton ID="AddResourceButton" runat="server">Add resource</asp:LinkButton><br />
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
</asp:Content>