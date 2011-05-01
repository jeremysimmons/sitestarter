using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using Db4objects.Db4o.Query;
using Db4objects.Db4o;
using System.Collections;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.State;
using System.IO;
using Db4objects.Db4o.Config;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	public class Db4oDataStore : DataStore
	{
		// TODO: Check if this is needed
		private IConfiguration db4oConfiguration;
		
		public override bool IsClosed
		{
			get {
				if (ObjectContainer == null || ObjectServer == null)
					return true;
				else
					return ObjectContainer.Ext().IsClosed();
			}
		}
		
		/// <summary>
		/// Gets a flag indicating whether the actual data store file exists.
		/// If it doesn't exist it should mean no data has been created.
		/// </summary>
		public bool DoesExist
		{
			get
			{
				string path = GetStoreFileName();
				
				return File.Exists(path);
			}
		}
		
		private IQuery activeQuery;
		/// <summary>
		/// Gets/sets the active db4o query.
		/// </summary>
		public IQuery ActiveQuery
		{
			get
			{
				if (activeQuery == null)
					activeQuery = ObjectContainer.Query();
				return activeQuery;
			}
			set { activeQuery = value; }
		}
		
		/// <summary>
		/// Gets/sets the corresponding db4o object container.
		/// </summary>
		public IObjectServer ObjectServer
		{
			get
			{
				if (!StateAccess.State.ContainsApplication(ObjectServerKey)
				    || StateAccess.State.GetApplication(ObjectServerKey) == null)
					OpenServer();
				
				return (IObjectServer)StateAccess.State.GetApplication(ObjectServerKey);
			}
			set {
				StateAccess.State.SetApplication(ObjectServerKey, value);}
		}
		
		public int MaxOpenRetries = 10;
		
		/// <summary>
		/// Gets the key that represents the object server when held in the state store.
		/// </summary>
		public string ObjectServerKey
		{
			get
			{
				string key = "ObjectServer_" + this.Name;
				
				return key;
			}
		}

		/// <summary>
		/// Gets the key that represents the object container when held in the state store.
		/// </summary>
		public string ObjectContainerKey
		{
			get
			{
				string key = "ObjectContainer_" + this.Name;
				
				return key;
			}
		}

		/// <summary>
		/// Gets/sets the corresponding db4o object container.
		/// </summary>
		public IObjectContainer ObjectContainer
		{
			get
			{
				if (!StateAccess.State.ContainsOperation(ObjectContainerKey)
				    || StateAccess.State.GetOperation(ObjectContainerKey) == null)
					OpenContainer();
				
				return (IObjectContainer)StateAccess.State.GetOperation(ObjectContainerKey);
			}
			set { StateAccess.State.SetOperation(ObjectContainerKey, value); }
		}

		#region IDataStore Members

		private string name;
		/// <summary>
		/// Gets/sets the name of the data store.
		/// </summary>
		public override string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public Db4oDataStore()
		{
			using (LogGroup logGroup = LogGroup.Start("Constructing a Db4oDataStore object.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Empty constructor.");
			}
		}
		
		/// <summary>
		/// Sets the db4o configuration to be used by the data store.
		/// </summary>
		/// <param name="db4oConfiguration"></param>
		// TODO: See if this is necessary. If not, remove it
		public Db4oDataStore(IConfiguration db4oConfiguration)
		{
			using (LogGroup logGroup = LogGroup.Start("Constructing a Db4oDataStore object.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Setting the db4o configuration object.");
				this.db4oConfiguration = db4oConfiguration;
			}
		}

		/// <summary>
		/// Retrieves the file name of the physical data store.
		/// </summary>
		/// <returns>The file name for the physical data store.</returns>
		private string GetStoreFileName()
		{
			string path = String.Empty;
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving the file name for the data store.", NLog.LogLevel.Debug))
			{
				string fileName = Name;
				
				LogWriter.Debug("Store name: " + Name);
				
				// Add the path variation
				if (Config.Application.PathVariation != String.Empty)
					fileName = fileName + "." + Config.Application.PathVariation;
				
				// Add the extension
				fileName = fileName +  ".yap";
				
				path = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar
					+ fileName;
				
				LogWriter.Debug("Path: " + path);
				
			}
			return path;
		}
		
		/// <summary>
		/// Opens the data store object. This does not open the actual store as it's done when it's needed.
		/// </summary>
		public override void Open()
		{
			// Store is not opened here as it's not necessary.
			// It's opened JIT
		}
		
		/// <summary>
		/// Opens the db4o server.
		/// </summary>
		public void OpenServer()
		{
			using (LogGroup logGroup = LogGroup.Start("Opening data server.", NLog.LogLevel.Info))
			{
				LogWriter.Info("Opening db4o object server: " + Name);
				ObjectServer = new Db4oDataStoreOpener().TryOpenServer(GetStoreFileName(), MaxOpenRetries);
			}
		}
		
		
		/// <summary>
		/// Opens the db4o object container.
		/// </summary>
		public void OpenContainer()
		{
			using (LogGroup logGroup = LogGroup.Start("Opening data container.", NLog.LogLevel.Info))
			{
				LogWriter.Info("Opening db4o object container: " + Name);
				
				string fileName = Name;

				string fullName = GetStoreFileName();
				
				LogWriter.Debug("Full file name: " + fullName);
				
				// Get the server (this activates JIT loading if necessary)
				IObjectServer server = ObjectServer;
				
				if (!Directory.Exists(Path.GetDirectoryName(fullName)))
					Directory.CreateDirectory(Path.GetDirectoryName(fullName));
				
				// This info should come after the JIT loading of the object server (to make them show up in the right order in the logs)
				LogWriter.Debug("Opening db4o object container: " + Name);
				
				ObjectContainer = server.OpenClient();
			}
		}

		/// <summary>
		/// Disposes the data store. Also closes the data store and clears it from state.
		/// </summary>
		public override void Dispose()
		{
			// Don't commit/close here. Commit must be explicit.
			//Close();
			
			// Dispose the container
			if (ObjectContainer != null && !ObjectContainer.Ext().IsClosed())
			{
				// Roll back to the last commit
				// This roll back is important. The data store must not commit the latest data unless commit call is explicit.
				// If rollback is not called then the latest data will be automatically committed
				// The ability to dispose without committing is necessary for unit testing, transactions, etc.
				ObjectContainer.Rollback();
				// TODO: Add a property to specify whether or not to automatically roll back
				
				ObjectContainer.Dispose();
			}
			
			// Dispose the server
			if (ObjectServer != null)
			{
				ObjectServer.Close(); // ObjectServer must be closed to unlock files.
				ObjectServer.Dispose();
				ObjectServer = null;
				
				StateAccess.State.SetApplication(ObjectContainerKey, null);
				StateAccess.State.SetApplication(ObjectServerKey, null);
			}
			
		}

		/// <summary>
		/// Closes the data store.
		/// </summary>
		public override void Close()
		{
			if (ObjectContainer != null && !ObjectContainer.Ext().IsClosed())
			{
				ObjectContainer.Commit();
				ObjectContainer.Close();
				ObjectContainer = null;
				ObjectServer.Close();
				ObjectServer = null;
			}
		}

		#endregion



		
		/// <summary>
		/// Checks whether the provided entity is current in the data store.
		/// </summary>
		/// <param name="entity">The entity to look for in the data store.</param>
		/// <returns>A boolean value indicating whether the provided entity was found in the data store.</returns>
		public override bool IsStored(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (ObjectContainer == null)
				throw new InvalidOperationException("The ObjectContainer has not been initialized.");
			
			bool foundBound = ObjectContainer.Ext().IsStored(entity);
			
			bool foundByID = (Reader.GetEntity(entity.GetType(), "ID", entity.ID) != null);
			
			return foundBound
				|| foundByID;
		}
		

		#region Db4o specific functions

		/// <summary>
		/// Creates a new query in the data store that's constrained to the specified type.
		/// </summary>
		/// <returns>The newly created query.</returns>
		public IQuery Query<T>()
		{
			IQuery query = ObjectContainer.Query();
			query.Constrain(typeof(T));
			return query;
		}
		#endregion
		
		public override void Commit()
		{
			Commit(false);
		}
		
		
		public override void Commit(bool forceCommit)
		{
			using (LogGroup logGroup = LogGroup.Start("Committing the data store (or adding to batch for later).", NLog.LogLevel.Debug))
			{
				// Only commit if there's no batch running
				if (forceCommit || !BatchState.IsRunning)
				{
					LogWriter.Debug("No batch running. Committing immediately.");
					
					if (ObjectContainer != null)
					{
						ObjectContainer.Commit();
						RaiseCommitted();
					}
					else
						throw new InvalidOperationException("ObjectContainer == null");
				}
				// If a batch is running then the commit should be skipped. It'll be commit once the batch is complete.
				else
				{
					LogWriter.Debug("Batch running. Adding data source to batch. It will be committed when the batch is over.");
					
					BatchState.Handle(this);
				}
			}
		}
		
		
		#region Adapter functions
		public override IDataReader InitializeDataReader()
		{
			return new Db4oDataReader((Db4oDataProvider)DataAccess.Data, this);
		}
		
		public override IDataIndexer InitializeDataIndexer()
		{
			return new Db4oDataIndexer((Db4oDataProvider)DataAccess.Data, this);
		}
		
		public override IDataReferencer InitializeDataReferencer()
		{
			return new Db4oDataReferencer((Db4oDataProvider)DataAccess.Data, this);
		}
		
		public override IDataActivator InitializeDataActivator()
		{
			return new Db4oDataActivator((Db4oDataProvider)DataAccess.Data, this);
		}
		public override IDataSaver InitializeDataSaver()
		{
			return new Db4oDataSaver((Db4oDataProvider)DataAccess.Data, this);
		}
		public override IDataDeleter InitializeDataDeleter()
		{
			return new Db4oDataDeleter((Db4oDataProvider)DataAccess.Data, this);
		}
		public override IDataUpdater InitializeDataUpdater()
		{
			return new Db4oDataUpdater((Db4oDataProvider)DataAccess.Data, this);
		}
		
		#endregion

	}
}
