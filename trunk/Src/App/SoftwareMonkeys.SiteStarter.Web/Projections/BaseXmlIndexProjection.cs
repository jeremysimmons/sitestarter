using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Web;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Security;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Used as the base of all XML projections.
	/// </summary>
	public class BaseXmlIndexProjection : BaseXmlProjection
	{		
		private IndexController controller;
		/// <summary>
		/// Gets the controller used to perform actions in relation to this page.
		/// </summary>
		public IndexController Controller
		{
			get {
				return controller; }
		}
				
		public BaseXmlIndexProjection()
		{
		}
		
		/// <summary>
		/// Initializes the page and the controller for the specified type.
		/// </summary>
		/// <param name="type"></param>
		public void Initialize(Type type)
		{
			Initialize(type, false);
		}
		
		/// <summary>
		/// Initializes the page and the controller for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="isPaged"></param>
		public void Initialize(Type type, bool isPaged)
		{
			Command = new IndexCommandInfo(type.Name);
			controller = IndexController.New(this);

		}
		
		protected override void OnLoad(EventArgs e)
		{
			Index();
			
			base.OnLoad(e);
		}
		
		/// <summary>
		/// Displays an index of the entities.
		/// </summary>
		public virtual void Index()
		{
			using (LogGroup logGroup = LogGroup.Start("Displaying an index of entities.", NLog.LogLevel.Debug))
			{
				if (controller == null)
					throw new InvalidOperationException("Controller has not be initialized. Call IndexPage.Initialize().");
				
				IEntity[] entities = Controller.PrepareIndex();
				
				LogWriter.Debug("Count: " + entities.Length.ToString());
				
				Index(entities);
			}
		}
		
		/// <summary>
		/// Displays an index of the provided entities at the specified location.
		/// </summary>
		public void Index(IEntity[] entities)
		{
			using (LogGroup logGroup = LogGroup.Start("Displaying an index of the provided entities.", NLog.LogLevel.Debug))
			{
				if (entities == null)
					throw new ArgumentNullException("entities");
				
				CheckController();
				
				DataSource = entities;
				
				Controller.Index(entities);
								
				LogWriter.Debug("Count: " + entities.Length);
				
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
