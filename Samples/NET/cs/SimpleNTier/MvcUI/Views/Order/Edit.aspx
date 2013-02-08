<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<BusinessLibrary.Order>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
        
        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Id) %>
            </div>
            <div class="editor-field">
                <%: Html.DisplayTextFor(model => model.Id) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CustomerName) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.CustomerName) %>
                <%: Html.ValidationMessageFor(model => model.CustomerName) %>
            </div>
            
            <table>
            <thead>
              <tr><td><b>Id</b></td><td><b>Product</b></td>
            </tr></thead>
            <tbody>
            <% for (int i = 0; i < Model.LineItems.Count; i++) { %>
              <tr><td><%: Html.DisplayTextFor(r => Model.LineItems[i].Id) %></td><td><%: Html.TextBoxFor(r => Model.LineItems[i].Name)%></td></tr>
            <% } %>
            </tbody>
            </table>

            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

