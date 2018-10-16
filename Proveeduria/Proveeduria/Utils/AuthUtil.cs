using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Proveduria.Utils
{
    

    /// <summary>
    /// Methods for performing queries and checks against the AD.
    /// </summary>
    /// <remarks>
    /// The config file has to provide 2 settings DomainAccessServer, DomainAccessUser and DomainAccessPassword for specifying the
    /// user-account which will perform the queries against the AD!
    /// </remarks>
    public static class AuthUtil
    {
        #region methods

        /// <summary>
        /// Checks, if a given combination of <paramref name="userName"/> and <paramref name="passWord"/> is found inside the AD.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="passWord">The password of the user.</param>
        /// <returns><c>True</c> if the combination provided is correct.</returns>
        public static bool IsValidAdUser(string userName, string passWord)
        {
            var result = false;
            using (var context = GetContext())
            {
                result = context.ValidateCredentials(userName, passWord);
            }
            return result;
        }

        /// <summary>
        /// Checks, if a given user is member of a certain goup.
        /// </summary>
        /// <param name="userName">The user to check.</param>        
        /// <param name="groupName">The name of the AD group.</param>
        /// <returns><c>True</c> if the user is member of the group.</returns>
        public static bool IsUserMemberOfGroup(string userName, string groupName)
        {
            var groups = GetUserGroupNames(userName);
            return groups.Contains(groupName);
        }

        /// <summary>
        /// Retrieves a list of groups names the <paramref name="userName"/> is member of.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <returns>The list of groupnames.</returns>
        public static List<string> GetUserGroupNames(string userName)
        {
            var result = new List<string>();
            using (var context = GetContext())
            {
                try
                {
                    using (var userPrinc = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName))
                    {
                        foreach (var groupPrinc in userPrinc.GetGroups())
                        {
                            try
                            {
                                result.Add(groupPrinc.Name);
                            }
                            finally
                            {
                                groupPrinc.Dispose();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// Is used internally to create a context for AD queries.
        /// </summary>
        /// <returns>The instantiated context.</returns>
        private static PrincipalContext GetContext()
        {
            return new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["DomainAccessServer"]);
        }

        #endregion
    }
}