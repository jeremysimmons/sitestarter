using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the interface required for all data providers.
	/// </summary>
	public abstract class DataProvider : ProviderBase
	{
		/// <summary>
		/// Gets the path to the data directory.
		/// </summary>
		public string DataDirectoryPath
		{
			get { return StateAccess.State.PhysicalApplicationPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "App_Data"; }
		}
		
		/// <summary>
		/// Gets the path to the data directory.
		/// </summary>
		public string SuspendedDirectoryPath
		{
			get { return DataDirectoryPath + Path.DirectorySeparatorChar + "Suspended"; }
		}
		
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
		
		#region Schema adapter
		
		private IDataSchema schema;
		/// <summary>
		/// Holds a JIT loaded instance of the data schema editor for the current data provider.
		/// </summary>
		public IDataSchema Schema
		{
			get {
				if (schema == null)
					schema = InitializeDataSchema();
				return schema;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data schema adapter held by the Schema property.
		/// </summary>
		/// <returns>The provider specific Schema adapter.</returns>
		public virtual IDataSchema InitializeDataSchema()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		
		#region Export adapter
		
		private IDataExporter exporter;
		/// <summary>
		/// Holds a JIT loaded instance of the data exporter for the current data provider.
		/// </summary>
		public IDataExporter Exporter
		{
			get {
				if (exporter == null)
					exporter = InitializeDataExporter();
				return exporter;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data exporter adapter held by the Exporter property.
		/// </summary>
		/// <returns>The provider specific Exporter adapter.</returns>
		public virtual IDataExporter InitializeDataExporter()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		
		#region Import adapter
		
		private IDataImporter importer;
		/// <summary>
		/// Holds a JIT loaded instance of the data importer for the current data provider.
		/// </summary>
		public IDataImporter Importer
		{
			get {
				if (importer == null)
					importer = InitializeDataImporter();
				return importer;  }
		}
		
		/// <summary>
		/// Must be overridden. Initializes the data importer adapter held by the Importer property.
		/// </summary>
		/// <returns>The provider specific Importer adapter.</returns>
		public virtual IDataImporter InitializeDataImporter()
		{
			throw new NotImplementedException("This method should be overridden.");
		}
		#endregion
		
		
		
		private DataStoreCollection stores;
		/// <summary>
		/// Gets/sets a collection of the available data stores.
		/// </summary>
		public virtual DataStoreCollection Stores
		{
			get {
				if (stores == null)
					stores = new DataStoreCollection();
				return stores; }
		}
		
		/// <summary>
		/// Disposes the data provider.
		/// </summary>
		/// <param name="fullDisposal">Whether to dispose the inner components.</param>
		/// <param name="commit">A value indicating whether to commit the stores on dispose.</param>
		public abstract void Dispose(bool fullDisposal, bool commit);
		
		/// <summary>
		/// Checks whether the provided entity or entity reference is currently found in the corresponding data store.
		/// </summary>
		/// <param name="entity">The entity to look for.</param>
		/// <returns>A bool value indicating whether the entity can be found.</returns>
		public abstract bool IsStored(IEntity entity);

		/// <summary>
		/// Initializes the data store with the provided name.
		/// </summary>
		/// <param name="dataStoreName">The name of the data store to initialize.</param>
		/// <returns>The initialized data store.</returns>
		public abstract IDataStore InitializeDataStore(string dataStoreName);
		
		/// <summary>
		/// Retrieves the names of all the data stores that can be found.
		/// </summary>
		/// <returns>An array containing the names of the data stores.</returns>
		public abstract string[] GetDataStoreNames();

		/// <summary>
		/// Creates a data filter object derived from the specified base type but specific to the data access provider currently configured.
		/// </summary>
		/// <param name="baseType">The base filter type that is inherited by the provider specific filter being created.</param>
		/// <returns>A data access provider specific data filter determined by the specified based type.</returns>
		public abstract IDataFilter CreateFilter(Type baseType);
		
		/// <summary>
		/// Creates a filter object used to match entities by checking that a specified property matches a specified value.
		/// </summary>
		/// <returns></returns>
		public virtual PropertyFilter CreatePropertyFilter()
		{
			return (PropertyFilter)CreateFilter(typeof(PropertyFilter));
		}
		
		/// <summary>
		/// Creates a filter object used to match entities by checking that a specified reference property is associated with a specified referenced entity.
		/// </summary>
		/// <returns></returns>
		public virtual ReferenceFilter CreateReferenceFilter()
		{
			return (ReferenceFilter)CreateFilter(typeof(ReferenceFilter));
		}
		
		/// <summary>
		/// Creates a filter object used to match entities by checking that a specified property matches a specified value.
		/// </summary>
		/// <returns></returns>
		public virtual FilterGroup CreateFilterGroup()
		{
			return new FilterGroup();
		}
		
		
		/// <summary>
		/// Disposes the data provider and suspends the data stores by moving them to a save location outside the application.
		/// Note: This is used during an update to safely clear the data stores and allow the updated data to be imported without conflicts.
		/// </summary>
		public abstract void Suspend();
		
	}
}
