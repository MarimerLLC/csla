<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CslaMVC.ViewModels.CustomerViewModel>" %>
<%@ Import Namespace="Csla.Web.Mvc" %>
<%@ Import Namespace="Csla.Rules" %>
<%@ Import Namespace="Csla.Security" %>
<%@ Import Namespace="CslaMVC.Library" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
      function handleUpdate(context) {
        //debugger;
        var jsonString = context.get_data();
        var test = new Function("return " + jsonString)();
        var msg = test.Message + '!';
        $('#action-message').text(msg);
      }

      function handleError(context) {
        //debugger;
        var jsonString = context.get_data();
        var test = new Function("return " + jsonString)();
        var msg = test.Action + ': ' + test.Message;
        $('#action-message').text(msg);
      }
    </script>

		<h2>Customer Edit</h2>

		<% using (Html.BeginForm()) {%>
				<%= Html.ValidationSummary(true) %>
				
				<fieldset>
						<legend>Fields</legend>
						
						<div class="editor-label">
								<%= Html.LabelFor(model => model.ModelObject.CustomerNo)%>
						</div>
						<div class="display-field"><%= Html.Encode(Model.ModelObject.CustomerNo)%></div>
						
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
								<%= Html.HasPermission(AuthorizationActions.ReadProperty, Model.ModelObject, Customer.ZipcodeProperty, Html.TextBoxFor(model => model.ModelObject.Zipcode), MvcHtmlString.Create("no access")) %>
								<%= Html.ValidationMessageFor(model => model.ModelObject.Zipcode)%>
						</div>
						
						<p>
								<input type="submit" value="Save" />
						</p>
				</fieldset>

		<% } %>

		<div>
				<%= Html.ActionLink("Back to List", "Index") %> |
        <%= Ajax.ActionLink("Ajax update", "TestAction", "Customers", new AjaxOptions { HttpMethod = "Post", OnSuccess = "handleUpdate", OnFailure = "handleError" })%>
		</div>

    <div id="action-message"></div>

</asp:Content>

