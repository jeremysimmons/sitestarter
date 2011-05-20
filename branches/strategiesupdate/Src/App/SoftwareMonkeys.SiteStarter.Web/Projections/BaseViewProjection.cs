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
	public class BaseViewProjection : BaseProjection
	{
		private ViewController controller;
		/// <summary>
		/// Gets the controller responsible for this page.
		/// </summary>
		public ViewController Controller
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
		
		public override string InternalAction
		{
			get { return "Retrieve"; }
		}
		
		public BaseViewProjection()
		{
		}
		
		public BaseViewProjection(string action, Type type) : base(action, type)
		{}
		
		public BaseViewProjection(string action, Type type, bool requireAuthorisation) : base(action, type, requireAuthorisation)
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
		/// Initializes the projection.
		/// </summary>
		/// <param name="type">The type of entity being viewed.</param>
		/// <param name="form">The form used to display entity data.</param>
		public void Initialize(Type type)
		{
			Initialize(type, null);
		}
				
		/// <summary>
		/// Initializes the projection.
		/// </summary>
		/// <param name="type">The type of entity being viewed.</param>
		/// <param name="form">The form used to display entity data.</param>
		public void Initialize(Type type, EntityForm form)
		{
			Type = type;
			controller = ViewController.New(this);
			Form = form;
			if (Form != null)
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
			T entity = Controller.LoadEntity<T>(id);
			
			return entity;
		}
		
		/// <summary>
		/// Prepares to view the specified entity.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to prepare.</param>
		/// <returns>The entity specified by the unique key.</returns>
		public virtual T PrepareView<T>(string uniqueKey)
		{
			T entity = Controller.LoadEntity<T>(uniqueKey);
			
			return entity;
		}
		
		/// <summary>
		/// Prepares to view the specified entity.
		/// </summary>
		/// <param name="id">The ID of the entity to prepare.</param>
		/// <returns>The entity specified by the unique key.</returns>
		public virtual IEntity PrepareView(Guid id)
		{
			IEntity entity = Controller.LoadEntity(id);
			
			return entity;
		}
		
		/// <summary>
		/// Prepares to view the specified entity.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to prepare.</param>
		/// <returns>The entity specified by the unique key.</returns>
		public virtual IEntity PrepareView(string uniqueKey)
		{
			IEntity entity = Controller.LoadEntity(uniqueKey);
			
			return entity;
		}
		
		/// <summary>
		/// Displays the entity specified by query strings.
		/// </summary>
		public virtual void View()
		{
			using (LogGroup logGroup = LogGroup.Start("Starting a view action.", NLog.LogLevel.Debug))
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
			using (LogGroup logGroup = LogGroup.Start("Starting a view action.", NLog.LogLevel.Debug))
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
			using (LogGroup logGroup = LogGroup.Start("Starting a view action.", NLog.LogLevel.Debug))
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
			using (LogGroup logGroup = LogGroup.Start("Starting a view action.", NLog.LogLevel.Debug))
			{
				CheckController();
				
				Controller.View(entity);
				
				if (Form != null)
					Form.DataSource = entity;
					
				WindowTitle = entity.ToString();
				
				DataBind();
			}
		}
		
		private void CheckController()
		{
			if (controller == null)
				throw new InvalidOperationException("Controller has not be initialized. Call Initialize().");
		}
		
		/// <summary>
		/// Evaluates the provided expression against the data source and returns the corresponding value.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public object Eval(string expression)
		{
			return Controller.Eval(expression);
		}
	}
}
