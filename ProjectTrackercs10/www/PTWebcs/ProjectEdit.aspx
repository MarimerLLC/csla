<%@ Page language="c#" Codebehind="ProjectEdit.aspx.cs" AutoEventWireup="false" Inherits="PTWebcs.ProjectEdit" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>ProjectEdit</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body MS_POSITIONING="FlowLayout">
    <FORM id="Form1" method="post" runat="server">
      <H1>Edit Project</H1>
      <P>
        <asp:HyperLink id="HyperLink1" runat="server" NavigateUrl="Default.aspx">Home</asp:HyperLink>&nbsp;&nbsp;&nbsp;
        <asp:HyperLink id="HyperLink3" runat="server" NavigateUrl="Projects.aspx">Project list</asp:HyperLink>&nbsp;&nbsp;&nbsp;
        <asp:LinkButton id="btnNewProject" runat="server">Add new project</asp:LinkButton></P>
      <P>
        <TABLE id="Table1" cellSpacing="1" cellPadding="1" width="100%" border="0">
          <TR>
            <TD>ID</TD>
            <TD>
              <asp:TextBox id=txtID runat="server" ReadOnly="True" Width="245px" Text="<%# _project.ID.ToString() %>">
              </asp:TextBox></TD>
          </TR>
          <TR>
            <TD>Name</TD>
            <TD>
              <asp:TextBox id=txtName runat="server" Text="<%# _project.Name %>">
              </asp:TextBox>&nbsp;
              <asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ErrorMessage="Name is required" ControlToValidate="txtName"></asp:RequiredFieldValidator></TD>
          </TR>
          <TR>
            <TD>Started</TD>
            <TD>
              <asp:TextBox id=txtStarted runat="server" Text="<%# _project.Started %>">
              </asp:TextBox>&nbsp;
              <asp:CompareValidator id="CompareValidator1" runat="server" ErrorMessage="Must be earlier than ended" ControlToValidate="txtStarted" ControlToCompare="txtEnded" Operator="LessThanEqual" Type="Date"></asp:CompareValidator></TD>
          </TR>
          <TR>
            <TD style="HEIGHT: 24px">Ended</TD>
            <TD style="HEIGHT: 24px">
              <asp:TextBox id=txtEnded runat="server" Text="<%# _project.Ended %>">
              </asp:TextBox>&nbsp;
              <asp:CompareValidator id="CompareValidator2" runat="server" ErrorMessage="Must be later than started" ControlToValidate="txtEnded" ControlToCompare="txtStarted" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator></TD>
          </TR>
          <TR>
            <TD>Description</TD>
            <TD>
              <asp:TextBox id=txtDescription runat="server" Width="435px" Text="<%# _project.Description %>" TextMode="MultiLine" Height="112px">
              </asp:TextBox></TD>
          </TR>
          <TR>
            <TD>Resources</TD>
            <TD>
              <asp:DataGrid id="dgResources" runat="server" AutoGenerateColumns="False" EnableViewState="False" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="4">
                <SelectedItemStyle Font-Bold="True" ForeColor="#CCFF99" BackColor="#009999"></SelectedItemStyle>
                <ItemStyle ForeColor="#003399" BackColor="White"></ItemStyle>
                <HeaderStyle Font-Bold="True" ForeColor="#CCCCFF" BackColor="#003399"></HeaderStyle>
                <FooterStyle ForeColor="#003399" BackColor="#99CCCC"></FooterStyle>
                <Columns>
                  <asp:BoundColumn Visible="False" DataField="ResourceID" HeaderText="Resource ID"></asp:BoundColumn>
                  <asp:BoundColumn DataField="FirstName" HeaderText="First name"></asp:BoundColumn>
                  <asp:BoundColumn DataField="LastName" HeaderText="Last name"></asp:BoundColumn>
                  <asp:BoundColumn DataField="Assigned" HeaderText="Assigned"></asp:BoundColumn>
                  <asp:ButtonColumn DataTextField="Role" HeaderText="Role" CommandName="SelectRole"></asp:ButtonColumn>
                  <asp:ButtonColumn Text="Remove" CommandName="Delete"></asp:ButtonColumn>
                  <asp:ButtonColumn Text="Details" CommandName="Select"></asp:ButtonColumn>
                </Columns>
                <PagerStyle HorizontalAlign="Left" ForeColor="#003399" BackColor="#99CCCC" Mode="NumericPages"></PagerStyle>
              </asp:DataGrid>
              <P>
                <asp:LinkButton id="btnAssignResource" runat="server">Assign resource</asp:LinkButton></P>
            </TD>
          </TR>
        </TABLE>
      </P>
      <P>
        <asp:Button id="btnSave" runat="server" Text="Save"></asp:Button>&nbsp;&nbsp;&nbsp;
        <asp:Button id="btnCancel" runat="server" Text="Cancel"></asp:Button></P>
    </FORM>
  </body>
</HTML>
