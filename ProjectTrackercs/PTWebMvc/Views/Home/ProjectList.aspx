<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="ProjectList.aspx.cs" Inherits="PTWebMvc.Views.Home.ProjectList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <p>
    This is some content</p>
    <% 
       foreach (var item in ViewData["Data"] as ProjectTracker.Library.ProjectList)
         Response.Write(Html.ActionLink(string.Format("{0}", item.Name), 
           ViewData["Page"].ToString(), new RouteValueDictionary() { {"Id", item.Id }}) + "<br>");
    %>
</asp:Content>
