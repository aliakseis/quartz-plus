using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ILCWebApplication.ILCSettings;
using Utils.CompileScripts;

namespace ILCWebApplication.EditForms
{
    public partial class EditFormScript : System.Web.UI.Page
    {
        private const string PAGE_TITLE_EDIT = "Edit Script";
        private const string PAGE_TITLE_ADD = "Add New Script";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.Params["id"];
                // Is script added
                if (string.IsNullOrEmpty(id))
                {
                    Header.Title = PAGE_TITLE_ADD;
                }
                else
                {
                    Header.Title = PAGE_TITLE_EDIT;

                    DetailScriptData data = IlcWebDao.GetDetailScriptData(Request.Params["Id"]);

                    tbName.Text = data.name;
                    tbScriptingExpression.Text = data.scriptingExspression;
                }
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            DetailScriptData data = new DetailScriptData();

            data.name = tbName.Text;
            data.scriptingExspression = tbScriptingExpression.Text;

            string id = Request.Params["id"];
            // Is server added
            if (string.IsNullOrEmpty(id))
            {
                id = IlcWebDao.InsertDetailScriptData(data, User).ToString();
            }
            else
            {
                data.scriptId = id;
                IlcWebDao.UpdateDetailScriptData(data, User);
            }

            Response.Write("<script type=\"text/javascript\"> window.opener.parent.__doPostBack('',''); self.close(); </script>");
        }

        protected void OnNameValidate(object source, ServerValidateEventArgs args)
        {
            string id = Request.Params["id"];
            if (string.IsNullOrEmpty(id))
                id = "-1";
            args.IsValid = !IlcWebDao.IsScriptNameExist(args.Value, id);
        }

        protected void OnScriptingValidate(object source, ServerValidateEventArgs args)
        {
            ScenarioItem item = new ScenarioItem();
            item.scriptingExpression = args.Value;
            item.id = 0;
            List<ScenarioItem> list = new List<ScenarioItem>();
            list.Add(item);
            try
            {
                ScriptsCompiling.CompileAssembly(list, null);
            }
            catch (CompileErrorException e)
            {
                args.IsValid = false;
                CustomValidator2.Text = e.Message;
            }
        }
    }
}
