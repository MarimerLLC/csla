<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ResourceList.aspx.cs" Inherits="ResourceList" title="Resource List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
      <strong>Resources:</strong><br />
      <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataSourceID="ResourceListDataSource" DataKeyNames="Id" OnRowDeleted="GridView1_RowDeleted" >
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id"
            Visible="False" />
          <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="ResourceEdit.aspx?id={0}"
            DataTextField="Name" HeaderText="Name" />
          <asp:CommandField ShowDeleteButton="True" SelectText="Edit" />
        </Columns>
      </asp:GridView>
      <asp:LinkButton ID="NewResourceButton" runat="server" OnClick="NewResourceButton_Click">New resource</asp:LinkButton><br />
      <br />
      <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red"></asp:Label>
      <csla:CslaDataSource ID="ResourceListDataSource" runat="server"
        TypeAssemblyName="ProjectTracker.Library" TypeName="ProjectTracker.Library.ResourceList" OnDeleteObject="ResourceListDataSource_DeleteObject" OnSelectObject="ResourceListDataSource_SelectObject">
      </csla:CslaDataSource>
      &nbsp;</div>
</asp:Content>