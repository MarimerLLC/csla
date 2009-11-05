<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Default.aspx.cs" Inherits="Web._Default" %>


<%@ Register Assembly="System.Web.Silverlight" Namespace="System.Web.UI.SilverlightControls"
	TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<title>Test Page For Sample Silverlight Application</title>
</head>
<body style="height:100%;margin:0;">
	<form id="form1" runat="server" style="height:100%;">
		<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
		<div  style="height:100%;">
			<asp:Silverlight ID="Xaml1" runat="server" Source="~/ClientBin/SampleSilverlightApplication.xap" MinimumVersion="2.0.30523" Width="600" Height="400" />
		</div>
	</form>
</body>
</html>
