<%@ Page Language="C#" MasterPageFile="~/ilc.Master" AutoEventWireup="true" CodeBehind="Validation_settings.aspx.cs" Inherits="ILCWebApplication.Validation_settings" %>
<%@ Register Namespace="ILCWebApplication.ValidationInfoDS" TagPrefix="ILCWA" Assembly="ILCWebApplication" %>
<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Validation Settings
</asp:Content>
<asp:Content ID="ContentBody" ContentPlaceHolderID="PlaceHolderBody" runat="server">
<div style="height:435px">
    <table style="width: 100%">
        <tr>
            <td valign="top" class="VRightDivider">    
            <div id="treeContainer"  style="visibility:hidden">            
                <asp:Panel ID="Panel1" runat="server" Height="428px" ScrollBars="Auto" Width="220px">
                
                <asp:TreeView ID="ValidationTreeView" runat="server" DataSourceID="ValidationDataSource" ExpandDepth="0" OnTreeNodeDataBound="OnTreeNodeDataBound" >                                   
                    <SelectedNodeStyle Font-Bold="True"/>
                </asp:TreeView>
                <ILCWA:ValidationInfoDataSource runat="server" id="ValidationDataSource">
                </ILCWA:ValidationInfoDataSource>
                </asp:Panel>
            </div>
            </td>
            <td style="width:580px" valign="top">
                <iframe name="IFrame1" id="IFrame1" style="width: 100%" frameborder="0" height="428" src="<%= iFrameSrc %>"></iframe>
            </td>
        </tr>
    </table>
</div>
<script type="text/javascript" src="<%= ResolveClientUrl("~/scripts/validation_settings_lib.js") %>" >
</script>
<script type="text/javascript">
    function LoadGridView(dataPath)
    {   
    	var gridFrame = document.getElementById("IFrame1");    	
        var oldPath = gridFrame.src;
        var index = oldPath.indexOf('&');
        if (index != -1)
            oldPath = oldPath.substring(0, index);
        if (oldPath != dataPath) {
            if (dataPath.indexOf('?') != -1 && document.activeElement)
                dataPath += "&name=" + encodeURIComponent(document.activeElement.innerHTML);
            TreeView_SelectNode(<%= ValidationTreeView.ClientID + "_Data" %>, document.activeElement, document.activeElement.id);
    	    gridFrame.src = dataPath;    	 
    	}
    }
    
	function saveScroll() {
		ySave = document.getElementById("<%= Panel1.ClientID %>").scrollTop;
		document.cookie = "scrollPos<%= Panel1.ClientID %>" + "=" + ySave;
		var iframeBody = window.frames.IFrame1.document.body;
		if (iframeBody) {
		    ySave = iframeBody.scrollTop;
		    document.cookie = "scrollPosIFrame1" + "=" + ySave;
		}
	}

</script>
</asp:Content>


