using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Web.Validation;
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
			get { return (IEntity)((BaseProjection)Container).DataSource; }
			set { ((BaseProjection)Container).DataSource = value; }
		}
		
		private string entitySavedLanguageKey = "EntitySaved";
		/// <summary>
		/// Gets/sets the language key for the "[entity] saved successfully" message.
		/// </summary>
		public string EntitySavedLanguageKey
		{
			get { return entitySavedLanguageKey; }
			set { entitySavedLanguageKey = value; }
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
					CheckContainer();
					Container.CheckCommand();
					
					saver = StrategyState.Strategies.Creator.NewSaver(Command.TypeName);
					saver.RequireAuthorisation = Container.RequireAuthorisation;
				}
				return saver; }
			set { saver = value; }
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
		
		public CreateController()
		{
			ActionOnSuccess = "View";
		}
		
		public CreateController(IControllable container) : base()
		{
			Container = container;
		}
		
		/// <summary>
		/// Begins the create process.
		/// </summary>
		public virtual IEntity Create()
		{
			IEntity entity = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Creating a new entity."))
			{
				if (EnsureAuthorised())
				{
					entity = CreateStrategy.New(Command.TypeName).Create();
					
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
			using (LogGroup logGroup = LogGroup.StartDebug("Creating a new entity."))
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
			using (LogGroup logGroup = LogGroup.StartDebug("Starting the create entity process."))
			{
				Validation.CheckMessages(entity);
				
				DataSource = entity;
				
				if (EnsureAuthorised(entity))
					OperationManager.StartOperation(Command, null);
			}
		}
		
		public virtual bool Save(IEntity entity)
		{
			bool success = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Saving the provided entity."))
			{
				ExecutePreSave(entity);
				
				success = ExecuteSave(entity);
				
				if (success)
					ExecutePostSave(entity);
				
				if (AutoNavigate && success)
					NavigateAfterSave();
			}
			return success;
		}
		
		public virtual void ExecutePreSave(IEntity entity)
		{
			// no implementation here
			// can be overridden to implement pre-save functionality
		}
		
		public virtual void ExecutePostSave(IEntity entity)
		{
			
			// no implementation here
			// can be overridden to implement post-save functionality
		}
		
		/// <summary>
		/// Saves the provided entity.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		/// <returns>A value indicating whether the entity was saved. If it fails it's due to the entity being invalid.</returns>
		public virtual bool ExecuteSave(IEntity entity)
		{
			bool saved = false;
			using (LogGroup logGroup = LogGroup.StartDebug("Saving data from form."))
			{
				
				if (EnsureAuthorised(entity))
				{
					DataSource = entity;
					
					// Save the entity
					if (Saver.Save(entity))
					{
						// Display the result
						Result.Display(DynamicLanguage.GetEntityText(EntitySavedLanguageKey, Command.TypeName));

						saved = true;
					}
					else
					{
						// Add the validation error to the result control
						Validation.DisplayError(entity);
						
						saved = false;
					}
				}
				LogWriter.Debug("Saved: " + saved.ToString());
			}
			return saved;
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
		/// <returns></returns>
		public static CreateController New(IControllable container)
		{
			CreateController controller = null;
			
			using (LogGroup logGroup = LogGroup.Start("Instantiating a new create controller.", NLog.LogLevel.Debug))
			{
				controller = ControllerState.Controllers.Creator.New<CreateController>(container);
				
				LogWriter.Debug("Type name: " + container.Command.TypeName);
			}
			
			return controller;
		}
		
		/// <summary>
		/// Creates a new create controller.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="uniquePropertyName"></param>
		/// <returns></returns>
		[ObsoleteAttribute("The uniquePropertyName parameter is no longer used.")]
		public static CreateController New(IControllable container, string uniquePropertyName)
		{
			return New(container);
		}
	}
}
