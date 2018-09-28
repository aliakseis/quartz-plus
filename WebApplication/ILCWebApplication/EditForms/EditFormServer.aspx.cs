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
using ILCWebApplication.ILCSettings;
using Quartz;

namespace ILCWebApplication.EditForms
{
    public partial class EditFormServer1 : System.Web.UI.Page
    {
        private const string PAGE_TITLE_EDIT = "Edit Server";
        private const string PAGE_TITLE_ADD = "Add New Server";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.Params["id"];
                // Is server added
                if (string.IsNullOrEmpty(id))
                {
                    Header.Title = PAGE_TITLE_ADD;

                    tbName.Text = string.Empty;
                    tbChannels.Text = "2";
                    tbConnString.Text = string.Empty;
                    cbEnabled.Checked = true;
                    serviceCheckBox.Checked = true;
                    //tbChecker.Text = string.Empty;
                    tbCron.Text = string.Empty;
                }
                else
                {
                    Header.Title = PAGE_TITLE_EDIT;

                    DetailServerData data = IlcWebDao.GetDetailServerData(Request.Params["Id"]);

                    RangeValidator1.Text = string.Format("Enter number from {0} to {1}",
                                                         RangeValidator1.MinimumValue, RangeValidator1.MaximumValue);

                    tbName.Text = data.name;
                    tbChannels.Text = data.channels;
                    tbConnString.Text = data.connectionString;
                    cbEnabled.Checked = (data.enabled == "1");
                    serviceCheckBox.Checked = (!string.IsNullOrEmpty(data.checker));
                    //tbChecker.Text = data.checker;
                    tbCron.Text = data.cron;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            DetailServerData data = new DetailServerData();

            data.name = tbName.Text;
            data.channels = tbChannels.Text;
            data.connectionString = tbConnString.Text;
            data.enabled = cbEnabled.Checked ? "1" : "0";
            //data.checker = checkerList.SelectedItem.Text == "NEW..."
            //                   ? tbChecker.Text.Trim()
            //                   : checkerList.SelectedItem.Text;
            data.checker = serviceCheckBox.Checked ? "AspirinLoginChecker" : "";
            data.cron = tbCron.Text.Trim();

            string id = Request.Params["id"];
            bool isAdded = false;
            // Is server added
            if (string.IsNullOrEmpty(id))
            {
                isAdded = true;
                id = IlcWebDao.InsertDetailServerData(data, User).ToString();
            }
            else
            {
                data.serverId = id;
                IlcWebDao.UpdateDetailServerData(data, User);
            }

            string strEvent = isAdded ? "'treeViewRefresh.server.add'" : "'treeViewRefresh.server.update'";

            string response = "<html><head><script type='text/javascript' src='" +
                              ResolveClientUrl("~/scripts/popup.js") +
                              "'></script></head><body onload=\"javascript:modalHandleClose([" +
                              strEvent + ", '" + id + "," + tbName.Text.Replace("'", "\\'") + 
                              "'])\"></body></html>";

            Response.Write(response);
        }

        protected void OnValidateCron(object source, ServerValidateEventArgs args)
        {
            args.IsValid = CronExpression.IsValidExpression(args.Value);
        }

        //protected void OnAuthCheckersListDataBound(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        checkerList.Items.Insert(0, "");
        //        checkerList.Items.Add("NEW...");
        //        string id = Request.Params["id"];
        //        if (!string.IsNullOrEmpty(id))
        //            checkerList.SelectedValue = tbChecker.Text;
        //    }
        //}
    }
}
