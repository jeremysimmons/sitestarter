using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used as a base for all factory objects.
	/// </summary>
	[DataObject(true)]
	public class BaseFactory : IFactory
	{
		public string Name
		{
			get { return this.GetType().Name; }
		}
		
		private CommandManager commands;
		public CommandManager Commands
		{
			get {
				if (commands == null)
					commands = new CommandManager(this);
				return commands; }
			set { commands = value; }
		}
		
		private Dictionary<string, Type> defaultTypes;
		public virtual Dictionary<string, Type> DefaultTypes
		{
			get { return defaultTypes; }
			set { defaultTypes = value; }
		}
		
		#region Activate functions
		/// <summary>
		/// Activates the entity by loading all references.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		public virtual void Activate(IEntity entity)
		{
			Activate(entity, 1);
		}
		
		/// <summary>
		/// Activates the entity by loading all references to the specified depth.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		public virtual void Activate(IEntity entity, int depth)
		{
			foreach (PropertyInfo property in entity.GetType().GetProperties())
			{
				if (EntitiesUtilities.IsReference(entity.GetType(), property))
				{
					Activate(entity, property.Name,
					         depth-1); // We've stepped down one level so reduce the depth
				}
			}
		}
		
		/// <summary>
		/// Activates all the entities provided by loading all references for each entity.
		/// </summary>
		/// <param name="entities">The array of entities to activate.</param>
		public virtual void Activate(Array entities)
		{
			Activate(entities, 1);
		}
		
		/// <summary>
		/// Activates all the entities provided by loading all references for each entity to the specified depth.
		/// </summary>
		/// <param name="entities">The array of entities to activate.</param>
		public virtual void Activate(Array entities, int depth)
		{
			if (entities != null)
			{
				foreach (IEntity entity in entities)
				{
					if (entity != null)
						Activate(entity, depth);
				}
			}
		}
		
		/// <summary>
		/// Activates the specified property of all the entities provided, by loading all references for the specified property of each entity.
		/// </summary>
		/// <param name="entities">The array of entities to activate.</param>
		/// <param name="propertyName">The name of the property to activate.</param>
		public virtual void Activate(Array entities, string propertyName)
		{
			Activate(entities, propertyName, 1);
		}
		
		
		/// <summary>
		/// Activates the specified property of all the entities provided, by loading all references for the specified property of each entity.
		/// </summary>
		/// <param name="entities">The array of entities to activate.</param>
		/// <param name="propertyName">The name of the property to activate.</param>
		public virtual void Activate(Array entities, string propertyName, int depth)
		{
			if (entities != null)
			{
				foreach (IEntity entity in entities)
				{
					if (entity != null)
						Activate(entity, propertyName, depth-1);
				}
			}
		}
		
		public virtual void Activate(IEntity entity, string propertyName)
		{
			Activate(entity, propertyName, 1);
		}
		
		/// <summary>
		/// Activates the specified property of the provided entity by loading the references for the property.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		/// <param name="propertyName">The name of the property to activate.</param>
		public virtual void Activate(IEntity entity, string propertyName, int depth)
		{
			DataAccess.Data.Activate(entity, propertyName, depth);
		}
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Activates all the entities provided by loading all references for each entity.
		/// </summary>
		/// <param name="entities">The array of entities to activate.</param>
		public virtual void Activate(object entities)
		{
			if (typeof(IEntity).IsAssignableFrom(entities.GetType()))
			{
				Activate((IEntity)entities);
			}
			else if (typeof(IEnumerable).IsAssignableFrom(entities.GetType()))
			{
				foreach (object entity in (IEnumerable)entities)
				{
					if (entity != null)
					{
						if (entity is IEntity)
							Activate((IEntity)entity);
					}
				}
			}
		}*/
			#endregion
			
			#region Get entity functions
			public virtual T Get<T>(string uniqueKey)
		{
			return (T)Get(typeof(T), uniqueKey);
		}
		
		public virtual IEntity DefaultGet(Type type, string uniqueKey)
		{
			if (uniqueKey == String.Empty)
				return null;

			return (IEntity)DataAccess.Data.GetEntity(type, "UniqueKey", uniqueKey);
		}
		
		
		public virtual IEntity DefaultGet(Type type, Guid id)
		{
			if (id == Guid.Empty)
				return null;

			return (IEntity)DataAccess.Data.GetEntity(type, "ID", id);
		}
		public virtual IEntity Get(Type type, string uniqueKey)
		{
			object entity = null;
			
			if (!TryExecute("View", type.Name, out entity, new object[] {uniqueKey}))
			{
				entity = DefaultGet(type, uniqueKey);
			}
			
			if (entity is IEntity)
				return (IEntity)entity;
			else
				throw new InvalidOperationException("Invalid type: " + (entity == null ? "[null]" : entity.GetType().ToString()));
		}
		
		public virtual T Get<T>(Guid id)
		{
			return (T)Get(typeof(T), id);
		}
		
		
		public virtual IEntity Get(Type type, Guid id)
		{
			object entity = null;
			
			if (!TryExecute("View", type.Name, out entity, new object[] {id}))
			{
				entity = DefaultGet(type, id);
			}
			
			if (entity is IEntity)
				return (IEntity)entity;
			else
				throw new InvalidOperationException("Invalid type: " + (entity == null ? "[null]" : entity.GetType().ToString()));
		}
		
		#endregion
		
		#region Get entities page functions
		/*public virtual IEntity[] DefaultGetPage(Type type, PagingLocation location, string sortExpression)
		{
			return DefaultGetPage(
		}*/
		
		public virtual IEntity[] DefaultGetPage(Type type, PagingLocation location, string sortExpression)
		{
			IEntity[] entities = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Executing default function to retrieve a page of entities.", NLog.LogLevel.Debug))
			{
				if (location == null)
					throw new ArgumentNullException("location");
				
				AppLogger.Debug("Type: " + type.ToString());
				AppLogger.Debug("Page size: " + location.PageSize);
				AppLogger.Debug("Page index: " + location.PageIndex);
				AppLogger.Debug("Sort expression: " + sortExpression);
				
				entities = (IEntity[])DataAccess.Data.GetEntitiesPage(type, location, sortExpression);
				
				AppLogger.Debug("Entities #: " + (entities == null ? "[null]" : entities.Length.ToString()));
			}
			return entities;
		}
		
		
		public virtual T[] GetPage<T>(PagingLocation location, string sortExpression)
			where T : IEntity
		{
			T[] entities = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving a page of entities.", NLog.LogLevel.Debug))
			{
				if (location == null)
					throw new ArgumentNullException("location");
				
				AppLogger.Debug("Type: " + typeof(T).ToString());
				
				AppLogger.Debug("Page index: " + location.PageIndex.ToString());
				AppLogger.Debug("Page size: " + location.PageSize.ToString());
				AppLogger.Debug("Sort expression: " + sortExpression);
				
				entities = Collection<T>.ConvertAll(GetPage(typeof(T), location, sortExpression));
				
				AppLogger.Debug("Entities #: " + entities.Length.ToString());
			}
			return entities;
		}
		
		
		IEntity[] IFactory.GetPage(Type type, IPagingLocation location, string sortExpression)
		{
			return GetPage(type, (PagingLocation)location, sortExpression);
		}
		
		
		IEntity[] IFactory.GetPage(Type type, string propertyName, object propertyValue, IPagingLocation location, string sortExpression)
		{
			return GetPage(type, propertyName, propertyValue, (PagingLocation)location, sortExpression);
		}
		
		IEntity[] IFactory.GetPage(Type type, Dictionary<string, object> filterValues, IPagingLocation location, string sortExpression)
		{
			return GetPage(type, filterValues, (PagingLocation)location, sortExpression);
		}
		
		
		public virtual IEntity[] GetPage(Type type, PagingLocation location, string sortExpression)
		{
			return GetPage(type, new Dictionary<string, object>(), location, sortExpression);
		}
		
		public virtual IEntity[] GetPage(Type type, string propertyName, object propertyValue, PagingLocation location, string sortExpression)
		{
			Dictionary<string, object> filters = new Dictionary<string, object>();
			filters.Add(propertyName, propertyValue);
			return GetPage(type, filters, location, sortExpression);
		}
		
		public virtual IEntity[] GetPage(Type type, Dictionary<string, object> filterValues, PagingLocation location, string sortExpression)
		{
			IEntity[] entities = new IEntity[] {};
			
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving a page of entities.", NLog.LogLevel.Debug))
			{
				if (location == null)
					throw new ArgumentNullException("location");
				
				object data = null;
				
				AppLogger.Debug("Type: " + type.ToString());
				
				AppLogger.Debug("Page index: " + location.PageIndex.ToString());
				AppLogger.Debug("Page size: " + location.PageSize.ToString());
				AppLogger.Debug("Sort expression: " + sortExpression);
				
				//if (!TryExecute("Index", type.Name, out data, parameters))
				//{
				//	AppLogger.Debug("Could not execute custom function. Using default instead.");
					
					// TODO: Check if parameters should be passed to default function
					data = DefaultGetPage(type, location, sortExpression);
				//}
				
				entities = Collection<IEntity>.ConvertAll(data);
				
				AppLogger.Debug("Entities #: " + entities.Length.ToString());
			}
			
			return entities;
		}
		#endregion
		
		#region Get entities functions
		
		public virtual IEntity[] DefaultGetEntities(Type type)
		{
			IEntity[] entities = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Executing default function to retrieve a collection of entities.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Type: " + type.ToString());
				
				entities = (IEntity[])DataAccess.Data.GetEntities(type);
				
				AppLogger.Debug("Entities #: " + (entities == null ? "[null]" : entities.Length.ToString()));
			}
			return entities;
		}
		
		IEntity[] IFactory.GetEntities(Type type)
		{
			return GetEntities(type);
		}
		
		IEntity[] IFactory.GetEntities(Type type, params object[] parameters)
		{
			return GetEntities(type, parameters);
		}
		
		public virtual IEntity[] GetEntities(Type type)
		{
			return GetEntities(type, new object[] {});
		}
		
		public virtual IEntity[] GetEntities(Type type, params object[] parameters)
		{
			IEntity[] entities = new IEntity[] {};
			
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving an array of entities.", NLog.LogLevel.Debug))
			{				
				object data = null;
				
				AppLogger.Debug("Type: " + type.ToString());
				
				//  TODO: Clean up
				//if (!TryExecute("Index", type.Name, out data, new object[] {}))
				//{
				//	AppLogger.Debug("Could not execute custom function. Using default instead.");
					data = DefaultGetEntities(type);
				//}
				
				entities = Collection<IEntity>.ConvertAll(data);
				
				AppLogger.Debug("Entities #: " + entities.Length.ToString());
			}
			
			return entities;
		}
		#endregion
		
		#region Update functions
		public virtual bool Update(IEntity entity)
		{
			return (bool)Execute("Update", entity.GetType().Name, entity);
			//	throw new NotImplementedException();
		}
		#endregion
		
		#region Save functions
		public virtual bool Save(IEntity entity)
		{
			return (bool)Execute("Save", entity.GetType().Name, entity);
			//throw new NotImplementedException();
		}
		#endregion
		
		#region Delete functions
		public virtual void Delete(IEntity entity)
		{
			Execute("Delete", entity.GetType().Name, entity);
			//throw new NotImplementedException();
		}
		#endregion
		
		#region Create function
		public virtual IEntity Create(Type type)
		{
			return (IEntity)Execute("Create", type.Name);
		}
		#endregion
		
		public bool TryExecute(string action, string typeName, out object returnValue, params object[] parameters)
		{
			bool success = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Attempting to execute factory command.", NLog.LogLevel.Debug))
			{
				if (parameters == null)
					throw new ArgumentNullException("parameters");
				
				success = Commands.TryExecute(action, typeName, parameters, out returnValue);
				
				if (returnValue == null)
					AppLogger.Debug("Return value: [null]");
				else
					AppLogger.Debug("Return value: " + returnValue.ToString());
				
				AppLogger.Debug("Success? " + success.ToString());
			}
			
			return success;
		}
		
		public object Execute(string action, string typeName, params object[] parameters)
		{
			object returnValue = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Executing factory command.", NLog.LogLevel.Debug))
			{
				if (parameters == null)
					throw new ArgumentNullException("parameters");
				
				returnValue = Commands.Execute(action, typeName, parameters);
				
				if (returnValue == null)
					AppLogger.Debug("Return value: [null]");
				else
					AppLogger.Debug("Return value: " + returnValue.ToString());
			}
			
			return returnValue;
		}
		
		
		/// <summary>
		/// Registers the entity in the system.
		/// </summary>
		/// <param name="config">The mapping configuration object to add the settings to.</param>
		public void RegisterFactory(params Type[] entityTypes)
		{
			MappingItem item = new MappingItem(GetType().Name);
			//item.Settings.Add("DataStoreName", "Users");
			item.Settings.Add("IsFactory", true);
			item.Settings.Add("FullName", GetType().FullName);
			//item.Settings.Add("AssemblyName", typeof(this).Assembly.FullName);
			
			foreach (Type type in entityTypes)
			{
				item.Settings.Add("Handle." + type.Name + ".Entity" + type.Name, true);
				
			}
			
			
			//MappingItem item2 = new MappingItem("IUser");
			//item2.Settings.Add("Alias", "User");
			
			Config.Mappings.AddItem(item);
			//Config.Mappings.AddItem(item2);
		}
		
		/// <summary>
		/// Deregisters the factory from the system.
		/// </summary>
		/// <param name="config">The mapping configuration object to remove the settings from.</param>
		public void DeregisterFactory()
		{
			throw new NotImplementedException();
		}
	}
}
