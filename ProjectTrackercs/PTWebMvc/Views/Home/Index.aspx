<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="PTWebMvc.Views.Home.Index" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <p>
        To learn more about CSLA .NET visit <a href="http://www.lhotka.net/cslanet" title="CSLA .NET Website">www.lhotka.net</a>.
    </p>
</asp:Content>
