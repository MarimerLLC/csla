<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CslaMVC.ViewModels.CustomerListViewModel>" %>
<%@ Import Namespace="Csla.Web.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Customers Index</h2>

    <table>
        <tr>
            <th></th>
            <th>
                CustomerNo
            </th>
            <th>
                Name
            </th>
            <th>
                GroupNo
            </th>
            <th>
                City
            </th>
            <th>
                State
            </th>
            <th>
                Zipcode
            </th>

        </tr>

    <% foreach (var item in Model.Customers)
       { %>
    
        <tr>
            <td>
                <%= Html.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(CslaMVC.Library.Customer), Html.ActionLink("Edit", "Edit", new { id = item.CustomerNo }), "Edit")%> |
                <%= Html.ActionLink("Details", "Details", new { id = item.CustomerNo })%> |
                <%= Html.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(CslaMVC.Library.Customer), Html.ActionLink("Delete", "Delete", new { id = item.CustomerNo }), "Delete")%>
            </td>
            <td>
                <%= Html.Encode(item.CustomerNo) %>
            </td>
            <td>
                <%= Html.Encode(item.Name) %>
            </td>
            <td>
                <%= Html.Encode(item.GroupNo) %>
            </td>
            <td>
                <%= Html.Encode(item.City) %>
            </td>
            <td>
                <%= Html.Encode(item.State) %>
            </td>
            <td>
                <%= Html.Encode(item.Zipcode) %>
            </td>
        </tr>
    
    <% } %>

    </table>

    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>

