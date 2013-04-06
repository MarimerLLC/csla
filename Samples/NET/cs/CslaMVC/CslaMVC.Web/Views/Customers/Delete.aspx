<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CslaMVC.ViewModels.CustomerViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Delete
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Customer Delete</h2>

    <h3>Are you sure you want to delete this?</h3>
    <fieldset>
        <legend>Fields</legend>
        
        <div class="display-label">CustomerNo</div>
        <div class="display-field"><%= Html.Encode(Model.ModelObject.CustomerNo)%></div>
        
        <div class="display-label">Name</div>
        <div class="display-field"><%= Html.Encode(Model.ModelObject.Name)%></div>
        
        <div class="display-label">GroupNo</div>
        <div class="display-field"><%= Html.Encode(Model.ModelObject.GroupNo)%></div>
        
        <div class="display-label">City</div>
        <div class="display-field"><%= Html.Encode(Model.ModelObject.City)%></div>
        
        <div class="display-label">State</div>
        <div class="display-field"><%= Html.Encode(Model.ModelObject.State)%></div>
        
        <div class="display-label">Zipcode</div>
        <div class="display-field"><%= Html.Encode(Model.ModelObject.Zipcode)%></div>
        
    </fieldset>
    <% using (Html.BeginForm()) { %>
        <p>
		    <input type="submit" value="Delete" /> |
		    <%= Html.ActionLink("Back to List", "Index") %>
        </p>
    <% } %>

</asp:Content>

