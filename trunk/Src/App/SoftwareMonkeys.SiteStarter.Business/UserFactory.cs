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
	public class UserFactory
    {
        /// <summary>
        /// Gets the data store containing the objects that this factory interact with.
        /// </summary>
        static public IDataStore DataStore
        {
            get { return DataAccess.Data.Stores[typeof(Entities.User)]; }
        }

		#region Retrieve functions
	    /// <summary>
		/// Retrieves all the users from the DB.
		/// </summary>
		/// <returns>A UserSet containing the retrieved users.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
		static public Entities.User[] GetUsers()
		{
            return Collection<Entities.User>.ConvertAll(DataStore.GetEntities(typeof(Entities.User)));
		}

		/// <summary>
		/// Retrieves all the specified users from the DB.
		/// </summary>
		/// <param name="userIDs">An array of IDs of users to retrieve.</param>
		/// <returns>An array of the retrieved users.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public Entities.User[] GetUsers(Guid[] userIDs)
		{
			// Create a new user collection
            Collection<Entities.User> users = new Collection<Entities.User>();

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
        static public Entities.User GetUser(Guid userID)
		{
            // If the ID is empty return null
            if (userID == Guid.Empty)
                return null;

            return (Entities.User)DataStore.GetEntity(typeof(Entities.User), "id", userID);
		}

		/// <summary>
		/// Retrieves the user with the provided username.
		/// </summary>
        static public Entities.User GetUserByUsername(string username)
		{
			Entities.User user = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the user with the username: " + username, NLog.LogLevel.Debug))
			{
				user = (Entities.User)DataStore.GetEntity(typeof(Entities.User), "username", username);

				if (user != null)
					AppLogger.Debug("User ID: "+  user.ID);
				else
					AppLogger.Debug("User not found.");
			}
			return user;
		}

        /// <summary>
        /// Retrieves the user with the provided email.
        /// </summary>
        static public Entities.User[] GetUsersByEmail(string email)
        {
            return Collection<Entities.User>.ConvertAll(DataAccess.Data.GetEntities(typeof(Entities.User), "Email", email));
        }

        /// <summary>
        /// Retrieves the users with the provided name.
        /// </summary>
        static public Entities.User[] GetUsersByName(string name)
        {
            return Collection<Entities.User>.ConvertAll(DataAccess.Data.GetEntities(typeof(Entities.User), "Name", name));
        }

        /// <summary>
        /// Retrieves the user with the provided email.
        /// </summary>
        static public Entities.User GetUserByEmail(string email)
        {
            return (Entities.User)DataStore.GetEntity(typeof(Entities.User), "email", email);
        }
		#endregion

		#region Security functions
		/// <summary>
		/// Retrieves the user with the specified username and password.
		/// </summary>
		/// <param name="username">The username of the user to retrieve.</param>
		/// <param name="password">The password of the user to retrieve.</param>
		/// <returns>The user with the provided credentials.</returns>
        static public Entities.User AuthenticateUser(string username, string password)
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
		            parameters.Add("username", username);
		            parameters.Add("password", password);

			// Retrieve and return the user with the username and password.
            			user = (Entities.User)DataStore.GetEntity(typeof(Entities.User), parameters);

				if (user != null)
				{
					AppLogger.Debug("User ID: " + user.ID);
					AppLogger.Debug("Name: " + user.Name);
					AppLogger.Debug("Email: " + user.Email);
				}
				else
					AppLogger.Debug("User not found...credentials are invalid.");
			}

			return user;
		}
		#endregion

		#region Save functions
		/// <summary>
		/// Saves the provided user to the DB.
		/// </summary>
		/// <param name="user">The user to save.</param>
		/// <returns>A boolean value indicating whether the username is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        static public bool SaveUser(Entities.User user)
		{
			// Check if the username is already taken.
			if (UsernameTaken(user))
			{
				// Save unsuccessful.
				return false;
			}
			// ... if the username is NOT taken.
			else
			{
				// Save the object.
				DataStore.Save(user);

				// Save successful.
				return true;
			}
		}
		#endregion

		#region Update functions
		/// <summary>
		/// Updates the provided user to the DB.
		/// </summary>
		/// <param name="user">The user to update.</param>
		/// <returns>A boolean value indicating whether the username is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
        static public bool UpdateUser(Entities.User user)
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
                		DataStore.Update(user);

				// Update successful.
				return true;
			}
		}
		#endregion

		#region Delete functions
		/// <summary>
		/// Deletes the provided user.
		/// </summary>
		/// <param name="user">The user to delete.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        static public void DeleteUser(Entities.User user)
		{
            if (user != null)
            {
                // Check that the user is bound to the DB
                if (!DataStore.IsStored(user))
                    user = GetUser(user.ID);

                DataStore.Delete(user);
            }
		}
		#endregion

		#region Search functions
		/*/// <summary>
		/// Searches the users with the provided query.
		/// </summary>
		/// <param name="query">The query to search users with.</param>
		/// <returns>A UserSet containing the matching users.</returns>
        static public Collection<Entities.User> SearchUsers(string query)
		{
			// Create a list of searchable properties
			string[] properties = new string[] {"firstName",
												   "lastName",
												   "username"};

			// Search the users
            Collection<Entities.User> users = new Collection<Entities.User>(Db4oHelper.SearchObjects(typeof(Entities.User), properties, query));

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
        static public bool UsernameTaken(Entities.User user)
		{
            using (LogGroup logGroup = AppLogger.StartGroup("Verifying that the username is unique.", NLog.LogLevel.Info))
            {
                AppLogger.Info("User ID: " + user.ID.ToString());
                AppLogger.Info("Username: " + user.Username);

                // If no username was specified just skip this function
                if (user.Username == null || user.Username == String.Empty)
                    return false;

                // Retrieve any existing user with the username.
                Entities.User existing = GetUserByUsername(user.Username);

                AppLogger.Info("Found match - User ID: " + user.ID.ToString());
                AppLogger.Info("Found match - Username: " + user.Username);

                bool isTaken = (existing != null && existing.ID != user.ID);

                if (isTaken)
                    AppLogger.Info("Username has already been taken.");
                else
                    AppLogger.Info("Username can be used.");

                // If a user was found and the IDs are not the same then it's already taken.
                return isTaken;
            }
		}
		#endregion
	}
}
