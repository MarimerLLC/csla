<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="ShowProject.aspx.cs" Inherits="PTWebMvc.Views.Home.ShowProject" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<table border="1" cellpadding="3">
<tr>
<td>Id</td>
<td>
<% =((ProjectTracker.Library.Project)ViewData["Data"]).Id.ToString() %>
</td>
</tr>
<tr>
<td>Name</td>
<td>
<% =((ProjectTracker.Library.Project)ViewData["Data"]).Name %>
</td>
</tr>
<tr>
<td>Started</td>
<td>
<% =((ProjectTracker.Library.Project)ViewData["Data"]).Started %>
</td>
</tr>
<tr>
<td>Ended</td>
<td>
<% =((ProjectTracker.Library.Project)ViewData["Data"]).Ended %>
</td>
</tr>
<tr>
<td>Description</td>
<td>
<%= Html.TextArea("Description", ((ProjectTracker.Library.Project)ViewData["Data"]).Description)  %>
</td>
</tr>
</table>
</asp:Content>
