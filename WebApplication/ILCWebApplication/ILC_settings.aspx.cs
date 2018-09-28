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

namespace ILCWebApplication
{
    public partial class ILC_settings : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            Title = "ILC Settings - " + ConstExpressions.GetWebApplicationName();

            foreach (object validator in Validators)
            {
                if (validator is RangeValidator)
                    SetRangeValidatorText((RangeValidator)validator);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetIlcSettingsData();
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            IlcSettingsData data = new IlcSettingsData();

            data.scheduleCron = scheduleCron.Text;
            data.outgoingChannels = outgoingChanels.Text = outgoingChanels.Text.Trim();
            data.timeSpan = timeSpan.Text = timeSpan.Text.Trim();
            data.commonRecipientEmailAddresses = commonEmail.Text = commonEmail.Text;
            data.timeSpanBetweenVerifications = timeBetweenVerifications.Text = timeBetweenVerifications.Text.Trim();
            data.attemptsMaxNumber = maxAttempts.Text = maxAttempts.Text.Trim();
            data.emailFromAddress = fromEmail.Text;
            data.summaryReportCron = summaryReportCron.Text = summaryReportCron.Text.Trim();
            data.summaryRecipientEmail = summaryEmail.Text;
            data.jobMisfireThreshold = jobMisfireThreshold.Text = jobMisfireThreshold.Text.Trim();

            IlcWebDao.UpdateIlcSettings(data, User);
        }

        private static void SetRangeValidatorText(RangeValidator validator)
        {
            validator.Text = string.Format("Enter number from {0} to {1}", validator.MinimumValue, validator.MaximumValue);
        }

        protected void OnValidateCron(object source, ServerValidateEventArgs args)
        {
            args.IsValid = CronExpression.IsValidExpression(args.Value);
        }

        private void GetIlcSettingsData()
        {
            IlcSettingsData data = IlcWebDao.GetIlcSettingsData();
            scheduleCron.Text = data.scheduleCron;
            outgoingChanels.Text = data.outgoingChannels;
            timeSpan.Text = data.timeSpan;
            commonEmail.Text = data.commonRecipientEmailAddresses;
            timeBetweenVerifications.Text = data.timeSpanBetweenVerifications;
            maxAttempts.Text = data.attemptsMaxNumber;
            fromEmail.Text = data.emailFromAddress;
            summaryReportCron.Text = data.summaryReportCron;
            summaryEmail.Text = data.summaryRecipientEmail;
            jobMisfireThreshold.Text = data.jobMisfireThreshold;
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
