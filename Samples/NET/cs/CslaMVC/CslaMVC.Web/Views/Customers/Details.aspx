<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CslaMVC.ViewModels.CustomerViewModel>" %>
<%@ Import Namespace="Csla.Web.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Customer Details</h2>

	<fieldset>
		<legend>Fields</legend>
		
		<div class="display-label">CustomerNo</div>
		<div class="display-field"><%= Html.Encode(Model.ModelObject.CustomerNo)%></div>
		
		<div class="display-label">Name</div>
		<div class="display-field"><%= Html.DisplayFor(model => model.ModelObject.Name)%></div>
		
		<div class="display-label">GroupNo</div>
		<div class="display-field"><%= Html.Encode(Model.ModelObject.GroupNo)%></div>
		
		<div class="display-label">City</div>
		<div class="display-field"><%= Html.Encode(Model.ModelObject.City)%></div>
		
		<div class="display-label">State</div>
		<div class="display-field"><%= Html.Encode(Model.ModelObject.State)%></div>
		
		<div class="display-label">Zipcode</div>
		<div class="display-field"><%= Html.Encode(Model.ModelObject.Zipcode)%></div>
		
	</fieldset>
	<p>
		<%= Html.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(CslaMVC.Library.Customer), Html.ActionLink("Edit", "Edit", new { id = Model.ModelObject.CustomerNo }), "Edit")%> |
		<%= Html.ActionLink("Back to List", "Index") %> |
		<%= Html.ActionLink("Orders Index", "Index", "Orders", new { id = Model.ModelObject.CustomerNo }, null)%>
	</p>

</asp:Content>

