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
	[Controller("Mock", "IEntity")]
	public class MockController : BaseController
	{		
		public MockController()
		{
		}
		
		/// <summary>
		/// Begins the create process.
		/// </summary>
		public void Mock()
		{
			using (LogGroup logGroup = LogGroup.Start("Creating a new entity."))
			{
				Start();
			}
		}
		
		
		/// <summary>
		/// Begins the create process with the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		public void Mock(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Creating a new entity."))
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
				OperationManager.StartOperation("Mock" + Command.TypeName, null);
		}
		
		/// <summary>
		/// Saves the provided entity.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		/// <returns>A value indicating whether the entity was saved (and therefore passed the validation test).</returns>
		public bool Save(IEntity entity)
		{
			bool saved = true;
			return saved;
		}
		
		/// <summary>
		/// Ensures that the user is authorised to create an entity of the specified type.
		/// </summary>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public override bool EnsureAuthorised()
		{
			bool isAuthorised = AuthoriseSaveStrategy.New(Command.TypeName).IsAuthorised(Command.TypeName);
			
			if (!isAuthorised)
				FailAuthorisation();
			
			return isAuthorised;
		}
		
		/// <summary>
		/// Mocks a new create controller.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static MockController New(IControllable container, Type type)
		{
			if (type.Name == "IEntity")
				throw new ArgumentException("The provided type cannot be 'IEntity'.");
			
			MockController controller = ControllerState.Controllers.Creator.New<MockController>(container);
			
			controller.Container = container;
			
			return controller;
		}
	}
}
