<%@ Page Language="C#" MasterPageFile="~/EditForms/EditForm.Master" AutoEventWireup="true" CodeBehind="EditFormScript.aspx.cs" Inherits="ILCWebApplication.EditForms.EditFormScript" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageBody" runat="server">
    <fieldset>
		<legend>
			Server Settings
		</legend>
		<table cellspacing="5">
		<tr>
            <td class="EditLabel">
                <asp:Label ID="Label2" runat="server" Text="Name:"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbName" runat="server" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbName"
                    Display="Dynamic" ErrorMessage="Enter name"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="Such name already exists"
                    OnServerValidate="OnNameValidate" ControlToValidate="tbName"></asp:CustomValidator></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label3" runat="server" Text="Scripting expression:"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbScriptingExpression" runat="server" TextMode="MultiLine" Width="299px" MaxLength="4000" Height="96px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbScriptingExpression"
                    Display="Dynamic" ErrorMessage="Enter expression"></asp:RequiredFieldValidator></td>
        </tr>
		</table>
        <asp:CustomValidator ID="CustomValidator2" runat="server" ControlToValidate="tbScriptingExpression"
            Display="Dynamic" ErrorMessage="CustomValidator" OnServerValidate="OnScriptingValidate"></asp:CustomValidator></fieldset>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderButtons" runat="server">
    <div style="text-align:center">
    <asp:Button ID="Button2" runat="server" OnClick="Save_Click" Text="Save" style="width:60px" />
	<input id="Button4" type="button" value="Cancel" onclick='self.close();' style="width:60px" />
    </div>
</asp:Content>
