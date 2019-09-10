<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CslaMVC.Library.OrdersList>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Orders Index</h2>

    <table>
        <tr>
            <th></th>
            <th>
                CustomerNo
            </th>
            <th>
                OrderId
            </th>
            <th>
                OrderDate
            </th>
            <th>
                Status
            </th>
            <th>
                ShippedDate
            </th>
            <th>
                ReceivedDate
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                Edit<%--<%: Html.ActionLink("Edit", "Edit", new { id = item.OrderId })%>--%> |
                <%: Html.ActionLink("Details", "Details", new { id = item.OrderId })%> |
                Delete<%--<%: Html.ActionLink("Delete", "Delete", new { id = item.OrderId })%>--%>
            </td>
            <td>
                <%: item.CustomerNo %>
            </td>
            <td>
                <%: item.OrderId %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.OrderDate) %>
            </td>
            <td>
                <%: item.Status %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.ShippedDate) %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.ReceivedDate) %>
            </td>
        </tr>
    
    <% } %>

    </table>

    <p>
        <%: Html.ActionLink("Create New", "Create", new { id = RouteData.Values["id"] }, null)%> |
        <%: Html.ActionLink("Back to Customer Details", "Details", "Customers", new { id = RouteData.Values["id"] }, null)%>
    </p>

</asp:Content>

