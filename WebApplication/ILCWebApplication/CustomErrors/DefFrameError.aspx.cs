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

namespace ILCWebApplication.CustomErrors
{
    public partial class DefFrameError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Context.Error is HttpRequestValidationException)
                    Label1.Text = Request.FilePath;
                else
                    Label1.Text = Request.Params["aspxerrorpath"];
            }
        }
    }
}
