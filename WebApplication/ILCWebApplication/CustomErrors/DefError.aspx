<%@ Page Language="C#" MasterPageFile="~/ilc.Master" AutoEventWireup="true" Codebehind="DefError.aspx.cs"
    Inherits="ILCWebApplication.DefError" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Application Performed Incorrect Operation on
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    Page.
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderBody" runat="server">
    <div style="height: 435px;">
        <div style="padding-top: 20px; padding-left: 20px;">
            <% 
                if (Context.Error is HttpRequestValidationException)
                {
            %>
            <span style="font-weight:bold">Potentially malicious client input detected.</span> 
                        
            <br />            
            <br />
            Go back and correct input.
            <% 
                } 
                else
                {
            %>
            Please contact your system administrator.
            <%
                }
            %>
        </div>
    </div>
</asp:Content>
