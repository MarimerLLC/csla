<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="EditProject.aspx.cs" Inherits="PTWebMvc.Views.Home.EditProject" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <%= Html.Form("Home", "EditProject", FormMethod.Post) %>
  <%--<form action="~/Home/EditProject/<%= ((ProjectTracker.Library.Project)ViewData["Data"]).Id.ToString() %>" method="post">--%>
    <table border="1" cellpadding="3">
    <thead><tr><td>Id</td><td>Name</td><td>Started</td></tr></thead>
      <tr>
      <td>
        <%= ((ProjectTracker.Library.Project)ViewData["Data"]).Id.ToString() %>
      </td>
      <td>
        <%= Html.TextBox("Name", ((ProjectTracker.Library.Project)ViewData["Data"]).Name) %>
      </td>
      <td>
        <%= Html.TextBox("Started", ((ProjectTracker.Library.Project)ViewData["Data"]).Started) %>
      </td>
      </tr>
    </table>
    <input type="submit" value="Save" />
  </form>
</asp:Content>
