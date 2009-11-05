<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<ProjectTracker.Library.ProjectInfo>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Projects
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Projects</h2>

    <table>
        <tr>
            <th></th>
            <th>
                Name
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <% if ((bool)ViewData["CanEdit"]) { %>
                  <%= Html.ActionLink("Edit", "Edit", new { id=item.Id }) %> |
                <% } %>
                <%= Html.ActionLink("Details", "Details", new { id = item.Id })%>
            </td>
            <td>
                <%= Html.Encode(item.Name) %>
            </td>
        </tr>
    
    <% } %>

    </table>

    <% if ((bool)ViewData["CanEdit"]) { %>
      <p>
          <%= Html.ActionLink("Create New", "Create") %>
      </p>
    <% } %>

</asp:Content>

