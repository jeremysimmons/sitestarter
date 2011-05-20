using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataStore.
	/// </summary>
	public abstract class DataStore : IDataStore
	{
		public DataStore()
		{
		}
				
		public abstract bool IsClosed {get;}
		
		public abstract void Commit(bool forceCommit);
		
		public abstract void Commit();
		
		public abstract bool IsStored(IEntity entity);
		
		/// <summary>
		/// Closes the data store.
		/// </summary>
		public abstract void Close();
		
		/// <summary>
		/// Disposes the data store. Also closes the data store and clears it from state.
		/// </summary>
		public abstract void Dispose();
		
		/// <summary>
		/// Opens the data store object. Doesn't necessarily have to open the actual underlying store yet, as it can possibly wait until it's needed.
		/// </summary>
		public abstract void Open();
		
		public abstract string Name {get;set;}
		
		#region Reader adapter
		private IDataReader reader;
		/// <summary>
		/// Holds a JIT loaded instance of the data reader for the current data provider.
		/// </summary>
		public IDataReader Reader
		{
			get {
				if (reader == null)
					reader = InitializeDataReader();
				return reader;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data reader adapter held by the Reader property.
		/// </summary>
		/// <returns>The provider specific Reader adapter.</returns>
		public virtual IDataReader InitializeDataReader()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		#region Indexer adapter
		
		private IDataIndexer indexer;
		/// <summary>
		/// Holds a JIT loaded instance of the data indexer for the current data provider.
		/// </summary>
		public IDataIndexer Indexer
		{
			get {
				if (indexer == null)
					indexer = InitializeDataIndexer();
				return indexer;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data indexer adapter held by the Indexer property.
		/// </summary>
		/// <returns>The provider specific Indexer adapter.</returns>
		public virtual IDataIndexer InitializeDataIndexer()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		#region Referencer adapter
		private IDataReferencer referencer;
		/// <summary>
		/// Holds a JIT loaded instance of the data referencer for the current data provider.
		/// </summary>
		public IDataReferencer Referencer
		{
			get {
				if (referencer == null)
					referencer = InitializeDataReferencer();
				return referencer;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data referencer adapter held by the Referencer property.
		/// </summary>
		/// <returns>The provider specific Referencer adapter.</returns>
		public virtual IDataReferencer InitializeDataReferencer()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		
		#region Activator adapter
		
		private IDataActivator activator;
		/// <summary>
		/// Holds a JIT loaded instance of the data activator for the current data provider.
		/// </summary>
		public IDataActivator Activator
		{
			get {
				if (activator == null)
					activator = InitializeDataActivator();
				return activator;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data activator adapter held by the Referencer property.
		/// </summary>
		/// <returns>The provider specific activator adapter.</returns>
		public virtual IDataActivator InitializeDataActivator()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		#region Saver adapter
		
		private IDataSaver saver;
		/// <summary>
		/// Holds a JIT loaded instance of the data saver for the current data provider.
		/// </summary>
		public IDataSaver Saver
		{
			get {
				if (saver == null)
					saver = InitializeDataSaver();
				return saver;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data saver adapter held by the Saver property.
		/// </summary>
		/// <returns>The provider specific Saver adapter.</returns>
		public virtual IDataSaver InitializeDataSaver()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		
		#region Updater adapter
		
		private IDataUpdater updater;
		/// <summary>
		/// Holds a JIT loaded instance of the data updater for the current data provider.
		/// </summary>
		public IDataUpdater Updater
		{
			get {
				if (updater == null)
					updater = InitializeDataUpdater();
				return updater;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data updater adapter held by the Updater property.
		/// </summary>
		/// <param name="store">The data store that the adapter is tied to, or [null] to detect automatically.</param>
		/// <returns>The provider specific Updater adapter.</returns>
		public virtual IDataUpdater InitializeDataUpdater()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		#region Deleter adapter
		
		private IDataDeleter deleter;
		/// <summary>
		/// Holds a JIT loaded instance of the data deleter for the current data provider.
		/// </summary>
		public IDataDeleter Deleter
		{
			get {
				if (deleter == null)
					deleter = InitializeDataDeleter();
				return deleter;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data deleter adapter held by the Deleter property.
		/// </summary>
		/// <returns>The provider specific Deleter adapter.</returns>
		public virtual IDataDeleter InitializeDataDeleter()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		
		/// <summary>
		/// Fired when the batch gets committed.
		/// </summary>
		public event EventHandler Committed;
		
		/// <summary>
		/// Raises the committed event.
		/// </summary>
		public virtual void RaiseCommitted()
		{
			if (Committed != null)
				Committed(this, EventArgs.Empty);
		}
	}
}
