<%@ Page Language="C#" AutoEventWireup="false" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Login ID="Login1" runat="server" DisplayRememberMe="False" LoginButtonText="Login"
            TitleText="Project Tracker Login" UserNameLabelText="Username:" BackColor="#FFFBD6" BorderColor="#FFDFAD" BorderPadding="4" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" TextLayout="TextOnTop">
            <TitleTextStyle Font-Size="0.9em" HorizontalAlign="Left" BackColor="#990000" Font-Bold="True" ForeColor="White" />
          <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
          <TextBoxStyle Font-Size="0.8em" />
          <LoginButtonStyle BackColor="White" BorderColor="#CC9966" BorderStyle="Solid" BorderWidth="1px"
            Font-Names="Verdana" Font-Size="0.8em" ForeColor="#990000" />
        </asp:Login>
    </form>
</body>
</html>
