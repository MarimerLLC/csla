<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PTWebMvc.Views.Account.Login" %>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Login</h2>
    <p>
        Please enter your username and password below.
    </p>
    <%
        IList<string> errors = ViewData["errors"] as IList<string>;
        if (errors != null) {
            %>
                <ul class="error">
                <% foreach (string error in errors) { %>
                    <li><%= Html.Encode(error) %></li>
                <% } %>
                </ul>
            <%
        }
         %>
    <form method="post" action="<%= Html.AttributeEncode(Url.Action("Login")) %>">
        <div>
            <table>
                <tr>
                    <td>Username:</td>
                    <td><%= Html.TextBox("username") %></td>
                </tr>
                <tr>
                    <td>Password:</td>
                    <td><%= Html.Password("password") %></td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="checkbox" name="rememberMe" value="true" /> Remember me?</td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="submit" value="Login" /></td>
                </tr>
            </table>
        </div>
    </form>
</asp:Content>
