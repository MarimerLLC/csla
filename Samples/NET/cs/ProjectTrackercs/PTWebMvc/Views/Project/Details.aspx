<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProjectTracker.Library.Project>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
        <p>
            Name:
            <%= Html.Encode(Model.Name) %>
        </p>
        <p>
            Started:
            <%= Html.Encode(Model.Started) %>
        </p>
        <p>
            Ended:
            <%= Html.Encode(Model.Ended) %>
        </p>
        <p>
            Description:
            <%= Html.Encode(Model.Description) %>
        </p>
    </fieldset>
    <p>
        <%=Html.ActionLink("Edit", "Edit", new { id=Model.Id }) %> |
        <%=Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

