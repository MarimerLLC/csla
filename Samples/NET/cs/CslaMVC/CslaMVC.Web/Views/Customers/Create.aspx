<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CslaMVC.ViewModels.CustomerViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

		<h2>Customer Create</h2>

		<% using (Html.BeginForm()) {%>
				<%= Html.ValidationSummary(false) %>

				<fieldset>
						<legend>Fields</legend>
												
						<div class="editor-label">
								<%= Html.LabelFor(model => model.ModelObject.Name)%>
						</div>
						<div class="editor-field">
								<%= Html.TextBoxFor(model => model.ModelObject.Name)%>
								<%= Html.ValidationMessageFor(model => model.ModelObject.Name)%>
						</div>
						
						<div class="editor-label">
								<%= Html.LabelFor(model => model.ModelObject.GroupNo)%>
						</div>
						<div class="editor-field">
								<%= Html.TextBoxFor(model => model.ModelObject.GroupNo)%>
								<%= Html.ValidationMessageFor(model => model.ModelObject.GroupNo)%>
						</div>
						
						<div class="editor-label">
								<%= Html.LabelFor(model => model.ModelObject.City)%>
						</div>
						<div class="editor-field">
								<%= Html.TextBoxFor(model => model.ModelObject.City)%>
								<%= Html.ValidationMessageFor(model => model.ModelObject.City)%>
						</div>
						
						<div class="editor-label">
								<%= Html.LabelFor(model => model.ModelObject.State)%>
						</div>
						<div class="editor-field">
								<%--= Html.TextBoxFor(model => model.ModelObject.State)--%>
								<%= Html.DropDownListFor(model => model.ModelObject.State, new SelectList(Model.StateList))%>
								<%= Html.ValidationMessageFor(model => model.ModelObject.State)%>
						</div>
						
						<div class="editor-label">
								<%= Html.LabelFor(model => model.ModelObject.Zipcode)%>
						</div>
						<div class="editor-field">
								<%= Html.TextBoxFor(model => model.ModelObject.Zipcode)%>
								<%= Html.ValidationMessageFor(model => model.ModelObject.Zipcode)%>
						</div>

						<div class="editor-label">
								<%= Html.LabelFor(model => model.ModelObject.Start)%>
						</div>
						<div class="editor-field">
								<%= Html.TextBoxFor(model => model.ModelObject.Start)%>
								<%= Html.ValidationMessageFor(model => model.ModelObject.Start)%>
						</div>

						<div class="editor-label">
								<%= Html.LabelFor(model => model.ModelObject.End)%>
						</div>
						<div class="editor-field">
								<%= Html.TextBoxFor(model => model.ModelObject.End)%>
								<%= Html.ValidationMessageFor(model => model.ModelObject.End)%>
						</div>
						
						<p>
								<input type="submit" value="Create" />
						</p>
				</fieldset>

		<% } %>

		<div>
				<%= Html.ActionLink("Back to List", "Index") %>
		</div>

</asp:Content>

