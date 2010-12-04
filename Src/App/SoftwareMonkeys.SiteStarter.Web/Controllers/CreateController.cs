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
		
		private string entityExistsLanguageKey = "EntityExists";
		public string EntityExistsLanguageKey
		{
			get { return entityExistsLanguageKey; }
			set { entityExistsLanguageKey = value; }
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
					if (Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					saver = StrategyState.Strategies.Creator.NewSaver(Type.Name);
					saver.RequireAuthorisation = RequireAuthorisation;
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
			if (EnsureAuthorised())
				OperationManager.StartOperation("Create" + Type.Name, null);
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
					Result.Display(DynamicLanguage.GetEntityText(EntitySavedLanguageKey, Type.Name));

					saved = true;
				}
				else
				{					
					Result.DisplayError(DynamicLanguage.GetEntityText(EntityExistsLanguageKey, Type.Name));
					
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
			if (type.Name == "IEntity")
				throw new ArgumentException("The provided type cannot be 'IEntity'.");
			
			CreateController controller = ControllerState.Controllers.Creator.New<CreateController>("Create", type.Name);
			
			controller.Container = container;
			controller.Type = type;
			
			return controller;
		}
	}
}
