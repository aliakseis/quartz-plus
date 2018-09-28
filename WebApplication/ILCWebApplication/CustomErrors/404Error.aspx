<%@ Page Language="C#" MasterPageFile="~/ilc.Master" AutoEventWireup="true" Codebehind="404Error.aspx.cs"
    Inherits="ILCWebApplication._04Error" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Error 404: Page
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    does not exist.
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderBody" runat="server">
    <div style="height: 435px;">
        <div style="padding-top: 20px; padding-left: 20px;">
            Click
            <asp:HyperLink ID="HyperLink1" runat="server">here</asp:HyperLink>
            <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
            to return back.
        </div>
    </div>
</asp:Content>
