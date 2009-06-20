using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Data.Odbc;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Collections.Generic;

/*

This provider works with the following schema for the tables of role data.

CREATE TABLE Roles
(
  Rolename Text (255) NOT NULL,
  ApplicationName Text (255) NOT NULL,
    CONSTRAINT PKRoles PRIMARY KEY (Rolename, ApplicationName)
)

CREATE TABLE UsersInRoles
(
  Username Text (255) NOT NULL,
  Rolename Text (255) NOT NULL,
  ApplicationName Text (255) NOT NULL,
    CONSTRAINT PKUsersInRoles PRIMARY KEY (Username, Rolename, ApplicationName)
)

*/


namespace SoftwareMonkeys.SiteStarter.Web.Security
{

    public class RoleProvider : System.Web.Security.RoleProvider
    {

        //
        // Global connection string, generic exception message, event log info.
        //

        private string eventSource = "RoleProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";

       // private ConnectionStringSettings pConnectionStringSettings;
      //  private string connectionString;


        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool pWriteExceptionsToEventLog = false;

        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }



        //
        // System.Configuration.Provider.ProviderBase.Initialize Method
        //

        public override void Initialize(string name, NameValueCollection config)
        {

            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "RoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Dynamic Role provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);


            if (config["applicationName"] == null || config["applicationName"].Trim() == "")
            {
                pApplicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            }
            else
            {
                pApplicationName = config["applicationName"];
            }


            if (config["writeExceptionsToEventLog"] != null)
            {
                if (config["writeExceptionsToEventLog"].ToUpper() == "TRUE")
                {
                    pWriteExceptionsToEventLog = true;
                }
            }


            //
            // Initialize OdbcConnection.
            //

            /*pConnectionStringSettings = ConfigurationManager.
              ConnectionStrings[config["connectionStringName"]];

            if (pConnectionStringSettings == null || pConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = pConnectionStringSettings.ConnectionString;*/
        }



        //
        // System.Web.Security.RoleProvider properties.
        //


        private string pApplicationName;


        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }

        //
        // System.Web.Security.RoleProvider methods.
        //

        //
        // RoleProvider.AddUsersToRoles
        //

        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            foreach (string rolename in rolenames)
            {
                if (!RoleExists(rolename))
                {
                    throw new ProviderException("Role name not found.");
                }
            }

            foreach (string username in usernames)
            {
                if (username.Contains(","))
                {
                    throw new ArgumentException("User names cannot contain commas.");
                }

                foreach (string rolename in rolenames)
                {
                    if (IsUserInRole(username, rolename))
                    {
                        throw new ProviderException("User is already in role.");
                    }
                }
            }

            foreach (string username in usernames)
            {
                foreach (string rolename in rolenames)
                {
                    IUser user = UserFactory.Current.GetUserByUsername(username);

                    IUserRole role = UserRoleFactory.GetUserRoleByName(rolename);


                    user.AddRole(role);

                    role.AddUser(user);


                    UserFactory.Current.UpdateUser(user);

                    UserRoleFactory.UpdateUserRole(role);
                }
            }

            /*try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                cmd.Transaction = tran;

                foreach (string username in usernames)
                {
                    foreach (string rolename in rolenames)
                    {
                        userParm.Value = username;
                        roleParm.Value = rolename;
                        cmd.ExecuteNonQuery();
                    }
                }

                tran.Commit();
            }
            catch (OdbcException e)
            {
                try
                {
                    tran.Rollback();
                }
                catch { }


                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "AddUsersToRoles");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }*/
        }


        //
        // RoleProvider.CreateRole
        //

        public override void CreateRole(string rolename)
        {
            if (rolename.Contains(","))
            {
                throw new ArgumentException("Role names cannot contain commas.");
            }

            if (RoleExists(rolename))
            {
                throw new ProviderException("Role name already exists.");
            }

            IUserRole role = new UserRole();
            role.ID = Guid.NewGuid();
            role.Name = rolename;

            UserRoleFactory.SaveUserRole(role);
        }


        //
        // RoleProvider.DeleteRole
        //

        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            if (!RoleExists(rolename))
            {
                throw new ProviderException("Role does not exist.");
            }

            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
            {
                throw new ProviderException("Cannot delete a populated role.");
            }

            IUserRole role = UserRoleFactory.GetUserRoleByName(rolename);

            bool roleDeleted = false;

            if (role != null)
            {
                UserRoleFactory.DeleteUserRole(role);

                roleDeleted = true;
            }

            return roleDeleted;
        }


        //
        // RoleProvider.GetAllRoles
        //

        public override string[] GetAllRoles()
        {
            IUserRole[] roles = UserRoleFactory.GetUserRoles();

            List<string> names = new List<string>();
            foreach (IUserRole role in roles)
            {
                names.Add(role.Name);
            }

            return (string[])names.ToArray();
        }


        //
        // RoleProvider.GetRolesForUser
        //

        public override string[] GetRolesForUser(string username)
        {
            IUser user = UserFactory.Current.GetUserByUsername(username);

            IUserRole[] roles = UserRoleFactory.GetUserRoles(user.RoleIDs);

            List<string> names = new List<string>();
            foreach (IUserRole role in roles)
            {
                names.Add(role.Name);
            }

            return (string[])names.ToArray();
        }


        //
        // RoleProvider.GetUsersInRole
        //

        public override string[] GetUsersInRole(string rolename)
        {
            IUserRole role = UserRoleFactory.GetUserRoleByName(rolename);

            IUser[] users = UserFactory.Current.GetUsers(role.UserIDs);

            List<string> usernames = new List<string>();
            foreach (User user in users)
            {
                usernames.Add(user.Username);
            }

            return (string[])usernames.ToArray();
        }


        //
        // RoleProvider.IsUserInRole
        //

        public override bool IsUserInRole(string username, string rolename)
        {
            IUser user = UserFactory.Current.GetUserByUsername(username);

            IUserRole role = UserRoleFactory.GetUserRoleByName(rolename);


		if (user == null)
			throw new ProviderException("User not found with specified username.");

		if (role == null)
            throw new ProviderException("Role not found with specified name.");
	
		if (user.RoleIDs == null)
			return false;

            return Array.IndexOf(user.RoleIDs, role.ID) > -1;
        }


        //
        // RoleProvider.RemoveUsersFromRoles
        //

        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            foreach (string rolename in rolenames)
            {
                if (!RoleExists(rolename))
                {
                    throw new ProviderException("Role name not found.");
                }
            }

            foreach (string username in usernames)
            {
                foreach (string rolename in rolenames)
                {
                    if (!IsUserInRole(username, rolename))
                    {
                        throw new ProviderException("User is not in role.");
                    }
                }
            }


            foreach (string username in usernames)
            {
                foreach (string rolename in rolenames)
                {
                    IUser user = UserFactory.Current.GetUserByUsername(username);

                    IUserRole role = UserRoleFactory.GetUserRoleByName(rolename);


                    user.RemoveRole(role);

                    role.RemoveUser(user);


                    UserFactory.Current.UpdateUser(user);

                    UserRoleFactory.UpdateUserRole(role);
                }
            }
        }


        //
        // RoleProvider.RoleExists
        //

        public override bool RoleExists(string rolename)
        {
            IUserRole role = UserRoleFactory.GetUserRoleByName(rolename);

            return role != null;
        }

        //
        // RoleProvider.FindUsersInRole
        //

        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            List<string> usernames = new List<string>();

            IUserRole role = UserRoleFactory.GetUserRoleByName(rolename);

            IUser[] allUsers = UserFactory.Current.GetUsers(role.UserIDs);

            foreach (User user in allUsers)
                if (Array.IndexOf(user.RoleIDs, role.ID) > -1)
                    usernames.Add(user.Username);

            return (string[])usernames.ToArray();
        }

        //
        // WriteToEventLog
        //   A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also 
        // thrown by the caller.
        //

        private void WriteToEventLog(OdbcException e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            string message = exceptionMessage + "\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();

            log.WriteEntry(message);
        }

    }
}
