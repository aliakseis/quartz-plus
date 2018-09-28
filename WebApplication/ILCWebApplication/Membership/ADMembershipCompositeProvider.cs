using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.Security;

namespace ILCWebApplication.Membership
{
    /// <summary>
    /// Implements custom membership provider using active directory
    /// </summary>
    public class ADMembershipCompositeProvider : MembershipProvider
    {
        private const bool enablePasswordReset = true;
        private const bool enablePasswordRetrieval = true;
        private const int passwordAttemptThreshold = 5;
        private const bool requiresQuestionAndAnswer = false;
        private const bool requiresUniqueEMail = true;
        private NameValueCollection config;
        private MembershipPasswordFormat passwordFormat;
        private List<ActiveDirectoryMembershipProvider> providers;
        private string strApplicationName = "/";
        private string strProviderName;
        private string[] connectionStrings;

        public override string ApplicationName
        {
            get { return strApplicationName; }
            set { strApplicationName = value; }
        }

        public override string Name
        {
            get { return strProviderName; }
        }

        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return passwordAttemptThreshold; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 0; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return ""; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool RequiresUniqueEmail
        {
            get { return requiresUniqueEMail; }
        }

        private void InitializeProviders()
        {
            providers = new List<ActiveDirectoryMembershipProvider>();
            StringBuilder strBuilder = new StringBuilder();

            for (int i = 0; i < connectionStrings.Length; ++i)
            {
                string currentStr = connectionStrings[i].Trim();
                ActiveDirectoryMembershipProvider provider = new ActiveDirectoryMembershipProvider();
                try
                {
                    NameValueCollection currConfig = new NameValueCollection(config);
                    currConfig["connectionStringName"] = currentStr;
                    provider.Initialize(strProviderName, currConfig);
                    providers.Add(provider);
                }
                catch (Exception e)
                {
                    strBuilder.Append(currentStr + ": " + e.Message + "\n");
                    if ((i == connectionStrings.Length - 1) && (providers.Count == 0))
                        throw new Exception(strBuilder.ToString(), e);
                }
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            connectionStrings = config["connectionStringName"].Split(',');

            strProviderName = name;
            passwordFormat = new MembershipPasswordFormat();
            this.config = new NameValueCollection(config);

            InitializeProviders();
        }

        public override bool ValidateUser(string strName, string strPassword)
        {
            try
            {
                return DoValidateUser(strName, strPassword);
            }
            catch (Exception)
            {
                InitializeProviders();
                return DoValidateUser(strName, strPassword);                
            }
        }

        private bool DoValidateUser(string strName, string strPassword)
        {
            for (int i = providers.Count - 1; i >= 0; --i)
            {
                try
                {
                    return providers[i].ValidateUser(strName, strPassword);
                }
                catch (Exception)
                {
                    providers.RemoveAt(i);
                    if (providers.Count == 0)
                        throw;
                }
            }
            throw new Exception("There are no accessible servers");
        }

        public override string GetPassword(string strName, string strAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUser CreateUser(
            string username,
            string password,
            string email,
            string passwordQuestion,
            string passwordAnswer,
            bool isApproved,
            object userId,
            out MembershipCreateStatus status)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetUserNameByEmail(string strEmail)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string ResetPassword(string strName, string strAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool ChangePassword(string strName, string strOldPwd, string strNewPwd)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool ChangePasswordQuestionAndAnswer(string strName, string strPassword,
                                                             string strNewPwdQuestion, string strNewPwdAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUser GetUser(string strName, bool boolUserIsOnline)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool DeleteUser(string strName, bool boolDeleteAllRelatedData)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUserCollection FindUsersByEmail(string strEmailToMatch, int iPageIndex, int iPageSize,
                                                                  out int iTotalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUserCollection FindUsersByName(string strUsernameToMatch, int iPageIndex,
                                                                 int iPageSize, out int iTotalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUserCollection GetAllUsers(int iPageIndex, int iPageSize, out int iTotalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool UnlockUser(string strUserName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}