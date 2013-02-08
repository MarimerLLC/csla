<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    About Us
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>About</h2>
    <p>
        SimpleNTier application created using <a href="http://www.lhotka.net/cslanet/">CSLA 4</a> and <a href="http://www.asp.net">ASP.NET MVC</a>.
    </p>
</asp:Content>
