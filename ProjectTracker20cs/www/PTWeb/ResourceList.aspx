<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ResourceList.aspx.cs" Inherits="ResourceList" title="Resource List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
      <br />
      <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Home</asp:HyperLink><br />
      <br />
      <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
        CellPadding="4" DataSourceID="ResourceListDataSource" ForeColor="#333333"
        GridLines="None" DataKeyNames="Id" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <Columns>
          <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id"
            Visible="False" />
          <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" SortExpression="Name" />
          <asp:CommandField ShowDeleteButton="True" ShowSelectButton="True" SelectText="Edit" />
        </Columns>
        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
      </asp:GridView>
      <asp:LinkButton ID="NewResourceButton" runat="server" OnClick="NewResourceButton_Click">New resource</asp:LinkButton><br />
      <csla:CslaDataSource ID="ResourceListDataSource" runat="server"
        TypeAssemblyName="ProjectTracker.Library" TypeName="ProjectTracker.Library.ResourceList" OnDeleteObject="ResourceListDataSource_DeleteObject" OnSelectObject="ResourceListDataSource_SelectObject">
      </csla:CslaDataSource>
      &nbsp;</div>
</asp:Content>