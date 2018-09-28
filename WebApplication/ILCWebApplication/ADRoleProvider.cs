using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.DirectoryServices;
using System.Text;
using System.Web.Security;

namespace ILCWebApplication
{
    /// <summary>
    /// Custom Active Directory LDAP role provider based on users’ "memberOf" AD properties
    /// </summary>
    public class ADRoleProvider : RoleProvider
    {
        private string connectionStringName;
        private string connectionUsername;
        private string connectionPassword;
        private string attributeMapUsername;

        #region Overrides of RoleProvider

        public override void Initialize(string name, NameValueCollection config)
        {
            connectionStringName = config["connectionStringName"];
            connectionUsername = config["connectionUsername"];
            connectionPassword = config["connectionPassword"];
            attributeMapUsername = config["attributeMapUsername"];

            if (string.IsNullOrEmpty(connectionStringName))
            {
                throw new ProviderException("ADRoleProvider: Connection string name must not be empty");
            }

            if ((connectionUsername != null) && (connectionUsername.Length == 0))
            {
                throw new ProviderException("ADRoleProvider: Connection username must not be empty");
            }

            if ((connectionPassword != null) && (connectionPassword.Length == 0))
            {
                throw new ProviderException("ADRoleProvider: Connection password must not be empty");
            }
            if (((connectionUsername != null) && (connectionPassword == null)) || ((connectionPassword != null) && (connectionUsername == null)))
            {
                throw new ProviderException("ADRoleProvider: Username and password required");
            }


            base.Initialize(name, config);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            string[] roles = GetRolesForUser(username);

            foreach (string role in roles)
            {
                if (role.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] connectionStrings = connectionStringName.Split(',');
            StringBuilder strBuilder = new StringBuilder();

            for(int i = 0; i < connectionStrings.Length; ++i)
            {
                string currentStr = connectionStrings[i].Trim();
                try
                {
                    return GetRolesForUser(username, ConfigurationManager.ConnectionStrings[currentStr].ConnectionString);
                }
                catch (System.Exception e)
                {
                    strBuilder.Append(currentStr + ": " + e.Message + "\n");
                    if (i == connectionStrings.Length - 1)
                        throw new Exception(strBuilder.ToString(), e);
                }
            }

            throw new ProviderException("ADRoleProvider: invalid provider configuration");
        }

        private string[] GetRolesForUser(string username, string connectionString)
        {

            using (
                DirectoryEntry rootEntry =
                    new DirectoryEntry(connectionString, connectionUsername, connectionPassword))
            {
                rootEntry.RefreshCache();

                //Search the user in the directory service
                using (DirectorySearcher searcher = new DirectorySearcher(rootEntry))
                {
                    searcher.PropertiesToLoad.Add("memberOf");
                    searcher.PropertiesToLoad.Add(attributeMapUsername);

                    searcher.Filter = String.Format("(&(objectClass=user)({0}={1}))", attributeMapUsername, username);
                    SearchResult result = searcher.FindOne();
                    if (result == null)
                        return new string[0];

                    DirectoryEntry userEntry = result.GetDirectoryEntry();

                    string[] roles = null;

                    PropertyValueCollection property = userEntry.Properties["memberOf"];
                    if (property.Value is Array)
                    {
                        Array values = (Array)property.Value;
                        roles = new string[values.Length];
                        values.CopyTo(roles, 0);
                    }
                    else if (property.Value is string)
                    {
                        roles = new string[1];
                        roles[0] = (string)property.Value;
                    }
                    else
                    {
                        return new string[0];
                    }

                    string[] processedRoles = new string[roles.Length];

                    for (int i = 0; i < processedRoles.Length; ++i)
                    {
                        processedRoles[i] = roles[i].Replace(',', ';');
                    }

                    return processedRoles;
                }
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}
