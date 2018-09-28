using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ILCWebApplication
{
    public partial class Validation_settings : System.Web.UI.Page
    {
        protected string iFrameSrc = "Grids/ServersGrid.aspx";

        protected void Page_Init(object sender, EventArgs e)
        {
            Title = "Validation Settings - " + ConstExpressions.GetWebApplicationName();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlGenericControl body = (HtmlGenericControl) Master.FindControl("body");
            body.Attributes.Add("onbeforeunload", "saveScroll();");
            body.Attributes.Add("onunload", "saveScroll();");
            string onloadAttr = "document.getElementById('treeContainer').style.visibility='visible';";
            if (IsPostBack)
                onloadAttr += "getPrevScroll();";
            body.Attributes.Add("onload", onloadAttr);

            if(IsPostBack)
            {
                string type = Request.Params.Get("__EVENTTARGET");
                if (!type.StartsWith("treeViewRefresh"))
                    return;

                string[] typeParams = type.Split('.');
                bool isServer = typeParams[1] == "server" ? true : false;

                string arg = Request.Params.Get("__EVENTARGUMENT");
                string[] argParams = arg.Split(new char[] {','}, 3);
                string id = argParams[0];
                string parentId = null;
                string name;
                if (isServer)
                {
                    name = argParams[1];
                }
                else
                {
                    parentId = argParams[1];
                    name = argParams[2];
                }

                bool parentChanged = false;
                string selectedId = parentId;
                string parentName = null;
                if (!isServer)
                {
                    TreeNode selectedNode = ValidationTreeView.SelectedNode;
                    if (selectedNode != null)
                    {
                        selectedId = selectedNode.Value;
                        parentName = selectedNode.Text;
                        if (selectedId != parentId)
                        {
                            parentChanged = true;
                            FindNode(selectedNode.ChildNodes, id, selectedNode.ChildNodes.Remove);
                        }
                    }
                }

                iFrameSrc = isServer ? "Grids/ServersGrid.aspx"
                                     : "Grids/ProjectsGrid.aspx?server=" + selectedId + "&name=" + HttpUtility.UrlEncode(parentName);

                if (typeParams[2] == "add" || parentChanged)
                   AddItemToTree(isServer, id, parentId, name);
                else
                   UpdateItemInTree(isServer, id, parentId, name);
            }
        }

        private void AddItemToTree(bool isServer, string id, string parentId, string name)
        {
            TreeNode nodeToAdd = ValidationTreeView.Nodes[0];
            foreach (TreeNode serverNode in ValidationTreeView.Nodes[0].ChildNodes)
            {
                if (isServer)
                {
                    if (serverNode.Value == id)
                        return;
                }
                else
                {
                    if (serverNode.Value == parentId)
                    {
                        if (FindNode(serverNode.ChildNodes, id, null))
                            return;

                        nodeToAdd = serverNode;
                        break;
                    }
                }
            }

            if (nodeToAdd.PopulateOnDemand)
                return;

            string strNavigateUrl = isServer
                ? "javascript:LoadGridView('Grids/ProjectsGrid.aspx?server=" + id + "');"
                : "javascript:LoadGridView('Grids/ItemsGrid.aspx?project=" + id + "');";

            nodeToAdd.ChildNodes.Add(new TreeNode(name, id, null, strNavigateUrl, null));
        }

        private void UpdateItemInTree(bool isServer, string id, string parentId, string name)
        {
            TreeNode updateNode = null;
            foreach (TreeNode serverNode in ValidationTreeView.Nodes[0].ChildNodes)
            {
                if (isServer)
                {
                    if (serverNode.Value == id)
                    {
                        updateNode = serverNode;
                        break;
                    }
                }
                else if (serverNode.Value == parentId)
                {
                    FindNode(serverNode.ChildNodes, id,
                        delegate(TreeNode projectNode) { updateNode = projectNode; });
                    break;
                }
            }

            if (updateNode != null)
            {
                updateNode.Text = name;
            }
        }

        protected void OnTreeNodeDataBound(object sender, TreeNodeEventArgs e)
        {
            // Set focus on the root
            if (ValidationTreeView.SelectedNode == null)
                ValidationTreeView.Nodes[0].Select();
        }

        private delegate void FindNodeCallback(TreeNode node);

        private static bool FindNode(TreeNodeCollection nodes, string id, FindNodeCallback onMatch)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Value == id)
                {
                    if (onMatch != null)
                        onMatch(node);
                    return true;
                }
            }
            return false;
        }
    }
}
