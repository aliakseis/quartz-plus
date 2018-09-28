<%@ Import Namespace="ILCWebApplication" %>

<%@ Page Language="C#" MasterPageFile="./GridPage.Master" AutoEventWireup="true"
    Codebehind="ItemsGrid.aspx.cs" Inherits="ILCWebApplication.ItemsGrid1" Title="Untitled Page" %>

<%@ OutputCache Location="None" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderTitleCell" runat="server">
    <%= Request.Params["name"] + " Users:"%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderButtonCell" runat="server">
    <asp:Button ID="addButton" runat="server" OnClientClick='<%$ Code:  "showPopup(\"../EditForms/EditFormItem.aspx?parentId=" + Request.Params["project"] + "\",\"\", 640, 420); return false;" %>'
        Text="Add" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderPageBody" runat="server">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
        DataKeyNames="ITEM_ID" CssClass="tblStd" EnableViewState="False">
        <SelectedRowStyle BackColor="#C0FFFF" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" ImageUrl="~/images/icon_edit.gif" Text="Edit"
                        NavigateUrl='<%# "javascript:showPopup(\"../EditForms/EditFormItem.aspx?Id=" + Eval("ITEM_ID") + "\", \"EditFormItem\", 640, 420)" %>'> </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TNUMBER" HeaderText="Phone" />
            <asp:BoundField DataField="USERID" HeaderText="Login" />
            <asp:BoundField DataField="PWD" HeaderText="Password" />
            <asp:CheckBoxField DataField="ENBL" HeaderText="Enabled" />
        </Columns>
        <RowStyle CssClass="dark" />
        <AlternatingRowStyle CssClass="light" />
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ Code: WebSettings.GetConnectionString() %>'
        ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand='<%$ Code: "SELECT ITEM_ID, TNUMBER, USERID, PWD," + ConstExpressions.GetCheckExpression("ENABLED", "0") + " AS ENBL FROM " + WebSettings.GetSchemaName() + ".SERVICE_INFO WHERE (IVR_PROJECT_ID = :PARAM1)" %>'>
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="" Name="PARAM1" QueryStringField="project" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
