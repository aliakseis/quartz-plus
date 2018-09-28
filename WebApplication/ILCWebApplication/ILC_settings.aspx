<%@ Import Namespace="ILCWebApplication"%>
<%@ Page Language="C#" MasterPageFile="~/ilc.Master" AutoEventWireup="true" CodeBehind="ILC_settings.aspx.cs" Inherits="ILCWebApplication.ILC_settings" %>
<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    ILC Settings
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="PlaceHolderBody" runat="server">
    <asp:Panel ID="Panel1" runat="server" Height="435px" ScrollBars="Auto" Width="800px">
    <fieldset>
    <%--
		<legend>
			ILC Settings
		</legend>
		--%>
    <table cellspacing="1"><tr><td class="EditLabel">
        <asp:Label ID="Label1" runat="server" Text="Schedule Cron:"></asp:Label></td><td>
        <asp:TextBox ID="scheduleCron" runat="server" MaxLength="50"></asp:TextBox>
        <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="scheduleCron" ErrorMessage="Incorrect Cron Expression"
                    OnServerValidate="OnValidateCron" Display="Dynamic"></asp:CustomValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="scheduleCron"
            ErrorMessage="Enter Cron Expression" Display="Dynamic"></asp:RequiredFieldValidator>&nbsp;
        </td></tr><tr><td class="EditLabel">
        <asp:Label ID="Label2" runat="server" Text="Outgoing channels count:"></asp:Label></td><td>
        <asp:TextBox ID="outgoingChanels" runat="server" MaxLength="3"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="outgoingChanels"
            ErrorMessage="RangeValidator" MaximumValue="999" MinimumValue="1" Type="Integer" Display="Dynamic"></asp:RangeValidator></td></tr><tr><td class="EditLabel">
        <asp:Label ID="Label4" runat="server" Text="Time span, sec:"></asp:Label></td><td>
        <asp:TextBox ID="timeSpan" runat="server" MaxLength="5"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="timeSpan"
            ErrorMessage="RangeValidator" MaximumValue="99999" MinimumValue="1" Type="Integer" Display="Dynamic"></asp:RangeValidator></td></tr><tr><td class="EditLabel">
        <asp:Label ID="Label5" runat="server" Text="Common Recipient Emails:"></asp:Label></td><td>
        <asp:TextBox ID="commonEmail" runat="server" TextMode="MultiLine" MaxLength="2000" Width="265px" Height="60px"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="commonEmail"
            ErrorMessage="Enter Emails" Display="Dynamic"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="commonEmail"
            ErrorMessage="Invalid entry" ValidationExpression='<%$ Code: ConstExpressions.MULTI_EMAIL_REGEXP %>' Display="Dynamic"></asp:RegularExpressionValidator>
            <asp:CustomValidator ID="CustomValidator3" runat="server" ControlToValidate="commonEmail"
                ErrorMessage="Invalid entry" OnServerValidate="OnValidateEmails" Display="Dynamic"></asp:CustomValidator></td></tr><tr><td class="EditLabel">
        <asp:Label ID="Label6" runat="server" Text="Time span between verifications, sec:"></asp:Label></td><td>
        <asp:TextBox ID="timeBetweenVerifications" runat="server" MaxLength="5"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="timeBetweenVerifications"
            ErrorMessage="RangeValidator" MaximumValue="99999" MinimumValue="1" Type="Integer" Display="Dynamic"></asp:RangeValidator></td></tr><tr><td class="EditLabel">
        <asp:Label ID="Label7" runat="server" Text="Max attempts:"></asp:Label></td><td>
        <asp:TextBox ID="maxAttempts" runat="server" MaxLength="3"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator4" runat="server" ControlToValidate="maxAttempts"
            ErrorMessage="RangeValidator" MaximumValue="999" MinimumValue="1" Type="Integer" Display="Dynamic"></asp:RangeValidator>
        </td></tr><tr><td class="EditLabel">
        <asp:Label ID="Label8" runat="server" Text="From Email:"></asp:Label></td><td>
        <asp:TextBox ID="fromEmail" runat="server" MaxLength="2000" Width="265px"></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="fromEmail"
            ErrorMessage="Invalid entry" ValidationExpression='<%$ Code: ConstExpressions.SINGLE_EMAIL_REGEXP %>' Display="Dynamic"></asp:RegularExpressionValidator>
        </td></tr><tr><td class="EditLabel">
        <asp:Label ID="Label12" runat="server" Text="Summary Report Cron:"></asp:Label></td><td>
        <asp:TextBox ID="summaryReportCron" runat="server" MaxLength="50"></asp:TextBox>
        <asp:CustomValidator ID="CustomValidator2" runat="server" ControlToValidate="summaryReportCron" ErrorMessage="Incorrect Cron Expression"
                    OnServerValidate="OnValidateCron" Display="Dynamic"></asp:CustomValidator>
        </td></tr><tr><td class="EditLabel">
        <asp:Label ID="Label13" runat="server" Text="Summary Recipient Emails:"></asp:Label></td><td>
        <asp:TextBox ID="summaryEmail" runat="server" TextMode="MultiLine" MaxLength="2000" Width="265px" Height="60px"></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="summaryEmail"
            ErrorMessage="Invalid entry" ValidationExpression='<%$ Code: ConstExpressions.MULTI_EMAIL_REGEXP %>' Display="Dynamic"></asp:RegularExpressionValidator>
            <asp:CustomValidator ID="CustomValidator4" runat="server" ControlToValidate="summaryEmail"
                ErrorMessage="Invalid entry" OnServerValidate="OnValidateEmails" Display="Dynamic"></asp:CustomValidator></td></tr><tr><td class="EditLabel">
        <asp:Label ID="Label14" runat="server" Text="Job  misfire threshold, sec:"></asp:Label></td><td>
        <asp:TextBox ID="jobMisfireThreshold" runat="server" MaxLength="5"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator8" runat="server" ControlToValidate="jobMisfireThreshold"
            ErrorMessage="RangeValidator" MaximumValue="99999" MinimumValue="1" Type="Integer" Display="Dynamic"></asp:RangeValidator>
        </td></tr></table>
        </fieldset>
         <div align="center">
                <asp:Button ID="saveButton" runat="server" OnClick="SaveButton_Click" Text="Save"  style="width:60px"/>
                <input id="cancelButton" type="button" value="Cancel" onclick='window.location="ILC_settings.aspx"' style="width:60px"/>
           </div>
        </asp:Panel>
</asp:Content>
