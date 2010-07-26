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
	/// Provides an interface for interacting with users.
	/// </summary>
	[DataObject(true)]
	public class UserFactory : UserFactory<Entities.User>
	{
		
	}
	
	
	/// <summary>
	/// Provides an interface for interacting with users.
	/// </summary>
	[DataObject(true)]
	public class UserFactory<U> : BaseFactory
		where U : IUser
	{
		static private UserFactory<U> current;
		static public UserFactory<U> Current
		{
			get {
				if (current == null)
					current = new UserFactory<U>();
				return current; }
		}
		
		/// <summary>
		/// Gets the data store containing the objects that this factory interact with.
		/// </summary>
		public IDataStore DataStore
		{
			get { return DataAccess.Data.Stores[typeof(Entities.User)]; }
		}
		
		public UserFactory()
		{
		}

		#region Retrieve functions
		/// <summary>
		/// Retrieves all the users from the DB.
		/// </summary>
		/// <returns>A UserSet containing the retrieved users.</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public U[] GetUsers()
		{
			return Collection<U>.ConvertAll(DataAccess.Data.Indexer.GetEntities<U>());
		}
		
		/*/// <summary>
		/// Retrieves all the users from the DB.
		/// </summary>
		/// <returns>A UserSet containing the retrieved users.</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public U[] GetPageOfUsers(PagingLocation location)
		{
			return Collection<U>.ConvertAll(DataAccess.Data.Indexer.GetEntitiesPage<U>(location));
		}*/

			/// <summary>
			/// Retrieves all the specified users from the DB.
			/// </summary>
			/// <param name="userIDs">An array of IDs of users to retrieve.</param>
			/// <returns>An array of the retrieved users.</returns>
			[DataObjectMethod(DataObjectMethodType.Select, true)]
			public U[] GetUsers(Guid[] userIDs)
		{
			// Create a new user collection
			Collection<U> users = new Collection<U>();

			// Loop through the IDs and add each user to the collection
			foreach (Guid userID in userIDs)
			{
				if (userID != Guid.Empty)
					users.Add(GetUser(userID));
			}

			// Return the collection
			return users.ToArray();
		}

		/// <summary>
		/// Retrieves the specified user from the DB.
		/// </summary>
		/// <param name="userID">The ID of the user to retrieve.</param>
		/// <returns>A User object containing the requested info.</returns>
		[DataObjectMethod(DataObjectMethodType.Select, true)]
		public U GetUser(Guid userID)
		{
			// If the ID is empty return null
			if (userID == Guid.Empty)
				return default(U);

			return (U)DataAccess.Data.Reader.GetEntity<U>("ID", userID);
		}

		/// <summary>
		/// Retrieves the user with the provided username.
		/// </summary>
		public Entities.User GetUserByUsername(string username)
		{
			Entities.User user = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the user with the username: " + username, NLog.LogLevel.Debug))
			{
				user = (Entities.User)DataAccess.Data.Reader.GetEntity<Entities.User>("Username", username);

				if (user != null)
					AppLogger.Debug("User ID: "+  user.ID);
				else
					AppLogger.Debug("User not found.");
			}
			return user;
		}

		/// <summary>
		/// Retrieves the user with the provided username.
		/// </summary>
		public Entities.User[] GetNotifiableUsers()
		{
			Entities.User[] users = new Entities.User[] {};
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the users who are selected to be notified of important events.", NLog.LogLevel.Debug))
			{
				users = DataAccess.Data.Indexer.GetEntities<Entities.User>("EnableNotifications", true);
			}
			return users;
		}

		
		
		/// <summary>
		/// Retrieves the user with the provided email.
		/// </summary>
		public U[] GetUsersByEmail(string email)
		{
			return Collection<U>.ConvertAll(DataAccess.Data.Indexer.GetEntities<U>("Email", email));
		}

		/// <summary>
		/// Retrieves the users with the provided name.
		/// </summary>
		public U[] GetUsersByName(string name)
		{
			return Collection<U>.ConvertAll(DataAccess.Data.Indexer.GetEntities<U>("Name", name));
		}

		/// <summary>
		/// Retrieves the user with the provided email.
		/// </summary>
		public U GetUserByEmail(string email)
		{
			return (U)DataAccess.Data.Reader.GetEntity<U>("Email", email);
		}
		#endregion

		#region Security functions
		/// <summary>
		/// Retrieves the user with the specified username and password.
		/// </summary>
		/// <param name="username">The username of the user to retrieve.</param>
		/// <param name="password">The password of the user to retrieve.</param>
		/// <returns>The user with the provided credentials.</returns>
		public Entities.User AuthenticateUser(string username, string password)
		{
			Entities.User user = null;

			using(LogGroup logGroup = AppLogger.StartGroup("Retrieves the user with the specified username and password.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Username: " + username);
				// TODO: Check encrypt password code
				// Encrypt the password if necessary.
				//if (encryptPassword)
				password = Crypter.EncryptPassword(password);
				
				// Create the query
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters.Add("Username", username);
				parameters.Add("Password", password);

				// Retrieve and return the user with the username and password.
				user = (Entities.User)DataAccess.Data.Reader.GetEntity<Entities.User>(parameters);

				if (user != null)
				{
					AppLogger.Debug("User ID: " + user.ID);
					AppLogger.Debug("Name: " + user.Name);
					AppLogger.Debug("Email: " + user.Email);
				}
				else
					AppLogger.Debug("User not found...credentials are invalid.");
				
				// If the account hasn't be approved or the user is locked out then return null
				// This method is for validation. To retrieve a locked out user the Get functions should be used.
				if (user != null && (!user.IsApproved || user.IsLockedOut))
					user = null;
			}

			return user;
		}
		#endregion

		#region Save functions
		/// <summary>
		/// Saves the provided user to the DB.
		/// </summary>
		/// <param name="user">The user to save.</param>
		/// <returns>A boolean value indicating whether the save was successful (if not the username is taken).</returns>
		[DataObjectMethod(DataObjectMethodType.Insert, true)]
		public bool SaveUser(Entities.IUser user)
		{
			bool isComplete = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the user: " + user.Username, NLog.LogLevel.Debug))
			{
				// Check if the username is already taken.
				if (UsernameTaken(user))
				{
					// Save unsuccessful.
					isComplete = false;
				}
				// ... if the username is NOT taken.
				else
				{
					// Save the object.
					DataAccess.Data.Saver.Save(user);

					// Save successful.
					isComplete = true;
				}
			}
			
			return isComplete;
		}
		#endregion

		#region Update functions
		/// <summary>
		/// Updates the provided user to the DB.
		/// </summary>
		/// <param name="user">The user to update.</param>
		/// <returns>A boolean value indicating whether the username is taken.</returns>
		[DataObjectMethod(DataObjectMethodType.Update, true)]
		public bool UpdateUser(Entities.IUser user)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Updating the provided user object.", NLog.LogLevel.Debug))
			{
				// Check if the username is already taken.
				if (UsernameTaken(user))
				{
					// Update unsuccessful.
					return false;
				}
				// ... if the username is NOT taken.
				else
				{
					// Update the object.
					DataAccess.Data.Updater.Update(user);
					
					// Update successful.
					return true;
				}
			}
		}
		#endregion

		#region Delete functions
		/// <summary>
		/// Deletes the provided user.
		/// </summary>
		/// <param name="user">The user to delete.</param>
		[DataObjectMethod(DataObjectMethodType.Delete, true)]
		public void DeleteUser(Entities.IUser user)
		{
			if (user != null)
			{
				// Check that the user is bound to the DB
				if (!DataStore.IsStored(user))
					user = GetUser(user.ID);

				DataAccess.Data.Deleter.Delete(user);
			}
		}
		#endregion

		#region Search functions
		/*/// <summary>
		/// Searches the users with the provided query.
		/// </summary>
		/// <param name="query">The query to search users with.</param>
		/// <returns>A UserSet containing the matching users.</returns>
		public Collection<Entities.IUser> SearchUsers(string query)
		{
			// Create a list of searchable properties
			string[] properties = new string[] {"firstName",
				"lastName",
				"username"};

			// Search the users
			Collection<Entities.IUser> users = new Collection<Entities.IUser>(Db4oHelper.SearchObjects(typeof(Entities.IUser), properties, query));

			// Return all matching users
			return users;
		}*/
			#endregion

			#region Validation functions
			/// <summary>
			/// Checks whether the username of the provided user is already taken.
			/// </summary>
			/// <param name="user">The user to check the username of.</param>
			/// <returns>A boolean value indicating whether the username is taken.</returns>
			public bool UsernameTaken(Entities.IUser user)
		{
			bool isTaken = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Verifying that the username is unique.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("User ID: " + user.ID.ToString());
				AppLogger.Debug("Username: " + user.Username);

				// If no username was specified just skip this function
				if (user.Username == null || user.Username == String.Empty)
					isTaken = false;
				else
				{

					// Retrieve any existing user with the username.
					Entities.IUser existing = GetUserByUsername(user.Username);

					if (existing != null)
					{
						AppLogger.Debug("Found user - User ID: " + existing.ID.ToString());
						AppLogger.Debug("Found user - Username: " + existing.Username);
					}
					else
						AppLogger.Debug("No existing user found with the username '" + user.Username + "'.");

				// If a user was found and the IDs are not the same then it's already taken.
					isTaken = (existing != null && existing.ID != user.ID);

					if (isTaken)
						AppLogger.Debug("Username has already been taken.");
					else
						AppLogger.Debug("Username can be used.");
				}
			}
			return isTaken;
		}
		#endregion
	}
}
