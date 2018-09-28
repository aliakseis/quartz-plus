using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Common.Logging;
using System.IO;

namespace LineCheckerSrv
{
    /// <summary>
    /// Implements sending emails during line checking
    /// </summary>
    internal static class Emailer
    {
        public static readonly string alertSubjectTempl =
            "{0}: IVRS Lines check is complete on {1}: Successful - {2}, Failed - {3}…";

        public static readonly string generalFailureSubject = "IVRS LineChecker general failure…";

        public static readonly string projectFailureBodyTempl =
            "Failure description: {0}, Phone number: {1}, User ID: {2}, Machine name: {3}";

        public static readonly string projectFailureSubjectTempl = "{0} IVRS LineChecker: Project {1} check failed on {2}…";

        public static readonly string skippedMessageTempl =
            "{0}: IVRS LineChecker: The scheduled check is skipped because a previous check is in-progress <\\eom>";

        public static readonly string startedMessageTempl = "{0}: IVRS LineChecker service started on {1} <\\eom>";
        public static readonly string stoppedMessageTempl = "{0}: IVRS LineChecker service stopped on {1} <\\eom>";

        public static readonly string weeklyReportSubjectTempl = "ILC SUMMARY REPORT FOR {0} - {1}";


        /// <summary>
        /// Sends email with attachment
        /// </summary>
        /// <param name="mailBody">email body</param>
        /// <param name="mailSubject">email subject</param>
        /// <param name="emailsString">email addresses</param>
        /// <param name="attachmentPaths">paths to attached files</param>
        /// <returns>return true if email was sended</returns>
        public static bool SendEmail(string mailBody, string mailSubject, string emailsString,
                                     params string[] attachmentPaths)
        {
            ILog log = LogManager.GetLogger(AppSettings.GetCommonLoggerName());
            try
            {
                SmtpClient client = new SmtpClient(AppSettings.GetEmailServerName());

                List<string> addresses = new List<string>();
                ParseEmailsString(emailsString, addresses);

                if (addresses.Count == 0)
                    return false;

                using (MailMessage message = new MailMessage(AppSettings.GetEmailFromAddress(), addresses[0]))
                {
                    for (int i = 1; i < addresses.Count; ++i)
                    {
                        message.To.Add(addresses[i]);
                    }

                    message.Body = mailBody;
                    message.Subject = mailSubject;

                    foreach (string str in attachmentPaths)
                    {
                        if (File.Exists(str))
                            message.Attachments.Add(new Attachment(str));
                        else
                            log.Info("File " + str + " does not exist");
                    }

                    client.UseDefaultCredentials = false;
                    NetworkCredential theCredential = new NetworkCredential(
                        AppSettings.GetEmailClientLogin(), AppSettings.GetEmailClientPassword());
                    client.Credentials = theCredential;

                    log.Info("SendEmail: sending email to: " + emailsString);
                    client.Send(message);
                }
            }
            catch (FormatException e)
            {
                log.Error("SendEmail", e);
                return false;
            }
            catch (ArgumentException e)
            {
                log.Error("SendEmail", e);
                return false;
            }
            catch (InvalidOperationException e)
            {
                log.Error("SendEmail", e);
                return false;
            }
            catch (SmtpException e)
            {
                log.Error("SendEmail", e);
                return false;
            }

            return true;
        }

        private static void ParseEmailsString(string strEmail, ICollection<string> emailList)
        {
            if (strEmail != null)
            {
                string[] split = strEmail.Split(',');
                foreach (string str in split)
                {
                    string strTemp = str.Trim();
                    if (strTemp.Length != 0)
                        emailList.Add(strTemp);
                }
            }
        }
    }
}
