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
	public class UserRoleFactory : UserRoleFactory<UserRole>
	{
		
	}
	
	/// <summary>
	/// Provides an interface for interacting with roles.
	/// </summary>
	[DataObject(true)]
	public class UserRoleFactory<R> : BaseFactory
		where R : IUserRole
	{
		static private UserRoleFactory<R> current;
		static public UserRoleFactory<R> Current
		{
			get {
				if (current == null)
					current = new UserRoleFactory<R>();
				return current; }
		}
		
		public override Dictionary<string, Type> DefaultTypes
		{
			get { return base.DefaultTypes; }
			set { base.DefaultTypes = value; }
		}
		
		/// <summary>
		/// Gets the data store containing the objects that this factory interact with.
		/// </summary>
		public IDataStore DataStore
		{
			get { return DataAccess.Data.Stores[typeof(Entities.IUserRole)]; }
		}
		
		public UserRoleFactory()
		{
			DefaultTypes = new Dictionary<string, Type>();
			DefaultTypes.Add("IUser", typeof(Entities.User));
		}

		#region Retrieve functions
		/// <summary>
		/// Retrieves all the roles from the DB.
		/// </summary>
		/// <returns>A UserRoleSet containing the retrieved roles.</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public R[] GetUserRoles()
		{
			return Collection<R>.ConvertAll(DataStore.GetEntities<R>());
		}

		/// <summary>
		/// Retrieves all the specified roles from the DB.
		/// </summary>
		/// <param name="roleIDs">An array of IDs of roles to retrieve.</param>
		/// <returns>A UserRoleSet containing the retrieved roles.</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public R[] GetUserRoles(Guid[] roleIDs)
		{
			// Create a new role collection
			Collection<R> roles = new Collection<R>();

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
		public R GetUserRole(Guid roleID)
		{
			// If the ID is empty return null
			if (roleID == Guid.Empty)
				return default(R);

			return (R)DataAccess.Data.GetEntity<R>("ID", roleID);
		}

		/// <summary>
		/// Retrieves the roles with the provided name.
		/// </summary>
		public R[] GetUserRolesByName(string name)
		{
			return DataAccess.Data.GetEntities<R>("Name", name);
		}


		/// <summary>
		/// Retrieves the role with the provided name.
		/// </summary>
		public R GetUserRoleByName(string name)
		{
			return (R)DataAccess.Data.GetEntity<R>("Name", name);
		}
		#endregion

		#region Save functions
		/// <summary>
		/// Saves the provided role to the DB.
		/// </summary>
		/// <param name="role">The role to save.</param>
		/// <returns>A boolean value indicating whether the save was successful (if not the username is taken).</returns>
		[DataObjectMethod(DataObjectMethodType.Insert, true)]
		public bool SaveUserRole(Entities.IUserRole role)
		{
			bool isComplete = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Saving user role: " + role.Name, NLog.LogLevel.Debug))
			{
				// Check if the rolename is already taken.
				if (UserRoleNameTaken(role))
				{
					// Save unsuccessful.
					isComplete = false;
				}
				// ... if the rolename is NOT taken.
				else
				{
					// Save the object.
					DataStore.Save(role);

					// Save successful.
					isComplete = true;
				}
			}
			
			return isComplete;
		}
		#endregion

		#region Update functions
		/// <summary>
		/// Updates the provided role to the DB.
		/// </summary>
		/// <param name="role">The role to update.</param>
		/// <returns>A boolean value indicating whether the rolename is taken.</returns>
		[DataObjectMethod(DataObjectMethodType.Update, true)]
		public bool UpdateUserRole(Entities.IUserRole role)
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
		public void DeleteUserRole(Entities.IUserRole role)
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

		#region Validation functions
		/// <summary>
		/// Checks whether the rolename of the provided role is already taken.
		/// </summary>
		/// <param name="role">The role to check the rolename of.</param>
		/// <returns>A boolean value indicating whether the rolename is taken.</returns>
		public bool UserRoleNameTaken(Entities.IUserRole role)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Verifying that the rolename is unique.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("User role ID: " + role.ID.ToString());
				AppLogger.Debug("User role name: " + role.Name);

				// If no rolename was specified just skip this function
				if (role.Name == String.Empty)
					return false;

				// Retrieve any existing role with the rolename.
				Entities.IUserRole existing = GetUserRoleByName(role.Name);

				bool isTaken = (existing != null && existing.ID != role.ID);

				if (isTaken)
					AppLogger.Debug("Role name has already been taken.");
				else
					AppLogger.Debug("Role name can be used.");

				// If a role was found and the IDs are not the same then it's already taken.
				return isTaken;
			}
		}
		#endregion
	}
}