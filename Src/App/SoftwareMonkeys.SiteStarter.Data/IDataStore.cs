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
        string Name { get; }
        #endregion

        #region Initialize/open/close/dispose functions
        void Open();
        void Dispose();
        void Close();
        #endregion

        bool IsStored(IEntity entity);
        
        void Commit();
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
    }
}
