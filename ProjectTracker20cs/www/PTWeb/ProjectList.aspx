<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ProjectList.aspx.cs" Inherits="ProjectList" title="Project List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <div>
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Home</asp:HyperLink><br />
        <br />
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataSourceID="ProjectListDataSource" PageSize="4" DataKeyNames="Id" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
          <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" Visible="False" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:CommandField ShowDeleteButton="True" ShowSelectButton="True" SelectText="Edit" />
          </Columns>
        </asp:GridView>
        <asp:LinkButton ID="NewProjectButton" runat="server" OnClick="NewProjectButton_Click">New project</asp:LinkButton>
        <csla:CslaDataSource ID="ProjectListDataSource" runat="server"
          TypeName="ProjectTracker.Library.ProjectList" TypeAssemblyName="ProjectTracker.Library" OnDeleteObject="ProjectListDataSource_DeleteObject" OnSelectObject="ProjectListDataSource_SelectObject"></csla:CslaDataSource>
        <br />
      </div>
</asp:Content>