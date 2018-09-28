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
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void OnAuthenticate(object sender, AuthenticateEventArgs e)
        {
            foreach(MembershipProvider provider in System.Web.Security.Membership.Providers)
            {
                if (provider.ValidateUser(Login1.UserName, Login1.Password))
                {
                    e.Authenticated = true;
                    return;
                }
            }
        }
    }
}
