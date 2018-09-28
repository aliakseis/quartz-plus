using System;
using System.Collections.Specialized;
using System.Web.Security;

namespace ILCWebApplication.Membership
{
    /// <summary>
    /// Implements custom membership provider using ASPIRIN.AUTHUSERS table
    /// </summary>
    public class LoginMembershipProvider : MembershipProvider
    {
        private const bool enablePasswordReset = true;
        private const bool enablePasswordRetrieval = true;
        private const int passwordAttemptThreshold = 5;
        private MembershipPasswordFormat passwordFormat;
        private const bool requiresQuestionAndAnswer = false;
        private const bool requiresUniqueEMail = true;
        private string strApplicationName = "/";
        private string strProviderName;

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

        public override void Initialize(string name, NameValueCollection config)
        {
            strProviderName = name;
            passwordFormat = new MembershipPasswordFormat();
        }

        public override bool ValidateUser(string strName, string strPassword)
        {
            return IlcWebDao.IsUserExist(strName, strPassword);
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