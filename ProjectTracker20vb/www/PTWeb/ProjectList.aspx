<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ProjectList.aspx.vb" Inherits="ProjectList" title="Project List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <div>
        <strong>Projects:<br />
        </strong>
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataSourceID="ProjectListDataSource" PageSize="4" DataKeyNames="Id">
          <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" 
              SortExpression="ID" Visible="False" />
            <asp:HyperLinkField DataNavigateUrlFields="Id" 
              DataNavigateUrlFormatString="ProjectEdit.aspx?id={0}"
              DataTextField="Name" HeaderText="Name" />
            <asp:CommandField ShowDeleteButton="True" 
              SelectText="Edit" />
          </Columns>
        </asp:GridView>
        <asp:LinkButton ID="NewProjectButton" runat="server">New project</asp:LinkButton>
        <csla:CslaDataSource ID="ProjectListDataSource" runat="server"
          TypeName="ProjectTracker.Library.ProjectList" TypeAssemblyName="ProjectTracker.Library"></csla:CslaDataSource>
        <br />
        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label></div>
</asp:Content>