<%@ Page Language="C#" MasterPageFile="~/CustomErrors/FrameErrorPage.Master" AutoEventWireup="true"
    Codebehind="DefFrameError.aspx.cs" Inherits="ILCWebApplication.CustomErrors.DefFrameError"
    Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Application Performed Incorrect Operation on
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    Page.
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderBody" runat="server">
    <% 
        if (Context.Error is HttpRequestValidationException)
        {
    %>
    <span style="font-weight: bold">Potentially malicious client input detected.</span>
    <br />
    <br />
    Go back and correct input.
    <% 
        }
             else if(Context.Items["ErrorDesc"] != null)
                {
                Response.Write(Context.Items["ErrorDesc"]);
                }
        else
        {
    %>
    Please contact your system administrator.
    <%
        }
    %>
</asp:Content>
