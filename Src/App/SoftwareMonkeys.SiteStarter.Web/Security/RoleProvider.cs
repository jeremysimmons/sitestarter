﻿using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Security
{
	
	public class RoleProvider : RoleProvider<Entities.UserRole>
	{
	}

	public class RoleProvider<R> : System.Web.Security.RoleProvider
		where R : IUserRole
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
					IUser user = RetrieveStrategy.New<User>().Retrieve<User>("Username", username);

					IUserRole role = RetrieveStrategy.New<UserRole>().Retrieve<UserRole>("Name", rolename);

					ActivateStrategy.New<User>().Activate(user);

					user.Roles = Collection<IUserRole>.Add(user.Roles, role);

					// Not needed. Previous line does it all.
					//                role.Users.Add(user);


					UpdateStrategy.New<User>().Update(user);
					
					// Not needed. Previous line does it all.
					//                UserRoleFactory.Current.UpdateUserRole(role);
				}
			}
		}


		//
		// RoleProvider.CreateRole
		//

		public override void CreateRole(string rolename)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Creating new role: " + rolename))
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
				LogWriter.Debug("Role ID: " + role.ID);
				role.Name = rolename;

				SaveStrategy.New<UserRole>().Save(role);
			}
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

			IUserRole role = RetrieveStrategy.New<UserRole>().Retrieve<UserRole>(rolename);

			bool roleDeleted = false;

			if (role != null)
			{
				DeleteStrategy.New<UserRole>().Delete(role);

				roleDeleted = true;
			}

			return roleDeleted;
		}


		//
		// RoleProvider.GetAllRoles
		//

		public override string[] GetAllRoles()
		{
			IUserRole[] roles = IndexStrategy.New<UserRole>().Index<UserRole>();

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
			IUser user = RetrieveStrategy.New<User>(false).Retrieve<User>("Username", username);

			IUserRole[] roles = IndexStrategy.New<UserRole>(false).Index<UserRole>(Collection<IUserRole>.GetIDs(user.Roles));

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
			IUserRole role = RetrieveStrategy.New<UserRole>().Retrieve<UserRole>("Name", rolename);

			ActivateStrategy.New<UserRole>().Activate(role, "Users");
			
			IUser[] users = role.Users;

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

		public bool IsUserInRole<U>(string username, string rolename)
			where U : IUser
		{
			IUser user = RetrieveStrategy.New("User", false).Retrieve<User>("Username", username);

			//IUserRole role = UserRoleFactory<R>.Current.GetUserRoleByName(rolename);

			ActivateStrategy.New("User", false).Activate(user, "Roles");

			if (user == null)
				throw new ProviderException("User not found with specified username.");
			
			//if (role == null)
			//	throw new ProviderException("Role not found with specified name.");
			
			if (user.Roles == null)
				return false;
			
			return user.IsInRole(rolename);
			
		}
		
		public override bool IsUserInRole(string username, string rolename)
		{
			return IsUserInRole<User>(username, rolename);
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
					IUser user = RetrieveStrategy.New<User>().Retrieve<User>("Username", username);

					IUserRole role = RetrieveStrategy.New<User>().Retrieve<UserRole>("Name", rolename);

					user.Activate();
					role.Activate();

					user.Roles = Collection<IUserRole>.Remove(user.Roles, role);

					role.Users = Collection<IUser>.Remove(role.Users, user);


					UpdateStrategy.New<User>().Update(user);
					
					UpdateStrategy.New<UserRole>().Update(role);
					
				}
			}
		}


		//
		// RoleProvider.RoleExists
		//

		public override bool RoleExists(string rolename)
		{
			IUserRole role = RetrieveStrategy.New<UserRole>().Retrieve<UserRole>("Name", rolename);

			return role != null;
		}

		//
		// RoleProvider.FindUsersInRole
		//
		
		public override string[] FindUsersInRole(string rolename, string usernameToMatch)
		{
			return FindUsersInRole<Entities.User>(rolename, usernameToMatch);
		}

		public string[] FindUsersInRole<U>(string rolename, string usernameToMatch)
			where U : IUser
		{
			List<string> usernames = new List<string>();

			IUserRole role = RetrieveStrategy.New<UserRole>().Retrieve<UserRole>("Name", rolename);
			
			U[] allUsers = IndexStrategy.New<User>().Index<U>(Collection<IUser>.GetIDs(role.Users));

			foreach (U user in allUsers)
				if (Array.IndexOf(Collection<IUserRole>.GetIDs(user.Roles), role.ID) > -1)
				usernames.Add(user.Username);

			return (string[])usernames.ToArray();
		}
	}
}
