<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CslaMVC.Library.Order>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

		<h2>Order Create</h2>

		<% using (Html.BeginForm()) {%>
				<%: Html.ValidationSummary(false) %>

				<fieldset>
						<legend>Fields</legend>
						
						<div class="editor-label">
								<%: Html.LabelFor(model => model.CustomerNo) %>
						</div>
						<div class="editor-field">
								<%: Html.TextBoxFor(model => model.CustomerNo) %>
								<%: Html.ValidationMessageFor(model => model.CustomerNo) %>
						</div>
						
						<div class="editor-label">
								<%: Html.LabelFor(model => model.OrderId) %>
						</div>
						<div class="editor-field">
								<%: Html.TextBoxFor(model => model.OrderId) %>
								<%: Html.ValidationMessageFor(model => model.OrderId) %>
						</div>
						
						<div class="editor-label">
								<%: Html.LabelFor(model => model.OrderDate) %>
						</div>
						<div class="editor-field">
								<%: Html.TextBoxFor(model => model.OrderDate) %>
								<%: Html.ValidationMessageFor(model => model.OrderDate) %>
						</div>
						
						<div class="editor-label">
								<%: Html.LabelFor(model => model.Status) %>
						</div>
						<div class="editor-field">
								<%: Html.TextBoxFor(model => model.Status) %>
								<%: Html.ValidationMessageFor(model => model.Status) %>
						</div>
						
						<div class="editor-label">
								<%: Html.LabelFor(model => model.ShippedDate) %>
						</div>
						<div class="editor-field">
								<%: Html.TextBoxFor(model => model.ShippedDate) %>
								<%: Html.ValidationMessageFor(model => model.ShippedDate) %>
						</div>
						
						<div class="editor-label">
								<%: Html.LabelFor(model => model.ReceivedDate) %>
						</div>
						<div class="editor-field">
								<%: Html.TextBoxFor(model => model.ReceivedDate) %>
								<%: Html.ValidationMessageFor(model => model.ReceivedDate) %>
						</div>
						
						<p>
								<input type="submit" value="Create" />
						</p>
				</fieldset>

		<% } %>

		<div>
				<%: Html.ActionLink("Back to List", "Index") %>
		</div>

</asp:Content>

