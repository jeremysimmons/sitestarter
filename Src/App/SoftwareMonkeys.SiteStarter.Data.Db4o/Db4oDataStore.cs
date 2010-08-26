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
		private string virtualServerID;
		/// <summary>
		/// Gets/sets the ID of the virtual server that the data store is attached to.
		/// </summary>
		public override string VirtualServerID
		{
			get { return virtualServerID; }
			set { virtualServerID = value; }
		}
		
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
				    || StateAccess.State.GetApplication(ObjectContainerKey) == null)
					OpenServer();
				
				return (IObjectServer)StateAccess.State.GetApplication(ObjectServerKey);
			}
			set {
				StateAccess.State.SetApplication(ObjectServerKey, value);}
		}
		
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
				if (!StateAccess.State.ContainsApplication(ObjectContainerKey)
				    || StateAccess.State.GetApplication(ObjectContainerKey) == null)
					OpenContainer();
				
				return (IObjectContainer)StateAccess.State.GetApplication(ObjectContainerKey);
			}
			set { StateAccess.State.SetApplication(ObjectContainerKey, value); }
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
			using (LogGroup logGroup = AppLogger.StartGroup("Constructing a Db4oDataStore object.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Empty constructor.");
			}
		}
		
		/// <summary>
		/// Sets the db4o configuration to be used by the data store.
		/// </summary>
		/// <param name="db4oConfiguration"></param>
		// TODO: See if this is necessary. If not, remove it
		public Db4oDataStore(IConfiguration db4oConfiguration)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Constructing a Db4oDataStore object.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Setting the db4o configuration object.");
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
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the file name for the data store.", NLog.LogLevel.Debug))
			{
				string fileName = Name;
				
				AppLogger.Debug("Store name: " + Name);
				
				// Add the path variation
				if (Config.Application.PathVariation != String.Empty)
					fileName = fileName + "." + Config.Application.PathVariation;
				
				// Add the extension
				fileName = fileName +  ".yap";
				
				path = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar
					+ fileName;
				
				AppLogger.Debug("Path: " + path);
				
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
			using (LogGroup logGroup = AppLogger.StartGroup("Opening data server.", NLog.LogLevel.Info))
			{
				Db4oFactory.Configure().ActivationDepth(2);
				Db4oFactory.Configure().UpdateDepth(0);
				
				Db4oFactory.Configure().ObjectClass(typeof(IEntity)).ObjectField("id").Indexed(true);
				
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("property1Name").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("type1Name").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("entity1ID").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("property2Name").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("type2Name").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("entity2ID").Indexed(true);
				

				string fullName = GetStoreFileName();
				
				
				AppLogger.Debug("Full file name: " + fullName);
				
				if (!Directory.Exists(Path.GetDirectoryName(fullName)))
					Directory.CreateDirectory(Path.GetDirectoryName(fullName));
				
				ObjectServer = Db4oFactory.OpenServer(Db4oFactory.NewConfiguration(),
				                                      fullName, 0);
				
				AppLogger.Debug("Server opened");
				//objectContainer = ObjectServer.OpenClient();
			}
		}
		
		
		/// <summary>
		/// Opens the db4o object container.
		/// </summary>
		public void OpenContainer()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Opening data container.", NLog.LogLevel.Info))
			{
				string fileName = Name;
				
				AppLogger.Debug("Name: " + Name);


				string fullName = GetStoreFileName();
				
				AppLogger.Debug("Full file name: " + fullName);
				
				if (!Directory.Exists(Path.GetDirectoryName(fullName)))
					Directory.CreateDirectory(Path.GetDirectoryName(fullName));
				//ObjectServer = Db4oFactory.OpenServer(fullName, 0);
				ObjectContainer = ObjectServer.OpenClient();//Db4oFactory.OpenFile(fullName);
				
				AppLogger.Debug("Container opened");
			}
		}

		/// <summary>
		/// Disposes the data store. Also closes the data store and clears it from state.
		/// </summary>
		public override void Dispose()
		{
			// Don't commit/close here. Commit must be explicit.
			//Close();
			
			// Roll back to the last commit
			// This roll back is important. The data store must not commit the latest data unless commit call is explicit.
			// If rollback is not called then the latest data will be automatically committed
			// The ability to dispose without committing is necessary for unit testing and/or transactions
			ObjectContainer.Rollback();
			
			ObjectContainer.Dispose(); 
			ObjectServer.Close(); // ObjectServer must be closed to unlock files.
			ObjectServer = null;
			
			StateAccess.State.SetApplication(ObjectContainerKey, null);
			StateAccess.State.SetApplication(ObjectServerKey, null);
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
			//ObjectServer.Close();
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
			
			return ObjectContainer.Ext().IsStored(entity);
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

		/*public IEntity GetEntity(IObjectSet os)
		{
			throw new NotImplementedException();
		}*/
		#endregion

		/*static public string ToCamelCase(string text)
		{
			// TODO: Check if this is done properly
			if (text == string.Empty)
				return String.Empty;

			string firstChar = text.Substring(0, 1);

			text = text.Substring(1, text.Length - 1);

			text = firstChar.ToLower() + text;

			return text;
		}*/

		
		/*public T[] GetEntitiesPage<T>(PagingLocation location, string sortExpression)
			where T : IEntity
		{
			int total = 0;
			
			T[] output = GetEntitiesPage<T>(location.PageIndex, location.PageSize, sortExpression, out total);
			
			location.AbsoluteTotal = total;
			
			return output;
		}*/
		/*
		public T[] GetEntitiesPage<T>(string fieldName, object fieldValue, PagingLocation location, string sortExpression)
			where T : IEntity
		{
			
			int total = 0;
			
			
			T[] output = GetEntitiesPage<T>(fieldName, fieldValue, location.PageIndex, location.PageSize, sortExpression, out total);
			
			location.AbsoluteTotal = total;
			
			return output;
		}*/
		
		
		public override void Commit()
		{
			Commit(false);
		}
		
		
		public override void Commit(bool forceCommit)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Committing the data store (or adding to batch for later).", NLog.LogLevel.Debug))
			{
				// Only commit if there's no batch running
				if (forceCommit || !BatchState.IsRunning)
				{
					AppLogger.Debug("No batch running. Committing immediately.");
					
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
					AppLogger.Debug("Batch running. Adding data source to batch. It will be committed when the batch is over.");
					
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
