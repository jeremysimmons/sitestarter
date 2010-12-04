using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to control the viewing of entities.
	/// </summary>
	[Controller("View", "IEntity")]
	public class ViewController : BaseController
	{
		public override string Action
		{
			get { return "View"; }
		}
		
		private IRetrieveStrategy retriever;
		/// <summary>
		/// Gets/sets the strategy used to retrieve entities.
		/// </summary>
		public IRetrieveStrategy Retriever
		{
			get {
				if (retriever == null)
				{
					if (Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					retriever = StrategyState.Strategies.Creator.NewRetriever(Type.Name);
				}
				return retriever; }
			set { retriever = value; }
		}
		
		
		private IActivateStrategy activator;
		/// <summary>
		/// Gets/sets the strategy used to activate entities.
		/// </summary>
		public IActivateStrategy Activator
		{
			get {
				if (activator == null)
				{
					if (Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					activator = StrategyState.Strategies.Creator.NewActivator(Type.Name);
				}
				return activator; }
			set { activator = value; }
		}
		
		public ViewController()
		{
		}
		
		#region Main functions
		/// <summary>
		/// Displays the page for viewing an entity.
		/// </summary>
		public void View()
		{
			Reflector.InvokeGenericMethod(this,
			                              "View",
			                              new Type[] {Type},
			                              new object[]{});
		}
		
		/// <summary>
		/// Displays the page for viewing an entity.
		/// </summary>
		public void View<T>()
			where T : IEntity
		{
			Guid id = QueryStrings.GetID(Type.Name);
			string uniqueKey = QueryStrings.GetUniqueKey(Type.Name);
			
			if (id != Guid.Empty)
				View<T>(id);
			else if (uniqueKey != String.Empty)
				View<T>(uniqueKey);
			else
				throw new InvalidOperationException("Cannot view entity. No identifier found.");
		}
		
		/// <summary>
		/// Displays the page for viewing the entity with the provided ID.
		/// </summary>
		public void View(Guid entityID)
		{
			Reflector.InvokeGenericMethod(this,
			                              "View",
			                              new Type[] {Type},
			                              new object[]{entityID});
		}
		
		/// <summary>
		/// Displays the page for viewing the entity with the provided ID.
		/// </summary>
		public T View<T>(Guid entityID)
			where T : IEntity
		{
			IEntity entity = PrepareView(entityID);
			
			View(entity);
			
			return (T)entity;
		}
		
		/// <summary>
		/// Displays the page for viewing an entity with the provided unique key.
		/// </summary>
		public IEntity View(string uniqueKey)
		{
			return (IEntity)Reflector.InvokeGenericMethod(this,
			                                              "View",
			                                              new Type[] {Type},
			                                              new object[]{uniqueKey});
		}
		
		/// <summary>
		/// Displays the page for viewing the entity with the provided unique key.
		/// </summary>
		public T View<T>(string uniqueKey)
			where T : IEntity
		{
			T entity = PrepareView<T>(uniqueKey);
			
			View(entity);
			
			return (T)entity;
		}
		
		/// <summary>
		/// Displays the page for viewing the provided entity.
		/// </summary>
		public void View(IEntity entity)
		{
			Reflector.InvokeGenericMethod(this,
			                              "View",
			                              new Type[] {typeof(IEntity)},
			                              new object[]{entity});
		}
		
		/// <summary>
		/// Displays the page for viewing an entity.
		/// </summary>
		public void View<T>(T entity)
			where T : IEntity
		{
			StartView();
			
			DataSource = entity;
		}
		#endregion
		
		#region Utility functions
		
		public void StartView()
		{
			OperationManager.StartOperation("View" + Type.Name, null);
		}
		
		/// <summary>
		/// Loads the entity specified by the query string and prepares it for viewing.
		/// </summary>
		/// <returns>The entity specified by the query string.</returns>
		public IEntity PrepareView()
		{
			return (IEntity)Reflector.InvokeGenericMethod(this, "PrepareView",
			                                     new Type[] { Type },
			                                     new object[] {});
		}
		
		/// <summary>
		/// Loads the entity specified by the query string and prepares it for viewing.
		/// </summary>
		/// <returns>The entity specified by the query string.</returns>
		public T PrepareView<T>()
			where T : IEntity
		{
			
			Guid goalID = QueryStrings.GetID(Type.Name);
			string goalKey = QueryStrings.GetUniqueKey(Type.Name);
			
			if (goalID != Guid.Empty)
				return (T)PrepareView<T>(goalID);
			else if (goalKey != String.Empty)
				return (T)PrepareView<T>(goalKey);
			else
				throw new InvalidOperationException("Cannot find entity ID or unique key in query string.");
		}
		
		/// <summary>
		/// Loads the specified entity and prepares it for viewing.
		/// </summary>
		/// <param name="id">The ID of the entity to load and prepare.</param>
		/// <returns>The entity with the specified ID.</returns>
		public T PrepareView<T>(Guid id)
		{
			return (T)PrepareView(id);
		}
		
		/// <summary>
		/// Loads the specified entity and prepares it for viewing.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to load and prepare.</param>
		/// <returns>The entity with the specified unique key.</returns>
		public T PrepareView<T>(string uniqueKey)
		{
			return (T)PrepareView(uniqueKey);
		}
		
		/// <summary>
		/// Loads the specified entity and prepares it for viewing.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to load and prepare.</param>
		/// <returns>The entity with the specified unique key.</returns>
		public IEntity PrepareView(string uniqueKey)
		{
			IEntity entity = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Loading entity from unique key.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Unique key: " + uniqueKey);
				
				DataSource = entity = Retriever.Retrieve(uniqueKey);
				
				if (entity == null)
					throw new ArgumentException("Entity of type '" + Type.Name + "' not found with unique key '" + uniqueKey + "'. Check that the entity and the factory retrieve method are properly configured.");
				
				PrepareView(entity);
			}
			return entity;
		}
		
		/// <summary>
		/// Loads the specified entity and prepares it for viewing.
		/// </summary>
		/// <param name="id">The ID of the entity to load and prepare.</param>
		/// <returns>The entity with the specified ID.</returns>
		public IEntity PrepareView(Guid id)
		{
			IEntity entity = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Loading entity from unique key.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("ID: " + id.ToString());
				
				entity = Retriever.Retrieve(id);
				
				if (entity == null)
					throw new ArgumentException("Entity of type '" + Type.Name + "' not found with ID '" + id.ToString() + "'. Check that the entity and the factory retrieve method are properly configured.");
				
				AppLogger.Debug("Entity: " + entity.ToString());
				
				PrepareView(entity);
			}
			return entity;
		}
		
		/// <summary>
		/// Prepares the provided entity for viewing.
		/// </summary>
		/// <param name="entity">The entity to prepare for viewing.</param>
		public void PrepareView(IEntity entity)
		{
			Activator.Activate(entity);
			
			DataSource = entity;
		}
		#endregion
		
		public static ViewController New(IControllable container, Type type)
		{
			ViewController controller = ControllerState.Controllers.Creator.NewViewer(type.Name);
			
			controller.Container = container;
			controller.Type = type;
			
			return controller;
		}
		
		public void CheckDataSource()
		{
			if (DataSource == null)
				throw new InvalidOperationException("The DataSource property has not been set.");
		}
	}
}
