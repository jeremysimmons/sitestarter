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
	/// Provides functions for interacting with keywords.
	/// </summary>
    [DataObject(true)]
    public class KeywordFactory : KeywordFactory<Keyword>
	{
	
	}
	
	/// <summary>
	/// Provides functions for interacting with keywords.
	/// </summary>
    [DataObject(true)]
	public class KeywordFactory<K>
		where K : IKeyword
    {
        /// <summary>
        /// Gets the data store containing the objects that this factory interact with.
        /// </summary>
        static public IDataStore DataStore
        {
            get { return DataAccess.Data.Stores[typeof(K)]; }
        }

		#region Retrieve functions
	    /// <summary>
		/// Retrieves all the keywords from the DB.
		/// </summary>
		/// <returns>An array of keywords.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
		static public K[] GetKeywords()
		{
            return (K[])Collection<K>.ConvertAll(DataStore.GetEntities<K>());
		}

		/// <summary>
		/// Retrieves all the specified keywords from the DB.
		/// </summary>
		/// <param name="keywordIDs">An array of IDs of keywords to retrieve.</param>
		/// <returns>An array of keywords.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public K[] GetKeywords(Guid[] keywordIDs)
		{
			// Create a new keyword collection
            Collection<K> keywords = new Collection<K>();

			// Loop through the IDs and add each keyword to the collection
			foreach (Guid keywordID in keywordIDs)
			{
				if (keywordID != Guid.Empty)
					keywords.Add(GetKeyword(keywordID));
			}

			// Return the collection
			return keywords.ToArray();
		}

		/// <summary>
		/// Retrieves the specified keyword from the DB.
		/// </summary>
		/// <param name="keywordID">The ID of the keyword to retrieve.</param>
		/// <returns>A Keyword object containing the requested info.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public K GetKeyword(Guid keywordID)
		{
            // If the ID is empty return null
            if (keywordID == Guid.Empty)
            	return default(K);

            return (K)DataAccess.Data.GetEntity<K>("ID", keywordID);
		}

        /// <summary>
        /// Retrieves the keyword with the provided name.
        /// </summary>
        static public K GetKeywordByName(string name)
        {
            return (K)DataAccess.Data.GetEntity<K>("Name", name);
        }
		#endregion

		#region Save functions
		/// <summary>
		/// Saves the provided keyword to the DB.
		/// </summary>
		/// <param name="keyword">The keyword to save.</param>
		/// <returns>A boolean value indicating whether the keywordname is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        static public bool SaveKeyword(K keyword)
		{
			// Check if the keywordname is already taken.
			if (KeywordNameTaken(keyword))
			{
				// Save unsuccessful.
				return false;
			}
			// ... if the keywordname is NOT taken.
			else
			{
				// Save the object.
				DataStore.Save(keyword);

				// Save successful.
				return true;
			}
		}
		#endregion

		#region Update functions
		/// <summary>
		/// Updates the provided keyword to the DB.
		/// </summary>
		/// <param name="keyword">The keyword to update.</param>
		/// <returns>A boolean value indicating whether the keywordname is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
        static public bool UpdateKeyword(K keyword)
		{
			// Check if the keywordname is already taken.
			if (KeywordNameTaken(keyword))
			{
				// Update unsuccessful.
				return false;
			}
			// ... if the keywordname is NOT taken.
			else
			{
				// Update the object.
                		DataStore.Update(keyword);

				// Update successful.
				return true;
			}
		}
		#endregion

		#region Delete functions
		/// <summary>
		/// Deletes the provided keyword.
		/// </summary>
		/// <param name="keyword">The keyword to delete.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        static public void DeleteKeyword(K keyword)
		{
            if (keyword != null)
            {
                // Check that the keyword is bound to the DB
                if (!DataStore.IsStored(keyword))
                    keyword = GetKeyword(keyword.ID);

                DataStore.Delete(keyword);
            }
		}
		#endregion

		#region Search functions
		/*/// <summary>
		/// Searches the keywords with the provided query.
		/// </summary>
		/// <param name="query">The query to search keywords with.</param>
		/// <returns>A KeywordSet containing the matching keywords.</returns>
        static public Collection<K> SearchKeywords(string query)
		{
			// Create a list of searchable properties
			string[] properties = new string[] {"firstName",
												   "lastName",
												   "keywordname"};

			// Search the keywords
            Collection<K> keywords = new Collection<K>(Db4oHelper.SearchObjects(typeof(K), properties, query));

			// Return all matching keywords
			return keywords;
		}*/
		#endregion

		#region Validation functions
		/// <summary>
		/// Checks whether the keywordname of the provided keyword is already taken.
		/// </summary>
		/// <param name="keyword">The keyword to check the keywordname of.</param>
		/// <returns>A boolean value indicating whether the keywordname is taken.</returns>
        static public bool KeywordNameTaken(K keyword)
		{
            using (LogGroup logGroup = AppLogger.StartGroup("Verifying that the keywordname is unique.", NLog.LogLevel.Info))
            {
                AppLogger.Info("Keyword ID: " + keyword.ID.ToString());
                AppLogger.Info("Keywordname: " + keyword.Name);

                // If no keywordname was specified just skip this function
                if (keyword.Name == String.Empty)
                    return false;

                // Retrieve any existing keyword with the keywordname.
                K existing = GetKeywordByName(keyword.Name);

                AppLogger.Info("Found match - Keyword ID: " + keyword.ID.ToString());
                AppLogger.Info("Found match - Keywordname: " + keyword.Name);

                bool isTaken = (existing != null && existing.ID != keyword.ID);

                if (isTaken)
                    AppLogger.Info("Keywordname has already been taken.");
                else
                    AppLogger.Info("Keywordname can be used.");

                // If a keyword was found and the IDs are not the same then it's already taken.
                return isTaken;
            }
		}
		#endregion
	}
}