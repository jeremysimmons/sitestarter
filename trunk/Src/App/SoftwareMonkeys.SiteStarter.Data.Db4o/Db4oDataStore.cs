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
				IObjectServer server = (IObjectServer)StateAccess.State.GetApplication(ObjectServerKey);
				if (server == null)
					OpenServer();
				return (IObjectServer)StateAccess.State.GetApplication(ObjectServerKey);
			}
			set {
				StateAccess.State.SetApplication(ObjectServerKey, value);}
		}
		
		public string ObjectServerKey
		{
			get
			{
				string key = "ObjectServer_" + this.Name;
				string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
				if (prefix != null && prefix != String.Empty)
				{
					key = prefix + "_" + key;
				}
				return key;
			}
		}

		public string ObjectContainerKey
		{
			get
			{
				string key = "ObjectContainer_" + this.Name;
				string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
				if (prefix != null && prefix != String.Empty)
				{
					key = prefix + "_" + key;
				}
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
				IObjectContainer objectContainer = (IObjectContainer)StateAccess.State.GetApplication(ObjectContainerKey);
				if (objectContainer == null)
					OpenContainer();
				
				return (IObjectContainer)StateAccess.State.GetApplication(ObjectContainerKey);
				//return (IObjectContainer)StateAccess.State.GetApplication(ObjectContainerKey);
				//return ObjectServer.OpenClient();
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
		
		public Db4oDataStore()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Constructing a Db4oDataStore object.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Empty constructor.");
			}
		}
		
		public Db4oDataStore(IConfiguration db4oConfiguration)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Constructing a Db4oDataStore object.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Setting the db4o configuration object.");
				this.db4oConfiguration = db4oConfiguration;
			}
		}

		private string GetStoreFileName()
		{
			string fileName = Name;
			
			AppLogger.Debug("Store name: " + Name);

			string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
			if (prefix != null && prefix != String.Empty && prefix != Guid.Empty.ToString())
			{
				fileName = @"VS\" + prefix + @"\" + fileName;
			}
			
			return Config.Application.PhysicalPath + @"\App_Data\" + fileName + ".yap";
		}
		
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
		
		public override void Open()
		{
			// Store is not opened here as it's not necessary.
			// It's opened JIT
		}
		
		public void OpenContainer()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Opening data container.", NLog.LogLevel.Info))
			{
				string fileName = Name;
				
				AppLogger.Debug("Name: " + Name);

				string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
				if (prefix != null && prefix != String.Empty && prefix != Guid.Empty.ToString())
				{
					fileName = @"VS\" + prefix + @"\" + fileName;
				}

				string fullName = Config.Application.PhysicalPath + @"\App_Data\" + fileName + ".yap";
				
				AppLogger.Debug("Full file name: " + fullName);
				
				if (!Directory.Exists(Path.GetDirectoryName(fullName)))
					Directory.CreateDirectory(Path.GetDirectoryName(fullName));
				//ObjectServer = Db4oFactory.OpenServer(fullName, 0);
				ObjectContainer = ObjectServer.OpenClient();//Db4oFactory.OpenFile(fullName);
				
				AppLogger.Debug("Container opened");
			}
		}

		public override void Dispose()
		{
			Close();
			StateAccess.State.SetApplication(ObjectContainerKey, null);
			//objectServer = null;
		}

		public override void Close()
		{
			if (ObjectContainer != null && !ObjectContainer.Ext().IsClosed())
			{
				ObjectContainer.Commit();
				ObjectContainer.Close();
				ObjectContainer = null;
				ObjectServer.Close();
				ObjectServer.Dispose();
				ObjectServer = null;
			}
			//ObjectServer.Close();
		}

		#endregion





		public override bool IsStored(IEntity entity)
		{
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
			Commit(true);
		}
		
		
		public override void Commit(bool forceCommit)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Committing the data store (or adding to batch for later).", NLog.LogLevel.Debug))
			{
				// Only commit if there's no batch running
				if (forceCommit || !Batch.IsRunning)
				{
					AppLogger.Debug("No batch running. Committing immediately.");
					
					if (ObjectContainer != null)
						ObjectContainer.Commit();
					else
						throw new InvalidOperationException("ObjectContainer == null");
				}
				// If a batch is running then the commit should be skipped. It'll be commit once the batch is complete.
				else
				{
					AppLogger.Debug("Batch running. Adding data source to batch. It will be committed when the batch is over.");
					
					Batch.Handle(this);
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
