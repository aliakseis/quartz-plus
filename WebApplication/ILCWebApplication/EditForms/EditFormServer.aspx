<%@ Import Namespace="ILCWebApplication"%>
<%@ Page Language="C#" MasterPageFile="./EditForm.Master" AutoEventWireup="true" CodeBehind="EditFormServer.aspx.cs" Inherits="ILCWebApplication.EditForms.EditFormServer1" Title="Server Details" %>
<%@ OutputCache Location="None" %>
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
                    ErrorMessage="*"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label3" runat="server" Text="Number of channels:" Width="125px"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbChannels" runat="server" MaxLength="3"></asp:TextBox>&nbsp;
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbChannels"
                    ErrorMessage="*"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="tbChannels"
                    ErrorMessage="Enter number from 1 to 999" MaximumValue="999" MinimumValue="1" Type="Integer"></asp:RangeValidator></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label4" runat="server" Text="Connection string:"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbConnString" runat="server" TextMode="MultiLine" Width="305px" MaxLength="2000"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbConnString"
                    ErrorMessage="*"></asp:RequiredFieldValidator></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label5" runat="server" Text="Enabled:"></asp:Label></td>
            <td>
                <asp:CheckBox ID="cbEnabled" runat="server" /></td>
        </tr>
        <tr>
            <td class="EditLabel">
                <%--asp:Label ID="Label6" runat="server" Text="Authentication checker:"></asp:Label--%>
                <asp:Label ID="Label6" runat="server" Text="Aspirin service:"></asp:Label>
            </td>
            <td>
                <%--asp:DropDownList ID="checkerList" onChange="javascript:ShowTextField();" DataTextField="AUTH_CHECKER"
                 DataSourceID="SqlDataSource1" OnDataBound="OnAuthCheckersListDataBound" runat="server"></asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                ConnectionString='<%$ Code: WebSettings.GetConnectionString() %>'
                ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>"
                SelectCommand='<%$ Code: "SELECT DISTINCT AUTH_CHECKER FROM " + WebSettings.GetSchemaName() + ".IVR_SERVER where not (AUTH_CHECKER is null)" % >'
                /--%>
                <%--asp:TextBox ID="tbChecker" runat="server" MaxLength="50" style="display:none"></asp:TextBox--%>
                <asp:CheckBox ID="serviceCheckBox" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="EditLabel">
                <asp:Label ID="Label7" runat="server" Text="Cron:"></asp:Label></td>
            <td>
                <asp:TextBox ID="tbCron" runat="server" MaxLength="50"></asp:TextBox>
                <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="tbCron" ErrorMessage="Incorrect Cron Expression"
                    OnServerValidate="OnValidateCron"></asp:CustomValidator></td>
        </tr>
    </table>
    </fieldset>
    
<%--script type="text/javascript">
    function ShowTextField() 
    { 
    var a = document.getElementById("<%= checkerList.ClientID %>").value;
    if (a == "NEW...") { 
    document.getElementById("<%= tbChecker.ClientID %>").style.display="";
    document.getElementById("<%= tbChecker.ClientID %>").value = ""; 
    document.getElementById("<%= checkerList.ClientID %>").style.display="none";
    }
    }

</script--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderButtons" runat="server">
    <div style="text-align:center">
    <asp:Button ID="Button2" runat="server" OnClick="Button1_Click" Text="Save" style="width:60px" />
	<input id="Button4" type="button" value="Cancel" onclick='self.close();' style="width:60px" />
    </div>
</asp:Content>
