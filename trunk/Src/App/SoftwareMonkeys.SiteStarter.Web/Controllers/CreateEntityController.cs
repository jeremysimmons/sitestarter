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
	public class CreateEntityController : BaseController
	{
		private IEntity dataSource;
		public IEntity DataSource
		{
			get { return dataSource; }
			set { dataSource = value;
			}
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
				}
				return saver; }
			set { saver = value; }
		}
		
		/*private bool requiresAuthentication = true;
		public bool RequiresAuthentication
		{
			get { return requiresAuthentication; }
			set { requiresAuthentication = value; }
		}*/
		
		public CreateEntityController()
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
					Result.Display(DynamicLanguage.GetEntityText("EntitySaved", Type.Name));

					saved = true;
				}
				else
				{					
					Result.DisplayError(DynamicLanguage.GetEntityText("EntityExists", Type.Name));
					
					saved = false;
				}
				
				AppLogger.Debug("Saved: " + saved.ToString());
			}
			return saved;
		}
		
		/// <summary>
		/// Ensures that the user is authorised to create an entity of the specified type.
		/// </summary>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public bool EnsureAuthorised()
		{
			bool isAuthorised = AuthoriseSaveStrategy.New(Type.Name).Authorise(Type.Name);
			
			if (!isAuthorised)
				FailAuthorisation();
			
			return isAuthorised;
		}
		
		/// <summary>
		/// Creates a new create controller.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static CreateEntityController CreateController(IControllable container, Type type)
		{
			if (type.Name == "IEntity")
				throw new ArgumentException("The provided type cannot be 'IEntity'.");
			
			CreateEntityController controller = new CreateEntityController();
			
			controller.Container = container;
			controller.Type = type;
			
			return controller;
		}
	}
}
