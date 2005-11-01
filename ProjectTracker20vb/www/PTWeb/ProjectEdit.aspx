<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ProjectEdit.aspx.vb" Inherits="ProjectEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-size: x-large">
      Project Edit<br />
    
    </div>
    <div>
      &nbsp;<br />
      <br />
      <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4"
        DataSourceID="CslaDataSource1" ForeColor="#333333" GridLines="None" Height="50px"
        Width="125px">
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <CommandRowStyle BackColor="#FFFFC0" Font-Bold="True" />
        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
        <Fields>
          <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" />
          <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
          <asp:BoundField DataField="Started" HeaderText="Started" SortExpression="Started" />
          <asp:BoundField DataField="Ended" HeaderText="Ended" SortExpression="Ended" />
          <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
          <asp:CheckBoxField DataField="IsValid" HeaderText="IsValid" ReadOnly="True" SortExpression="IsValid" />
          <asp:CheckBoxField DataField="IsDirty" HeaderText="IsDirty" ReadOnly="True" SortExpression="IsDirty" />
          <asp:CheckBoxField DataField="IsNew" HeaderText="IsNew" ReadOnly="True" SortExpression="IsNew" />
          <asp:CheckBoxField DataField="IsDeleted" HeaderText="IsDeleted" ReadOnly="True" SortExpression="IsDeleted" />
          <asp:CheckBoxField DataField="IsSavable" HeaderText="IsSavable" ReadOnly="True" SortExpression="IsSavable" />
        </Fields>
        <FieldHeaderStyle BackColor="#FFFF99" Font-Bold="True" />
        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
      </asp:DetailsView>
      <br />
      <csla:CslaDataSource ID="CslaDataSource1" runat="server" 
          TypeName="ProjectTracker.Library.Project">
      </csla:CslaDataSource> 
      <br />
    </div>
      <br />
      &nbsp;
    </form>
</body>
</html>
