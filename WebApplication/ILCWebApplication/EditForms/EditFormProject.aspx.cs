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
    public partial class EditFormProject1 : System.Web.UI.Page
    {
        private const string PAGE_TITLE_EDIT = "Edit Project";
        private const string PAGE_TITLE_ADD = "Add New Project";

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string id = Request.Params["id"];
                // Is project added
                if (string.IsNullOrEmpty(id))
                {
                    Header.Title = PAGE_TITLE_ADD;

                    tbName.Text = string.Empty;
                    tbEmails.Text = string.Empty;
                    cbEnabled.Checked = true;
                    tbCron.Text = string.Empty;
                }
                else
                {
                    Header.Title = PAGE_TITLE_EDIT;

                    DetailProjectData data = IlcWebDao.GetDetailProjectData(id);

                    tbName.Text = data.name;
                    tbEmails.Text = data.emails;
                    cbEnabled.Checked = (data.enabled == "1");
                    tbCron.Text = data.cron;
                }
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            DetailProjectData data = new DetailProjectData();

            data.name = tbName.Text;
            data.emails = tbEmails.Text.Trim();
            data.enabled = cbEnabled.Checked ? "1" : "0";
            data.cron = tbCron.Text.Trim();


            string id = Request.Params["id"];
            string parentId = DropDownList1.SelectedValue;
            bool isAdded = false;
            data.serverId = parentId;
            // Is project added
            if (string.IsNullOrEmpty(id))
            {
                isAdded = true;
                id = IlcWebDao.InsertDetailProjectData(data, User).ToString();
            }
            else 
            {
                data.projectId = id;
                IlcWebDao.UpdateDetailProjectData(data, User);
            }

            string strEvent = isAdded ? "'treeViewRefresh.project.add'" : "'treeViewRefresh.project.update'";

            string response = "<html><head><script type='text/javascript' src='" +
                              ResolveClientUrl("~/scripts/popup.js") +
                              "'></script></head><body onload=\"javascript:modalHandleClose([" +
                              strEvent + ", '" + id + "," + parentId + "," + tbName.Text.Replace("'", "\\'") + 
                              "'])\"></body></html>";

            Response.Write(response);
        }

        protected void OnValidateCron(object source, ServerValidateEventArgs args)
        {
            args.IsValid = CronExpression.IsValidExpression(args.Value);
        }

        protected void OnServersListDataBound(object sender, EventArgs e)
        {
            if (!IsPostBack) // for any case
                DropDownList1.SelectedValue = Request.Params["parentId"];
        }

        protected void OnValidateEmails(object source, ServerValidateEventArgs args)
        {
            string[] emails = args.Value.Split(',');
            foreach (string email in emails)
            {
                if (!Utils.SmtpEmailValidator.IsValidAddress(email, ConfigurationManager.AppSettings["EmailServerName"], ConfigurationManager.AppSettings["EmailFromAddress"]))
                {
                    args.IsValid = false;
                    return;
                }
            }

            args.IsValid = true;
        }
    }
}
