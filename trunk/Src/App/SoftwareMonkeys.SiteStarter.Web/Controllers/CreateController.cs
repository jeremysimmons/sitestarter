using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to control the create entity process.
	/// </summary>
	[Controller("Create", "IEntity")]
	public class CreateController : BaseController
	{
		public override string Action
		{
			get { return "Create"; }
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
		public ISaveStrategy Saver
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
			
		}
		
		/// <summary>
		/// Begins the create process.
		/// </summary>
		public void Create()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Creating a new entity."))
			{
				Start();
			}
		}
		
		
		/// <summary>
		/// Begins the create process with the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		public void Create(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Creating a new entity."))
			{
				DataSource = entity;
				
				Start();
			}
		}
		
		/// <summary>
		/// Starts the create process.
		/// </summary>
		public void Start()
		{
			if (Container.EnsureAuthorised())
				OperationManager.StartOperation("Create" + Container.Type.Name, null);
		}
		
		/// <summary>
		/// Saves the provided entity.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		/// <returns>A value indicating whether the entity was saved (and therefore passed the validation test).</returns>
		public bool Save(IEntity entity)
		{
			bool saved = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Saving data from form.", NLog.LogLevel.Debug))
			{				
				// Save the entity
				if (Saver.Save(entity))
				{					
					// Display the result
					Result.Display(DynamicLanguage.GetEntityText(EntitySavedLanguageKey, Container.Type.Name));

					saved = true;
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
					
					saved = false;
				}
				
				AppLogger.Debug("Saved: " + saved.ToString());
			}
			return saved;
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
			// TODO: Remove type parameter if not needed
			
			if (type.Name == "IEntity")
				throw new ArgumentException("The provided type cannot be 'IEntity'.");
			
			CreateController controller = ControllerState.Controllers.Creator.New<CreateController>("Create", type.Name);
			
			controller.Container = container;
			controller.UniquePropertyName = uniquePropertyName;
			
			return controller;
		}
		
		public void CheckUniquePropertyName()
		{
			if (UniquePropertyName == null || UniquePropertyName == String.Empty)
				throw new InvalidOperationException("The UniquePropertyName property has not been set.");
		}
	}
}
