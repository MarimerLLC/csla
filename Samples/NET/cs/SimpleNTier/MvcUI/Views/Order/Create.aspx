<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<BusinessLibrary.Order>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>

        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Id) %>
            </div>
            <div class="editor-field">
                <%: Html.DisplayTextFor(model => model.Id)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CustomerName) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.CustomerName) %>
                <%: Html.ValidationMessageFor(model => model.CustomerName) %>
            </div>

            <table>
            <% foreach (var item in Model.LineItems)
               { %>
              <tr>
              <td>
                <div class="editor-label">
                    <%: Html.LabelFor(model => item.Id) %>
                </div>
              </td>
              <td>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => item.Name) %>
                    <%: Html.ValidationMessageFor(model => item.Name)%>
                </div>
              </td>
              </tr>
            <% } %>
            </table>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

