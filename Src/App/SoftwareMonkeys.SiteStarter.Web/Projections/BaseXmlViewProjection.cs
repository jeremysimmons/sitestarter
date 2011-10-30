using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// 
	/// </summary>
	public class BaseXmlViewProjection : BaseXmlProjection
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
		
		public BaseXmlViewProjection()
		{
		}
		/// <summary>
		/// Initializes the projection.
		/// </summary>
		/// <param name="type">The type of entity being viewed.</param>
		public void Initialize(Type type)
		{
			Command = new ViewCommandInfo(type.Name);
			
			controller = ViewController.New(this);
		}
		
		protected override void OnLoad(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Loading the projection.", NLog.LogLevel.Debug))
			{
				View();
				
				base.OnLoad(e);
			}
		}

		/// <summary>
		/// Displays an entity.
		/// </summary>
		public virtual void View()
		{
			using (LogGroup logGroup = LogGroup.Start("Viewing the entity specified by the query strings.", NLog.LogLevel.Debug))
			{
				IEntity entity = Controller.PrepareView();
				
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
				
				DataSource = entity;
			}
		}
		
		private void CheckController()
		{
			if (controller == null)
				throw new InvalidOperationException("Controller has not be initialized. Call Initialize().");
		}
	}
}
