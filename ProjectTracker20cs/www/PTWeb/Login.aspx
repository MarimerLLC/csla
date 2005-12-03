<%@ Page Language="C#" AutoEventWireup="false" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Login ID="Login1" runat="server" DisplayRememberMe="False" LoginButtonText="Login"
            TitleText="Project Tracker Login" UserNameLabelText="Username:">
            <TitleTextStyle Font-Size="X-Large" HorizontalAlign="Left" />
        </asp:Login>
    </form>
</body>
</html>
