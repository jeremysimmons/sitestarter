using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Provides an interface for interacting with roles.
	/// </summary>
    [DataObject(true)]
	public class UserRoleFactory
    {
        /// <summary>
        /// Gets the data store containing the objects that this factory interact with.
        /// </summary>
        static public IDataStore DataStore
        {
            get { return DataAccess.Data.Stores[typeof(Entities.UserRole)]; }
        }

		#region Retrieve functions
	    /// <summary>
		/// Retrieves all the roles from the DB.
		/// </summary>
		/// <returns>A UserRoleSet containing the retrieved roles.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
		static public Entities.UserRole[] GetUserRoles()
		{
            return (UserRole[])Collection<Entities.UserRole>.ConvertAll(DataStore.GetEntities(typeof(Entities.UserRole)));
		}

		/// <summary>
		/// Retrieves all the specified roles from the DB.
		/// </summary>
		/// <param name="roleIDs">An array of IDs of roles to retrieve.</param>
		/// <returns>A UserRoleSet containing the retrieved roles.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public Entities.UserRole[] GetUserRoles(Guid[] roleIDs)
		{
			// Create a new role collection
            Collection<Entities.UserRole> roles = new Collection<Entities.UserRole>();

			// Loop through the IDs and add each role to the collection
			foreach (Guid roleID in roleIDs)
			{
				if (roleID != Guid.Empty)
					roles.Add(GetUserRole(roleID));
			}

			// Return the collection
			return roles.ToArray();
		}

		/// <summary>
		/// Retrieves the specified role from the DB.
		/// </summary>
		/// <param name="roleID">The ID of the role to retrieve.</param>
		/// <returns>A UserRole object containing the requested info.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public Entities.UserRole GetUserRole(Guid roleID)
		{
            // If the ID is empty return null
            if (roleID == Guid.Empty)
                return null;

            return (Entities.UserRole)DataStore.GetEntity(typeof(Entities.UserRole), "id", roleID);
		}

        /// <summary>
        /// Retrieves the role with the provided email.
        /// </summary>
        static public Entities.UserRole[] GetUserRolesByEmail(string email)
        {
            return (UserRole[])Collection<Entities.UserRole>.ConvertAll(DataStore.GetEntities(typeof(Entities.UserRole), "email", email));
        }

        /// <summary>
        /// Retrieves the roles with the provided name.
        /// </summary>
        static public Entities.UserRole[] GetUserRolesByName(string name)
        {
            return (UserRole[])Collection<Entities.UserRole>.ConvertAll(DataStore.GetEntities(typeof(Entities.UserRole), "name", name));
        }


        /// <summary>
        /// Retrieves the role with the provided name.
        /// </summary>
        static public Entities.UserRole GetUserRoleByName(string name)
        {
            return (Entities.UserRole)DataStore.GetEntity(typeof(Entities.UserRole), "name", name);
        }

        /// <summary>
        /// Retrieves the role with the provided email.
        /// </summary>
        static public Entities.UserRole GetUserRoleByEmail(string email)
        {
            return (Entities.UserRole)DataStore.GetEntity(typeof(Entities.UserRole), "email", email);
        }
		#endregion

		#region Security functions
		/*/// <summary>
		/// Retrieves the role with the specified rolename and password.
		/// </summary>
		/// <param name="rolename">The rolename of the role to retrieve.</param>
		/// <param name="password">The password of the role to retrieve.</param>
		/// <returns>The role with the provided credentials.</returns>
        static public Entities.UserRole AuthenticateUserRole(string rolename, string password)
		{
			Entities.UserRole role = null;

			using(LogGroup logGroup = AppLogger.StartGroup("Retrieves the role with the specified rolename and password.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("UserRolename: " + rolename);
                  // TODO: Check encrypt password code
			// Encrypt the password if necessary.
			//if (encryptPassword)
				password = Crypter.EncryptPassword(password);
			
			// Create the query
		            Dictionary<string, object> parameters = new Dictionary<string, object>();
		            parameters.Add("rolename", rolename);
		            parameters.Add("password", password);

			// Retrieve and return the role with the rolename and password.
            			role = (Entities.UserRole)DataStore.GetEntity(typeof(Entities.UserRole), parameters);

				if (role != null)
				{
					AppLogger.Debug("UserRole ID: " + role.ID);
					AppLogger.Debug("Name: " + role.Name);
					AppLogger.Debug("Email: " + role.Email);
				}
				else
					AppLogger.Debug("UserRole not found...credentials are invalid.");
			}

			return role;
		}*/
		#endregion

		#region Save functions
		/// <summary>
		/// Saves the provided role to the DB.
		/// </summary>
		/// <param name="role">The role to save.</param>
		/// <returns>A boolean value indicating whether the rolename is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        static public bool SaveUserRole(Entities.UserRole role)
		{
			// Check if the rolename is already taken.
			if (UserRoleNameTaken(role))
			{
				// Save unsuccessful.
				return false;
			}
			// ... if the rolename is NOT taken.
			else
			{
				// Save the object.
				DataStore.Save(role);

				// Save successful.
				return true;
			}
		}
		#endregion

		#region Update functions
		/// <summary>
		/// Updates the provided role to the DB.
		/// </summary>
		/// <param name="role">The role to update.</param>
		/// <returns>A boolean value indicating whether the rolename is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
        static public bool UpdateUserRole(Entities.UserRole role)
		{
			// Check if the rolename is already taken.
			if (UserRoleNameTaken(role))
			{
				// Update unsuccessful.
				return false;
			}
			// ... if the rolename is NOT taken.
			else
			{
				// Update the object.
                		DataStore.Update(role);

				// Update successful.
				return true;
			}
		}
		#endregion

		#region Delete functions
		/// <summary>
		/// Deletes the provided role.
		/// </summary>
		/// <param name="role">The role to delete.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        static public void DeleteUserRole(Entities.UserRole role)
		{
            if (role != null)
            {
                // Check that the role is bound to the DB
                if (!DataStore.IsStored(role))
                    role = GetUserRole(role.ID);

                DataStore.Delete(role);
            }
		}
		#endregion

		#region Search functions
		/*/// <summary>
		/// Searches the roles with the provided query.
		/// </summary>
		/// <param name="query">The query to search roles with.</param>
		/// <returns>A UserRoleSet containing the matching roles.</returns>
        static public Collection<Entities.UserRole> SearchUserRoles(string query)
		{
			// Create a list of searchable properties
			string[] properties = new string[] {"firstName",
												   "lastName",
												   "rolename"};

			// Search the roles
            Collection<Entities.UserRole> roles = new Collection<Entities.UserRole>(Db4oHelper.SearchObjects(typeof(Entities.UserRole), properties, query));

			// Return all matching roles
			return roles;
		}*/
		#endregion

		#region Validation functions
		/// <summary>
		/// Checks whether the rolename of the provided role is already taken.
		/// </summary>
		/// <param name="role">The role to check the rolename of.</param>
		/// <returns>A boolean value indicating whether the rolename is taken.</returns>
        static public bool UserRoleNameTaken(Entities.UserRole role)
		{
            using (LogGroup logGroup = AppLogger.StartGroup("Verifying that the rolename is unique.", NLog.LogLevel.Info))
            {
                AppLogger.Info("UserRole ID: " + role.ID.ToString());
                AppLogger.Info("UserRolename: " + role.Name);

                // If no rolename was specified just skip this function
                if (role.Name == String.Empty)
                    return false;

                // Retrieve any existing role with the rolename.
                Entities.UserRole existing = GetUserRoleByName(role.Name);

                AppLogger.Info("Found match - UserRole ID: " + role.ID.ToString());
                AppLogger.Info("Found match - UserRolename: " + role.Name);

                bool isTaken = (existing != null && existing.ID != role.ID);

                if (isTaken)
                    AppLogger.Info("UserRolename has already been taken.");
                else
                    AppLogger.Info("UserRolename can be used.");

                // If a role was found and the IDs are not the same then it's already taken.
                return isTaken;
            }
		}
		#endregion
	}
}
