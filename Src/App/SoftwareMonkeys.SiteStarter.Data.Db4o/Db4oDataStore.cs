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
				if (!IsContainerInitialized)
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
				if (!IsServerInitialized)
					OpenServer();
				
				return (IObjectServer)StateAccess.State.GetApplication(ObjectServerKey);
			}
			set {
				StateAccess.State.SetApplication(ObjectServerKey, value);}
		}
		
		public int MaxOpenRetries = 1000;
		
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
				if (!IsContainerInitialized)
					OpenContainer();
				
				return (IObjectContainer)StateAccess.State.GetOperation(ObjectContainerKey);
			}
			set { StateAccess.State.SetOperation(ObjectContainerKey, value); }
		}
		
		public bool IsContainerInitialized
		{
			get
			{
				return IsServerInitialized
					&& StateAccess.State.ContainsOperation(ObjectContainerKey) // Container is found in state
				    && StateAccess.State.GetOperation(ObjectContainerKey) != null // Container entry in state is not null
				    && !((IObjectContainer)StateAccess.State.GetOperation(ObjectContainerKey)).Ext().IsClosed(); // Container is not closed
			}
		}
		
		public bool IsServerInitialized
		{
			get
			{
				return StateAccess.State.ContainsApplication(ObjectServerKey)
				    && StateAccess.State.GetApplication(ObjectServerKey) != null;
			}
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
			using (LogGroup logGroup = LogGroup.StartDebug("Constructing a Db4oDataStore object."))
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
			using (LogGroup logGroup = LogGroup.StartDebug("Constructing a Db4oDataStore object."))
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
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the file name for the data store."))
			{
				string fileName = Name;
				
				LogWriter.Debug("Store name: " + Name);
				
				// Add the path variation
				if (Config.Application.PathVariation != String.Empty)
					fileName = fileName + "." + Config.Application.PathVariation;
				
				// Add the extension
				fileName = fileName +  ".db4o";
				
				path = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar
					+ fileName;
				
				LogWriter.Debug("Path: " + path);
				
			}
			return path;
		}
		
		/// <summary>
		/// Opens the data store object.
		/// </summary>
		public override void Open()
		{
			OpenContainer();
		}
		
		/// <summary>
		/// Opens the db4o server.
		/// </summary>
		public void OpenServer()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Opening db4o data server."))
			{				
				LogWriter.Debug("${db4o.OpenServer:" + Name + "}");
				
				ObjectServer = new Db4oDataStoreOpener().TryOpenServer(GetStoreFileName(), MaxOpenRetries);
			}
		}
		
		
		/// <summary>
		/// Opens the db4o object container.
		/// </summary>
		public void OpenContainer()
		{
			//using (LogGroup logGroup = LogGroup.StartDebug("Opening db4o data container."))
			//{			
				LogWriter.Debug("${db4o.OpenContainer:" + Name + "}");
				
				string fileName = Name;

				string fullName = GetStoreFileName();
				
			//	LogWriter.Debug("Full file name: " + fullName);
				
				// Get the server (this activates JIT loading if necessary)
				IObjectServer server = ObjectServer;
				
				if (!Directory.Exists(Path.GetDirectoryName(fullName)))
					Directory.CreateDirectory(Path.GetDirectoryName(fullName));
				
				// This info should come after the JIT loading of the object server (to make them show up in the right order in the logs)
			//	LogWriter.Debug("Opening db4o object container: " + Name);
				
				if (server != null)
					ObjectContainer = server.OpenClient();
				else
					LogWriter.Error("Can't open container because server is not initialized.");
			//}
		}
		
		/// <summary>
		/// Disposes the data store but not the inner components.
		/// </summary>
		public override void Dispose()
		{
			Dispose(false);
		}

		/// <summary>
		/// Disposes the data store, and if specified, also closes the data store and clears it from state.
		/// </summary>
		public void Dispose(bool fullDisposal)
		{
			if (fullDisposal)
			{
				// If the container is initialized
				if (IsContainerInitialized)
				{
					// Roll back to the last commit
					// This roll back is important. The data store must not commit the latest data unless commit call is explicit.
					// If rollback is not called then the latest data will be automatically committed
					// The ability to dispose without committing is necessary for unit testing, transactions, etc.
					ObjectContainer.Rollback();
					// TODO: Add a property to specify whether or not to automatically roll back
					
				// Dispose the container
					ObjectContainer.Dispose();
				}
				
				// Dispose the server
				if (IsServerInitialized)
				{
					ObjectServer.Close(); // ObjectServer must be closed to unlock files.
					ObjectServer.Dispose();
					ObjectServer = null;
				}
				
				StateAccess.State.SetApplication(ObjectContainerKey, null);
				StateAccess.State.SetApplication(ObjectServerKey, null);
			}
			
		}

		/// <summary>
		/// Closes the data store.
		/// </summary>
		public override void Close()
		{
			if (IsContainerInitialized)
			{
				ObjectContainer.Close();
				ObjectContainer = null;
			}
			if (IsServerInitialized)
			{
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
			
			if (foundBound)
				return true;
			else
			{
				bool foundByID = (Reader.GetEntity(entity.GetType(), "ID", entity.ID) != null);
			
				return foundByID;
			}
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
			using (LogGroup logGroup = LogGroup.StartDebug("Committing the data store (or adding to batch for later)."))
			{
				// Only commit if there's no batch running
				if (forceCommit || !BatchState.IsRunning)
				{
					LogWriter.Debug("No batch running. Committing immediately.");
					
					if (ObjectContainer != null)
					{
						if (!ObjectContainer.Ext().IsClosed())
						{
							LogWriter.Debug("Committing.");
							
							ObjectContainer.Commit();
							RaiseCommitted();
						}
						else
							LogWriter.Debug("Can't commit. The data store is closed.");
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
