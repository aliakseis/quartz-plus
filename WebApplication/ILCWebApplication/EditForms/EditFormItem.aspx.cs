using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ILCWebApplication.ILCSettings;

namespace ILCWebApplication.EditForms
{
    public partial class EditFormItem1 : System.Web.UI.Page
    {
        private const string PAGE_TITLE_EDIT = "Edit Item";
        private const string PAGE_TITLE_ADD = "Add New Item";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string parentId = Request.Params["parentId"];
                // Is item added
                if (!string.IsNullOrEmpty(parentId))
                {
                    Header.Title = PAGE_TITLE_ADD;

                    tbPhone.Text = string.Empty;
                    tbLogin.Text = string.Empty;
                    tbPassword.Text = string.Empty;
                    cbEnabled.Checked = true;

                    return;
                }

                string id = Request.Params["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    Header.Title = PAGE_TITLE_EDIT;


                    DetailItemData data = IlcWebDao.GetDetailItemData(id);

                    tbPhone.Text = data.phoneNumber;
                    tbLogin.Text = data.login;
                    tbPassword.Text = data.password;
                    cbEnabled.Checked = (data.enabled == "1");

                    return;
                }

                throw new ApplicationException("Unexpected query string");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            DetailItemData data = new DetailItemData();

            data.phoneNumber = tbPhone.Text.Trim();
            data.login = tbLogin.Text.Trim();
            data.password = tbPassword.Text.Trim();
            data.enabled = cbEnabled.Checked ? "1" : "0";

            string id = Request.Params["id"];
            string parentId = Request.Params["parentId"];
            // Is item added
            if (!string.IsNullOrEmpty(parentId))
            {
                data.projectId = parentId;
                IlcWebDao.InsertDetailItemData(data, User);
            }
            else if (!string.IsNullOrEmpty(id))
            {
                data.itemId = id;
                IlcWebDao.UpdateDetailItemData(data, User);
            }
            else
                throw new ApplicationException("Unexpected page title");

            Response.Write("<script type=\"text/javascript\"> if (window.opener) window.opener.location.reload(1); else returnValue = 'reload'; self.close(); </script>");
        }

    }
}
