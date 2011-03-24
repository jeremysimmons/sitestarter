using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data
{
    /// <summary>
    /// Defines the interface for all data stores.
    /// </summary>
    public interface IDataStore
    {
        #region Properties
        /// <summary>
        /// Gets the name of the data store.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the ID of the virtual server that the data store exists on.
        /// </summary>
        string VirtualServerID { get; }
        #endregion

        #region Initialize/open/close/dispose functions
		/// <summary>
		/// Opens the data store object. Doesn't necessarily have to open the actual underlying store yet, as it can possibly wait until it's needed.
		/// </summary>
        void Open();
		/// <summary>
		/// Disposes the data store. Also closes the data store and clears it from state.
		/// </summary>
        void Dispose();
		/// <summary>
		/// Closes the data store.
		/// </summary>
        void Close();
        #endregion

        /// <summary>
        /// Checks whether the provided entity is found in the store.
        /// </summary>
        /// <param name="entity">The entity to look for.</param>
        /// <returns>A boolean flag indicating whether the provided entity was found.</returns>
        bool IsStored(IEntity entity);
        
        /// <summary>
        /// Checks whether the data store is currently closed.
        /// </summary>
        /// <returns>A boolean flag indicating whether the data store is closed.</returns>
        bool IsClosed {get;}
        
        /// <summary>
        /// Commits the data from memory to the physical store. Utilizes batching to avoid committing too often.
        /// </summary>
        void Commit();
        
        /// <summary>
        /// Commits the data from memory to the physical store.
        /// </summary>
        /// <param name="forceCommit">A boolean value indicating whether to force the commit immediately and not use batching.</param>
        void Commit(bool forceCommit);
        
		/// <summary>
		/// Holds a JIT loaded instance of the data reader for the current data provider.
		/// </summary>
		IDataReader Reader {get;}
		
		/// <summary>
		/// Holds a JIT loaded instance of the data indexer for the current data provider.
		/// </summary>
		IDataIndexer Indexer {get;}
		
		/// <summary>
		/// Holds a JIT loaded instance of the data referencer for the current data provider.
		/// </summary>
		IDataReferencer Referencer {get;}
		
		/// <summary>
		/// Holds a JIT loaded instance of the data activator for the current data provider.
		/// </summary>
		IDataActivator Activator {get;}
		
		/// <summary>
		/// Holds a JIT loaded instance of the data saver for the current data provider.
		/// </summary>
		
		IDataSaver Saver {get;}
		/// <summary>
		/// Holds a JIT loaded instance of the data updater for the current data provider.
		/// </summary>
		IDataUpdater Updater {get;}
		
		/// <summary>
		/// Holds a JIT loaded instance of the data deleter for the current data provider.
		/// </summary>
		IDataDeleter Deleter {get;}
		
		
		/// <summary>
		/// Fired when the batch gets committed.
		/// </summary>
		event EventHandler Committed;
		
		/// <summary>
		/// Raises the committed event.
		/// </summary>
		void RaiseCommitted();
    }
}
