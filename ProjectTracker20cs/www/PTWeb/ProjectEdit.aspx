<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ProjectEdit.aspx.cs" Inherits="ProjectEdit" title="Project Information" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
      &nbsp;<br />
      <br />
      <asp:LinkButton ID="ProjectListButton" runat="server" OnClick="ProjectListButton_Click">Project list</asp:LinkButton><br />
      <br />
      <asp:DetailsView ID="DetailsView1" runat="server" Height="50px" Width="536px" AutoGenerateRows="False" DataSourceID="ProjectDataSource" AllowPaging="True" CellPadding="4" ForeColor="#333333" GridLines="None" DataKeyNames="Id" OnItemDeleted="DetailsView1_ItemDeleted">
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
        CellPadding="4" DataSourceID="ResourcesDataSource" ForeColor="#333333" GridLines="None" Width="432px" DataKeyNames="ResourceId">
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
          TypeName="ProjectTracker.Library.Project" TypeAssemblyName="ProjectTracker.Library" OnDeleteObject="ProjectDataSource_DeleteObject" OnInsertObject="ProjectDataSource_InsertObject" OnSelectObject="ProjectDataSource_SelectObject" OnUpdateObject="ProjectDataSource_UpdateObject">
      </csla:CslaDataSource> 
      <csla:CslaDataSource ID="ResourcesDataSource" runat="server" 
          TypeName="ProjectTracker.Library.ProjectResources" TypeAssemblyName="ProjectTracker.Library" OnDeleteObject="ResourcesDataSource_DeleteObject" OnSelectObject="ResourcesDataSource_SelectObject" OnUpdateObject="ResourcesDataSource_UpdateObject">
      </csla:CslaDataSource> 
      <csla:CslaDataSource ID="RoleListDataSource" runat="server" 
          TypeName="ProjectTracker.Library.RoleList" TypeAssemblyName="ProjectTracker.Library" OnSelectObject="RoleListDataSource_SelectObject">
      </csla:CslaDataSource> 
      <br />
    </div>
      <br />
      &nbsp;
</asp:Content>