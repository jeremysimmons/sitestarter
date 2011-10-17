using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to control the create entity process.
	/// </summary>
	[Controller("Create", "IEntity")]
	public class CreateController : BaseController
	{
		/// <summary>
		/// Gets/sets the data source.
		/// </summary>
		public new IEntity DataSource
		{
			get { return ((BaseCreateEditProjection)Container).DataSource; }
			set { ((BaseCreateEditProjection)Container).DataSource = value; }
		}
		
		private string entitySavedLanguageKey = "EntitySaved";
		public string EntitySavedLanguageKey
		{
			get { return entitySavedLanguageKey; }
			set { entitySavedLanguageKey = value; }
		}
		
		private string entityPropertyTakenLanguageKey = "EntityPropertyTaken";
		public string EntityPropertyTakenLanguageKey
		{
			get { return entityPropertyTakenLanguageKey; }
			set { entityPropertyTakenLanguageKey = value; }
		}
		
		private ISaveStrategy saver;
		/// <summary>
		/// Gets/sets the strategy use to save an entity.
		/// </summary>
		public virtual ISaveStrategy Saver
		{
			get {
				if (saver == null)
				{
					if (Container.Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					saver = StrategyState.Strategies.Creator.NewSaver(Container.Type.Name);
					saver.RequireAuthorisation = Container.RequireAuthorisation;
				}
				return saver; }
			set { saver = value; }
		}
		
		public CreateController()
		{
			ActionOnSuccess = "View";
		}
		
		/// <summary>
		/// Begins the create process.
		/// </summary>
		public virtual IEntity Create()
		{
			IEntity entity = null;
			
			using (LogGroup logGroup = LogGroup.Start("Creating a new entity."))
			{
				if (EnsureAuthorised())
				{
					entity = CreateStrategy.New(Container.Type.Name).Create();
					
					ExecuteCreate(entity);
				}
			}
			
			return entity;
		}
		
		/// <summary>
		/// Begins the create process with the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Create(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Creating a new entity."))
			{
				ExecuteCreate(entity);
			}
		}
		
		/// <summary>
		/// Executes the create process with the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void ExecuteCreate(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Starting the create entity process.", NLog.LogLevel.Debug))
			{
				DataSource = entity;
				
				if (EnsureAuthorised(entity))
					OperationManager.StartOperation("Create" + Container.Type.Name, null);
			}
		}
		
		public virtual bool Save(IEntity entity)
		{
			bool success = false;
			
			using (LogGroup logGroup = LogGroup.Start("Saving the provided entity.", NLog.LogLevel.Debug))
			{
				success = ExecuteSave(entity);
			}
			return success;
		}
		
		/// <summary>
		/// Saves the provided entity.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		/// <returns>A value indicating whether the entity was saved (and therefore passed the validation test).</returns>
		public virtual bool ExecuteSave(IEntity entity)
		{
			bool saved = false;
			using (LogGroup logGroup = LogGroup.Start("Saving data from form.", NLog.LogLevel.Debug))
			{
				
				if (EnsureAuthorised(entity))
				{
					DataSource = entity;
					
					// Save the entity
					if (Saver.Save(entity))
					{
						// Display the result
						Result.Display(DynamicLanguage.GetEntityText(EntitySavedLanguageKey, Container.Type.Name));

						saved = true;
						
						if (AutoNavigate)
							NavigateAfterSave();
					}
					else
					{
						// TODO: Should be obsolete. Remove if it is.
						//CheckUniquePropertyName();
						
						// Get the "entity exists" language entry
						string error = DynamicLanguage.GetEntityText(EntityPropertyTakenLanguageKey, Container.Type.Name);
						
						// Insert the name of the unique property
						error = error.Replace("${property}", DynamicLanguage.GetText(UniquePropertyName).ToLower());
						
						// Display the error
						Result.DisplayError(error);
						
						saved = false;
					}
				}
				LogWriter.Debug("Saved: " + saved.ToString());
			}
			return saved;
		}
		
		public override bool AuthoriseStrategies()
		{
			return Security.Authorisation.UserCan("Create", TypeName)
				&& Security.Authorisation.UserCan("Save", TypeName);
		}
		
		public override bool AuthoriseStrategies(IEntity entity)
		{
			return Security.Authorisation.UserCan("Create", entity)
				&& Security.Authorisation.UserCan("Save", entity);
		}
		
		/// <summary>
		/// Navigates to the appropriate page after saving the entity from the form.
		/// </summary>
		public virtual void NavigateAfterSave()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Navigating after a save operation."))
			{
				if (DataSource == null)
					throw new InvalidOperationException("The DataSource property wasn't set.");
				
				LogWriter.Debug("CommandOnSuccess: "+ CommandOnSuccess);
				LogWriter.Debug("ActionOnSuccess: " + ActionOnSuccess);
				
				Navigation.Navigator.Current.NavigateAfterOperation(new CommandInfo(CommandOnSuccess), DataSource);
			}
		}
		
		
		/// <summary>
		/// Creates a new create controller.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static CreateController New(IControllable container, Type type)
		{
			return New(container, type, String.Empty);
		}
		
		/// <summary>
		/// Creates a new create controller.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static CreateController New(IControllable container, Type type, string uniquePropertyName)
		{
			CreateController controller = null;
			
			using (LogGroup logGroup = LogGroup.Start("Instantiating a new create controller.", NLog.LogLevel.Debug))
			{
				controller = New(container, container.Action, type, uniquePropertyName);
			}
			
			return controller;
		}
		
		/// <summary>
		/// Creates a new create controller.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static CreateController New(IControllable container, string action, Type type, string uniquePropertyName)
		{
			CreateController controller = null;
			
			using (LogGroup logGroup = LogGroup.Start("Instantiating a new create controller.", NLog.LogLevel.Debug))
			{
				
				if (type.Name == "IEntity")
					throw new ArgumentException("The provided type cannot be 'IEntity'.");
				
				controller = ControllerState.Controllers.Creator.New<CreateController>(action, type.Name);
				
				controller.Container = container;
				controller.UniquePropertyName = uniquePropertyName;
				
				LogWriter.Debug("Type name: " + type.Name);
				LogWriter.Debug("Unique property name: " + uniquePropertyName);
			}
			
			return controller;
		}
		
		// TODO: Should be obsolete. Remove if it is.
		public void CheckUniquePropertyName()
		{
			if (UniquePropertyName == null || UniquePropertyName == String.Empty)
				throw new InvalidOperationException("The UniquePropertyName property has not been set.");
		}
	}
}
