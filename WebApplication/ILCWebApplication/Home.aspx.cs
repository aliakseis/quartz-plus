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
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            Title = "Home - " + ConstExpressions.GetWebApplicationName();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
