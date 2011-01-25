using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to control standard edit entity projections.
	/// </summary>
	[Controller("Edit", "IEntity")]
	public class EditController : BaseController
	{
		public override string Action
		{
			get { return "Edit"; }
		}
		
		private IRetrieveStrategy retriever;
		/// <summary>
		/// Gets/sets the strategy used to retrieve an entity.
		/// </summary>
		public IRetrieveStrategy Retriever
		{
			get {
				if (retriever == null)
				{
					if (Container.Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					retriever = StrategyState.Strategies.Creator.NewRetriever(Container.Type.Name);
				}
				return retriever; }
			set { retriever = value; }
		}
		
		private IUpdateStrategy updater;
		/// <summary>
		/// Gets/sets the strategy used to update an entity.
		/// </summary>
		public IUpdateStrategy Updater
		{
			get {
				if (updater == null)
				{
					if (Container.Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					updater = StrategyState.Strategies.Creator.NewUpdater(Container.Type.Name);
				}
				return updater; }
			set { updater = value; }
		}
		
		private IActivateStrategy activator;
		/// <summary>
		/// Gets/sets the strategy used to activate an entity.
		/// </summary>
		public IActivateStrategy Activator
		{
			get {
				if (activator == null)
				{
					if (Container.Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					activator = StrategyState.Strategies.Creator.NewActivator(Container.Type.Name);
				}
				return activator; }
			set { activator = value; }
		}
		
		
		private string entityUpdatedLanguageKey = "EntityUpdated";
		public string EntityUpdatedLanguageKey
		{
			get { return entityUpdatedLanguageKey; }
			set { entityUpdatedLanguageKey = value; }
		}
		
		private string entityPropertyTakenLanguageKey = "EntityPropertyTaken";
		public string EntityPropertyTakenLanguageKey
		{
			get { return entityPropertyTakenLanguageKey; }
			set { entityPropertyTakenLanguageKey = value; }
		}
		
		
		public EditController()
		{
			ActionOnSuccess = "View";
		}
		
		/// <summary>
		/// Begins the edit process.
		/// </summary>
		public void StartEdit()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Starting the edit process.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					string typeName = Container.Type.Name;
					
					AppLogger.Debug("Type name: " + typeName);
					
					OperationManager.StartOperation("Edit" + typeName, null);
				}
			}
		}
		
		
		/// <summary>
		/// Prepares the edit form with the entity specified by the ID or unique key in the query string.
		/// </summary>
		/// <returns>The entity specified in the query string.</returns>
		public virtual T PrepareEdit<T>()
		{
			T entity = default(T);
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing to edit an entity.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Type name: " + typeof(T).Name);
				entity = (T)PrepareEdit();
			}
			return entity;
		}
		
		/// <summary>
		/// Prepares the edit form with the entity specified by the ID or unique key in the query string.
		/// </summary>
		/// <returns>The entity specified in the query string.</returns>
		public virtual IEntity PrepareEdit()
		{
			IEntity entity = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing to edit an entity.", NLog.LogLevel.Debug))
			{
				Guid id = QueryStrings.GetID(Container.Type.Name);
				string uniqueKey = QueryStrings.GetUniqueKey(Container.Type.Name);
				
				if (id != Guid.Empty)
				{
					AppLogger.Debug("ID: " + id.ToString());
					entity = PrepareEdit(id);
				}
				else if (uniqueKey != String.Empty)
				{
					AppLogger.Debug("Unique key: " + uniqueKey);
					entity = PrepareEdit(uniqueKey);
				}
				else
					throw new InvalidOperationException("Cannot edit entity. No identifier found.");
			}
			return entity;
		}
		
		/// <summary>
		/// Prepares the edit form with the entity specified by the provided ID.
		/// </summary>
		/// <returns>The entity specified by the provided ID.</returns>
		public virtual IEntity PrepareEdit(Guid entityID)
		{
			IEntity entity = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing to edit an entity with the ID: " + entityID, NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					AppLogger.Debug("Entity ID: " + entityID);
					
					IEntity e = Load(entityID);
					
					if (e == null)
						throw new Exception("Can't load entity of type '" + Container.Type.Name + "' with ID '" + entityID.ToString() + ".");
					
					entity = PrepareEdit(e);
				}
			}
			return entity;
		}
		
		/// <summary>
		/// Prepares the edit form with the entity specified by the provided unique key.
		/// </summary>
		/// <returns>The entity specified by the provided unique key.</returns>
		public virtual IEntity PrepareEdit(string uniqueKey)
		{
			IEntity entity = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing to edit an entity with the unique key: " + uniqueKey, NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					AppLogger.Debug("Unique key: " + uniqueKey);
					
					entity = Load(uniqueKey);
					
					if (entity == null)
						throw new Exception("Can't load entity of type '" + Container.Type.Name + "' with unique key '" + uniqueKey + "'.");
					
					PrepareEdit(entity);
				}
			}
			return entity;
		}
		
		public virtual IEntity PrepareEdit(IEntity entity)
		{
			IEntity output = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing to edit the provided entity.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised(entity))
					output = entity;
			}
			return output;
		}
		
		public virtual IEntity Edit(IEntity entity)
		{
			IEntity e = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Editing the provided entity.", NLog.LogLevel.Debug))
			{
				e = ExecuteEdit(entity);
			}
			return e;
		}
		
		/// <summary>
		/// Starts the edit process for the entity provided.
		/// </summary>
		/// <returns>The entity provided.</returns>
		public virtual IEntity ExecuteEdit(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Editing the provided entity.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised(entity))
				{
					StartEdit();
					
					Container.WindowTitle = DynamicLanguage.GetEntityText("EditEntity", Container.Type.Name);
					
					Load(entity);
				}
			}
			return entity;
		}
		
		/// <summary>
		/// Assigns the provided entity to the DataSource property.
		/// </summary>
		/// <param name="entity">The entity to assign to the DataSource</param>
		/// <returns>The assigned entity.</returns>
		public IEntity Load(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the provided entity onto DataSource and activating it.", NLog.LogLevel.Debug))
			{
				DataSource = entity;
				
				Activator.Activate(DataSource);
			}
			return entity;
		}
		
		/// <summary>
		/// Loads the entity of the specified type with the provided ID and assigns it to the DataSource property.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to load.</param>
		/// <returns>The loaded and activated entity.</returns>
		public IEntity Load(string uniqueKey)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Loading entity with the unique key: " + uniqueKey))
			{
				Load(Retriever.Retrieve("UniqueKey", uniqueKey));
			}
			return DataSource;
		}
		
		/// <summary>
		/// Loads the entity of the specified type with the provided ID and assigns it to the DataSource property.
		/// </summary>
		/// <param name="id">The ID of the entity to load.</param>
		/// <returns>The loaded and activated entity.</returns>
		public IEntity Load(Guid id)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Loading entity with the ID: " + id.ToString()))
			{
				Load(Retriever.Retrieve("ID", id));
			}
			return DataSource;
		}
		
		/// <summary>
		/// Updates the provided entity back to the data store.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>A bool value indicating the success of the update. If it fails it's due to the unique key being in use already.</returns>
		public virtual bool Update(IEntity entity)
		{
			return ExecuteUpdate(entity);
		}
		
		/// <summary>
		/// Updates the provided entity back to the data store.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>A bool value indicating the success of the update. If it fails it's due to the unique key being in use already.</returns>
		public virtual bool ExecuteUpdate(IEntity entity)
		{
			bool didSucceed = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Updating the provided entity.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					DataSource = entity;
					
					if (entity == null)
						throw new ArgumentNullException("entity");
					
					// Update the entity
					if (Updater.Update(entity))
					{
						Result.Display(DynamicLanguage.GetEntityText(EntityUpdatedLanguageKey, Container.Type.Name));

						didSucceed = true;
						
						if (AutoNavigate)
							NavigateAfterUpdate();
					}
					else
					{
						CheckUniquePropertyName();
						
						// Get the "entity exists" language entry
						string error = DynamicLanguage.GetEntityText(EntityPropertyTakenLanguageKey, Container.Type.Name);
						
						// Insert the name of the unique property
						error = error.Replace("${property}", DynamicLanguage.GetText(UniquePropertyName).ToLower());
						
						// Display the error
						Result.DisplayError(error);
						
						didSucceed = false;
					}
				}
				
				AppLogger.Debug("Did succeed: " + didSucceed.ToString());
			}
			return didSucceed;
		}
		
		public override bool AuthoriseStrategies()
		{
			return Security.Authorisation.UserCan("Edit", TypeName)
				&& Security.Authorisation.UserCan("Update", TypeName);
		}
		
		public override bool AuthoriseStrategies(IEntity entity)
		{
			return Security.Authorisation.UserCan("Edit", entity)
				&& Security.Authorisation.UserCan("Update", entity);
		}
		
		/// <summary>
		/// Navigates to the appropriate page after saving the entity from the form.
		/// </summary>
		public virtual void NavigateAfterUpdate()
		{
			Navigation.Navigator.Current.NavigateAfterOperation(ActionOnSuccess, DataSource);
		}
		
		public static EditController New(IControllable container, Type type)
		{
			return New(container, type);
		}
		
		public static EditController New(IControllable container, Type type, string uniquePropertyName)
		{
			EditController controller = null;
			using (LogGroup logGroup = AppLogger.StartGroup("Instantiating a new edit controller.", NLog.LogLevel.Debug))
			{
				controller = ControllerState.Controllers.Creator.NewEditor(type.Name);
				
				controller.Container = container;
				controller.UniquePropertyName = uniquePropertyName;
				
				AppLogger.Debug("Type name: " + type.Name);
				AppLogger.Debug("Unique property name: " + uniquePropertyName);
			}
			return controller;
		}
		
		public void CheckUniquePropertyName()
		{
			if (UniquePropertyName == null || UniquePropertyName == String.Empty)
				throw new InvalidOperationException("The UniquePropertyName property has not been set.");
		}
	}
}
