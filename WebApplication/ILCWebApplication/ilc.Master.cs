using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.ServiceProcess;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ILCWebApplication.HandleException;
using ILCWebApplication.ILCSettings;
using System.Reflection;
using System.Diagnostics;

namespace ILCWebApplication
{
    public partial class ilc : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string onloadAttr = body.Attributes["onload"];
            const string StatusTimerSetup = "setInterval('updateStatus()', 10000);";
            if (onloadAttr != StatusTimerSetup)
            {
                body.Attributes.Add("onload", (onloadAttr ?? "") + StatusTimerSetup);
            }
            if (!IsPostBack)
            {
                lblVersionState.Text = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            }
        }

        protected static IlcStatusData GetStatusData()
        {
            try
            {
                return IlcWebDao.GetStatusData();
            }
            catch (Exception ex)
            {
                new ExceptionLogger().HandleException(ex);
                return null;
            }
        }

        protected delegate object FieldCallback(IlcStatusData data);

        protected static string Field(IlcStatusData data, FieldCallback accessor)
        {
            return data == null ? "Error!" : accessor(data).ToString();
        }
    }
}
