<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ResourceList.aspx.vb" Inherits="ResourceList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-size: x-large">
      Resource List</div>
    <div>
      <br />
      <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
        CellPadding="4" DataKeyNames="Id" DataSourceID="ResourceListDataSource" ForeColor="#333333"
        GridLines="None">
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
      <asp:LinkButton ID="NewResourceButton" runat="server">New resource</asp:LinkButton><br />
      <csla:CslaDataSource ID="ResourceListDataSource" runat="server"
        TypeAssemblyName="ProjectTracker.Library" TypeName="ProjectTracker.Library.ResourceList">
      </csla:CslaDataSource>
      &nbsp;</div>
    </form>
</body>
</html>
