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
    public partial class _04Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Label1.Text = Request.Params["aspxerrorpath"];
                if (Request.UrlReferrer != null)
                {
                    HyperLink1.NavigateUrl = Request.UrlReferrer.ToString();
                    Label2.Visible = false;
                }
                else
                {
                    Label2.Text = "\"Back button\" of your browser";
                    HyperLink1.Visible = false;
                }
                
            }
        }
    }
}
