using System;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// The base of all standard view projections.
	/// </summary>
	public class BaseViewProjection : ControllableProjection
	{
		private ViewEntityController controller;
		/// <summary>
		/// Gets the controller responsible for this page.
		/// </summary>
		public ViewEntityController Controller
		{
			get {
				return controller; }
		}
		
		private EntityForm form;
		/// <summary>
		/// The form used to display entity details.
		/// </summary>
		public EntityForm Form
		{
			get { return form; }
			set { form = value; }
		}
		
		public BaseViewProjection()
		{
		}
		
		/// <summary>
		/// Calls the View function when !IsPostBack.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			if (!IsPostBack)
				View();
			
			base.OnLoad(e);
		}
		
		/// <summary>
		/// Initializes the view page, the controller, and the form.
		/// </summary>
		/// <param name="defaultType">The default type of the page.</param>
		/// <param name="form">The form used to display entity data.</param>
		public void Initialize(Type defaultType, EntityForm form)
		{
			controller = ViewEntityController.CreateController(this, defaultType);
			Form = form;
			Form.EntityCommand += new EntityFormEventHandler(Form_EntityCommand);
		}

		void Form_EntityCommand(object sender, EntityFormEventArgs e)
		{
			if (e.CommandName == "Edit")
			{
				Navigator.Go("Edit", "Goal", "ID", e.CommandArgument.ToString());
			}
		}
		
		/// <summary>
		/// Prepares to view the entity specified by the query string.
		/// </summary>
		/// <returns>The entity specified by the query string.</returns>
		public virtual IEntity PrepareView()
		{
			IEntity entity = Controller.PrepareView<IEntity>();
			
			return entity;
		}
		
		/// <summary>
		/// Prepares to view the entity specified by the query string.
		/// </summary>
		/// <returns>The entity specified by the query string.</returns>
		public virtual T PrepareView<T>()
			where T : IEntity
		{
			T entity = Controller.PrepareView<T>();
			
			return entity;
		}
		
		/// <summary>
		/// Prepares to view the specified entity.
		/// </summary>
		/// <param name="id">The ID of the entity to prepare.</param>
		/// <returns>The entity specified by the unique key.</returns>
		public virtual T PrepareView<T>(Guid id)
			where T : IEntity
		{
			T entity = Controller.PrepareView<T>(id);
			
			return entity;
		}
		
		/// <summary>
		/// Prepares to view the specified entity.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to prepare.</param>
		/// <returns>The entity specified by the unique key.</returns>
		public virtual T PrepareView<T>(string uniqueKey)
		{
			T entity = Controller.PrepareView<T>(uniqueKey);
			
			return entity;
		}
		
		/// <summary>
		/// Prepares to view the specified entity.
		/// </summary>
		/// <param name="id">The ID of the entity to prepare.</param>
		/// <returns>The entity specified by the unique key.</returns>
		public virtual IEntity PrepareView(Guid id)
		{
			IEntity entity = Controller.PrepareView(id);
			
			return entity;
		}
		
		/// <summary>
		/// Prepares to view the specified entity.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to prepare.</param>
		/// <returns>The entity specified by the unique key.</returns>
		public virtual IEntity PrepareView(string uniqueKey)
		{
			IEntity entity = Controller.PrepareView(uniqueKey);
			
			return entity;
		}
		
		/// <summary>
		/// Displays the entity specified by query strings.
		/// </summary>
		public virtual void View()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Starting a view action.", NLog.LogLevel.Debug))
			{
				CheckController();
				
				IEntity entity = PrepareView();
				
				View(entity);
			}
		}
		
		/// <summary>
		/// Displays the specified entity.
		/// </summary>
		/// <param name="entityID">The ID of the entity to display.</param>
		public virtual void View(Guid entityID)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Starting a view action.", NLog.LogLevel.Debug))
			{
				CheckController();
				
				IEntity entity = PrepareView(entityID);
				
				View(entity);
			}
		}
		
		/// <summary>
		/// Displayed the specified entity.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to display.</param>
		public virtual void View(string uniqueKey)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Starting a view action.", NLog.LogLevel.Debug))
			{
				CheckController();
				
				IEntity entity = PrepareView(uniqueKey);
				
				View(entity);
			}
		}
		
		/// <summary>
		/// Displays the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void View(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Starting a view action.", NLog.LogLevel.Debug))
			{
				CheckController();
				
				Controller.View(entity);
				
				Form.DataSource = entity;
				
				DataBind();
			}
		}
		
		private void CheckController()
		{
			if (controller == null)
				throw new InvalidOperationException("Controller has not be initialized. Call Initialize().");
		}
	}
}
