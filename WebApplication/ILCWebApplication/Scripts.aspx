<%@ Page Language="C#" MasterPageFile="~/ilc.Master" AutoEventWireup="true" CodeBehind="Scripts.aspx.cs" Inherits="ILCWebApplication.WebForm2" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Sripts settings
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderBody" runat="server">
    <iframe name="IFrame1" id="IFrame1" style="width: 100%" frameborder="0"
     height="428" src="<%= ResolveClientUrl("~/Grids/ScriptsGrid.aspx") %>"></iframe>
</asp:Content>
