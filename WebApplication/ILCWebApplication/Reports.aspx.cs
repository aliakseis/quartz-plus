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
using System.Text;
using ILCWebApplication.ILCSettings;
using System.Globalization;

namespace ILCWebApplication
{
    public partial class Reports : System.Web.UI.Page
    {
        private const string ReportDataSourceSessionKey = "ReportDataSource";

        protected void Page_Init(object sender, EventArgs e)
        {
            Title = "Reports - " + ConstExpressions.GetWebApplicationName();
            if (IsPostBack)
            {
                DataTable reportDataSource = (DataTable)Session[ReportDataSourceSessionKey];
                if (reportDataSource != null)
                {
                    // Should be performed in Page_Init 
                    // to avoid resetting of parameters coming from request
                    CrystalReportSource1.ReportDocument.SetDataSource(reportDataSource);
                    crystalReportViewer.DataBind();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                IFormatProvider culture = new CultureInfo("en-US", true);

                startDate.Text = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy", culture);
                startTime.Text = "12:00 AM";

                endDate.Text = DateTime.Now.ToString("MM/dd/yyyy", culture);
                endTime.Text = "12:00 AM";
            }
        }

        private void MakeReport()
        {
            List<KeyValuePair<string, object>> queryParams = new List<KeyValuePair<string, object>>();

            AddParams(queryParams
                  , "IVR_SERVER_ID",    server.SelectedValue
                  , "IVR_PROJECT.NAME", projectName.Text
                  , "TNUMBER",          phoneNumber.Text
                  , "USERID",           userId.Text
                  , "STATUS",           status.SelectedValue
            );

            IFormatProvider culture = new CultureInfo("en-US", true);

            IList<SummaryReportItem> reportData = IlcWebDao.GetSummaryReportData(
                DateTime.Parse(startDate.Text + " " + startTime.Text, culture, DateTimeStyles.AllowWhiteSpaces),
                DateTime.Parse(endDate.Text + " " + endTime.Text, culture, DateTimeStyles.AllowWhiteSpaces),
                queryParams);

            DataSet dst = new DataSet();
            dst.ReadXmlSchema(Server.MapPath("Report.xsd"));

            foreach (SummaryReportItem item in reportData)
            {
                int checkSuccess = item.checksCount - item.failedChecksCount;

                dst.Tables[0].Rows.Add(
                    item.projectName,
                    item.serverName,
                    item.phone,
                    string.IsNullOrEmpty(item.userId) ? "n/a" : item.userId,
                    item.checksCount,
                    checkSuccess,
                    item.failedChecksCount,
                    string.Format("{0:P1}", (float)checkSuccess / item.checksCount),
                    item.lastSuccessDate,
                    item.lastFailedDate
                );
            }

            DataTable reportDataSource = dst.Tables[0];
            Session[ReportDataSourceSessionKey] = reportDataSource;

            CrystalReportSource1.ReportDocument.SetDataSource(reportDataSource);
            crystalReportViewer.DataBind();
        }

        protected void runButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                MakeReport();
            }
        }

        protected void OnServersListDataBound(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                server.Items.Insert(0, new ListItem("ALL", ""));
                server.SelectedIndex = 0;
            }
        }

        protected static void AddParams(List<KeyValuePair<string, object>> queryParams, 
                                    params string[] nameValuePairs)
        {
            int i = 0;
            while (i < nameValuePairs.Length)
            {
                string name = nameValuePairs[i++];
                string value = nameValuePairs[i++];

                if (!string.IsNullOrEmpty(value))
                {
                    queryParams.Add(new KeyValuePair<string, object>(name, value));
                }
            }
        }

        protected void crystalReportViewer_Error(object source, CrystalDecisions.Web.ErrorEventArgs e)
        {
            string referer = Request.Headers["Referer"];
            
            if (referer != null && !referer.Contains("/Reports.aspx"))
            {
                //e.Handled = true;

                Context.Items["ErrorDesc"] = e.ErrorMessage;
                //Server.Transfer("~/CustomErrors/DefFrameError.aspx", false);
            }
            
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            string errorDesc = Context.Items["ErrorDesc"] as string;
            if (errorDesc != null)
            {
                Server.Transfer("~/CustomErrors/DefFrameError.aspx", false);                
            }
            // PreRender code
            base.OnPreRender(e);
        }

    }
}
