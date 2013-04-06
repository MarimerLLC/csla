<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CslaMVC.Library.Order>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Order Details</h2>

    <fieldset>
        <legend>Fields</legend>
        
        <div class="display-label">CustomerNo</div>
        <div class="display-field"><%: Model.CustomerNo %></div>
        
        <div class="display-label">OrderId</div>
        <div class="display-field"><%: Model.OrderId %></div>
        
        <div class="display-label">OrderDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.OrderDate) %></div>
        
        <div class="display-label">Status</div>
        <div class="display-field"><%: Model.Status %></div>
        
        <div class="display-label">ShippedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.ShippedDate) %></div>
        
        <div class="display-label">ReceivedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.ReceivedDate) %></div>
        
    </fieldset>
    <p>
        Edit<%--<%: Html.ActionLink("Edit", "Edit", new { id = Model.OrderId }, null) %>--%> |
        <%: Html.ActionLink("Back to List", "Index", new { id = Model.CustomerNo }, null) %>
    </p>

</asp:Content>

