using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Navigation;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Web.Validation;
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
		/// <summary>
		/// Gets/sets the data source.
		/// </summary>
		public new IEntity DataSource
		{
			get { return ((BaseCreateEditProjection)Container).DataSource; }
			set { ((BaseCreateEditProjection)Container).DataSource = value; }
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
					CheckContainer();
					Container.CheckCommand();
					
					retriever = StrategyState.Strategies.Creator.NewRetriever(Command.TypeName);
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
					CheckContainer();
					Container.CheckCommand();
					
					updater = StrategyState.Strategies.Creator.NewUpdater(Command.TypeName);
				}
				return updater; }
			set { updater = value; }
		}
		
		private string entityUpdatedLanguageKey = "EntityUpdated";
		public string EntityUpdatedLanguageKey
		{
			get { return entityUpdatedLanguageKey; }
			set { entityUpdatedLanguageKey = value; }
		}
		
		private string entityPropertyTakenLanguageKey = "EntityPropertyTaken";
		[ObsoleteAttribute()]
		public string EntityPropertyTakenLanguageKey
		{
			get { return entityPropertyTakenLanguageKey; }
			set { entityPropertyTakenLanguageKey = value; }
		}
		
		private ValidationFacade validation;
		public ValidationFacade Validation
		{
			get {
				if (validation == null)
					validation = new ValidationFacade();
				return validation; }
			set { validation = value; }
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
			using (LogGroup logGroup = LogGroup.Start("Starting the edit process.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					string typeName = Command.TypeName;
					
					LogWriter.Debug("Type name: " + typeName);
					
					OperationManager.StartOperation(Command, null);
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
			using (LogGroup logGroup = LogGroup.Start("Preparing to edit an entity.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Type name: " + typeof(T).Name);
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
			using (LogGroup logGroup = LogGroup.Start("Preparing to edit an entity.", NLog.LogLevel.Debug))
			{
				Guid id = GetID();
				string uniqueKey = GetUniqueKey();
				
				if (id != Guid.Empty)
				{
					LogWriter.Debug("ID: " + id.ToString());
					entity = PrepareEdit(id);
				}
				else if (uniqueKey != String.Empty)
				{
					LogWriter.Debug("Unique key: " + uniqueKey);
					entity = PrepareEdit(uniqueKey);
				}
				else
					throw new InvalidOperationException("Cannot edit entity. No identifier found.");
			}
			return entity;
		}
		
		/// <summary>
		/// Retrieves the ID of the entity being edited.
		/// </summary>
		/// <returns></returns>
		public virtual Guid GetID()
		{
			return QueryStrings.GetID(Command.TypeName);
		}
		
		/// <summary>
		/// Retrieves the unique key of the entity being edited.
		/// </summary>
		/// <returns></returns>
		public virtual string GetUniqueKey()
		{
			return QueryStrings.GetUniqueKey(Command.TypeName);
		}
		
		/// <summary>
		/// Prepares the edit form with the entity specified by the provided ID.
		/// </summary>
		/// <returns>The entity specified by the provided ID.</returns>
		public virtual IEntity PrepareEdit(Guid entityID)
		{
			IEntity entity = null;
			using (LogGroup logGroup = LogGroup.Start("Preparing to edit an entity with the ID: " + entityID, NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					LogWriter.Debug("Entity ID: " + entityID);
					
					IEntity e = Load(entityID);
					
					if (e == null)
						throw new Exception("Can't load entity of type '" + Command.TypeName + "' with ID '" + entityID.ToString() + ".");
					
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
			using (LogGroup logGroup = LogGroup.Start("Preparing to edit an entity with the unique key: " + uniqueKey, NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					LogWriter.Debug("Unique key: " + uniqueKey);
					
					entity = Load(uniqueKey);
					
					if (entity == null)
						throw new Exception("Can't load entity of type '" + Command.TypeName + "' with unique key '" + uniqueKey + "'.");
					
					PrepareEdit(entity);
				}
			}
			return entity;
		}
		
		public virtual IEntity PrepareEdit(IEntity entity)
		{
			IEntity output = null;
			using (LogGroup logGroup = LogGroup.Start("Preparing to edit the provided entity.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised(entity))
					output = entity;
			}
			return output;
		}
		
		public virtual IEntity Edit(IEntity entity)
		{
			IEntity e = null;
			using (LogGroup logGroup = LogGroup.Start("Editing the provided entity.", NLog.LogLevel.Debug))
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
			using (LogGroup logGroup = LogGroup.Start("Editing the provided entity.", NLog.LogLevel.Debug))
			{
				Validation.CheckMessages(entity);
				
				if (EnsureAuthorised(entity))
				{
					StartEdit();
					
					Container.WindowTitle = DynamicLanguage.GetEntityText("EditEntity", Command.TypeName);
					
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
			using (LogGroup logGroup = LogGroup.Start("Loading the provided entity onto DataSource and activating it.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					Navigator.Current.Go(ActionOnSuccess, Command.TypeName);
				else
				{
					entity.Activate();
					
					DataSource = entity;
				}
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
			using (LogGroup logGroup = LogGroup.StartDebug("Loading entity with the unique key: " + uniqueKey))
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
			using (LogGroup logGroup = LogGroup.StartDebug("Loading entity with the ID: " + id.ToString()))
			{
				Load(Retriever.Retrieve("ID", id));
			}
			return DataSource;
		}
		
		/// <summary>
		/// Updates the provided entity back to the data store.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>A bool value indicating the success of the update. If it fails it's due to the entity being invalid.</returns>
		public virtual bool Update(IEntity entity)
		{
			ExecutePreUpdate(entity);
			
			bool success = ExecuteUpdate(entity);
			
			if (success)
				ExecutePostUpdate(entity);
			
			if (AutoNavigate && success)
				NavigateAfterUpdate();
			
			return success;
		}
		
		/// <summary>
		/// Updates the provided entity back to the data store.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>A bool value indicating the success of the update. If it fails it's due to the entity being invalid.</returns>
		public virtual bool ExecuteUpdate(IEntity entity)
		{
			bool didSucceed = false;
			
			using (LogGroup logGroup = LogGroup.Start("Updating the provided entity.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					DataSource = entity;
					
					if (entity == null)
						throw new ArgumentNullException("entity");
					
					// Update the entity
					if (Updater.Update(entity))
					{
						Result.Display(DynamicLanguage.GetEntityText(EntityUpdatedLanguageKey, Command.TypeName));

						didSucceed = true;
						
					}
					else
					{
						// Add the validation error to the result control
						Validation.DisplayError(entity);
						
						didSucceed = false;
					}
				}
				
				LogWriter.Debug("Did succeed: " + didSucceed.ToString());
			}
			return didSucceed;
		}
		
		public virtual void ExecutePreUpdate(IEntity entity)
		{
			// no implementation here
			// can be overridden to implement pre-update functionality
		}
		
		public virtual void ExecutePostUpdate(IEntity entity)
		{
			
			// no implementation here
			// can be overridden to implement post-update functionality
		}
		
		/// <summary>
		/// Navigates to the appropriate page after saving the entity from the form.
		/// </summary>
		public virtual void NavigateAfterUpdate()
		{
			if (DataSource == null)
				throw new InvalidOperationException("The DataSource property wasn't set.");
			
			Navigation.Navigator.Current.NavigateAfterOperation(new CommandInfo(CommandOnSuccess), DataSource);
		}
		
		public static EditController New(IControllable container)
		{
			return New(container);
		}
		
		public static EditController New(IControllable container, string uniquePropertyName)
		{
			EditController controller = null;
			using (LogGroup logGroup = LogGroup.Start("Instantiating a new edit controller.", NLog.LogLevel.Debug))
			{
				container.CheckCommand();
				
				controller = ControllerState.Controllers.Creator.New<EditController>(container);
				
				controller.Container = container;
				
				LogWriter.Debug("Type name: " + container.Command.TypeName);
				LogWriter.Debug("Unique property name: " + uniquePropertyName);
			}
			return controller;
		}
	}
}
