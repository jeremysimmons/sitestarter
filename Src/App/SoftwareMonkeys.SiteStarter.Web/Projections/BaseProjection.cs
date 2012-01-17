using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// Defines the base of all web projections that can be used in the application.
	/// </summary>
	public class BaseProjection : UserControl, IProjection, IControllable
	{
		public List<IController> Controllers = new List<IController>();
		
		private ICommandInfo command;
		public ICommandInfo Command
		{
			get { return command; }
			set { command = value; }
			}
		
		/// <summary>
		/// Gets/sets the title displayed in the window.
		/// </summary>
		public string WindowTitle
		{
			get
			{
				if (Context == null)
					return String.Empty;
				if (Context.Items["WindowTitle"] == null)
					Context.Items["WindowTitle"] = "SiteStarter";
				return (string)Context.Items["WindowTitle"];
			}
			set { Context.Items["WindowTitle"] = value; }
		}
		
		private bool requireAuthorisation = true;
		/// <summary>
		/// Gets/sets a value indicating whether authorisation is required.
		/// </summary>
		public bool RequireAuthorisation
		{
			get
			{
				return requireAuthorisation;
			}
			set { requireAuthorisation = value; }
		}
		
		
		private string menuTitle = String.Empty;
		/// <summary>
		/// Gets/sets the title displayed in the menu.
		/// </summary>
		public string MenuTitle
		{
			get
			{
				return menuTitle;
			}
			set { menuTitle = value; }
		}
		
		private string menuCategory = String.Empty;
		/// <summary>
		/// Gets/sets the categy displayed in the menu.
		/// </summary>
		public string MenuCategory
		{
			get
			{
				return menuCategory;
			}
			set { menuCategory = value; }
		}
		
		private bool showOnMenu = false;
		/// <summary>
		/// Gets/sets a value indicating whether the projection is to be displayed in the menu.
		/// </summary>
		public bool ShowOnMenu
		{
			get
			{
				return showOnMenu;
			}
			set { showOnMenu = value; }
		}
		
		private Navigation.Navigator navigator;
		/// <summary>
		/// Gets/sets the component responsible for navigating between pages/projections.
		/// </summary>
		public Navigation.Navigator Navigator
		{
			get {
				if (navigator == null)
					navigator = new Navigation.Navigator(this);
				return navigator;
			}
			set { navigator = value; }
		}
		
		private string unauthorisedUrl;
		/// <summary>
		/// Gets/sets the URL of the page that unauthorised users are sent to.
		/// </summary>
		public string UnauthorisedUrl
		{
			get { return new Navigation.Navigator(this).GetLink("SignIn", "User"); }
			set { unauthorisedUrl = value; }
		}
		
		/// <summary>
		/// Gets/sets the data source.
		/// </summary>
		public object DataSource
		{
			get { return ViewState["DataSource"]; }
			set { ViewState["DataSource"] = value; }
		}
		
		private string actionAlias = String.Empty;
		/// <summary>
		/// Gets/sets the alternative term for the action. (Example: 'Create' may be an alternative to 'Report' or 'Post'.)
		/// </summary>
		public string ActionAlias
		{
			get
			{
				return actionAlias;
			}
			set { actionAlias = value; }
		}
		
		private ProjectionInfo[] info = new ProjectionInfo[]{};
		/// <summary>
		/// Gets/sets the info for this projection (if manually set within the projection).
		/// </summary>
		public ProjectionInfo[] Info
		{
			get { return info; }
			set { info = value; }
		}
		
		public BaseProjection()
		{
		}

		public BaseProjection(string action, Type type)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Constructing projection object."))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type: " + type.Name);
				
				Command = new CommandInfo(action, type.Name);
				
			}
		}
		
		public BaseProjection(string action, Type type, bool requireAuthorisation)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Constructing projection object."))
			{
				LogWriter.Debug("Action: " + action);
				LogWriter.Debug("Type: " + type.Name);
				LogWriter.Debug("RequireAuthorisation: " + RequireAuthorisation.ToString());
				
				Command = new CommandInfo(action, type.Name);
				
				RequireAuthorisation = requireAuthorisation;
			}
		}
		
		public virtual void InitializeInfo()
		{
			// override to set menu properties such as MenuTitle, MenuCategory, and ShowOnMenu
		}
		
		/// <summary>
		/// Adds the provided info object to the array held by the Info property.
		/// </summary>
		/// <param name="info"></param>
		public void AddInfo(ProjectionInfo info)
		{
			List<ProjectionInfo> list = new List<ProjectionInfo>();
			if (Info != null)
				list.AddRange(Info);
			
			list.Add(info);
			
			Info = list.ToArray();
		}
		
		protected override void OnInit(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Initializing the projection."))
			{
				LogWriter.Debug("Url: " + Request.Url.ToString());
				
				base.OnInit(e);
				
				InitializeControllers();
			}
		}
		
		public virtual void InitializeControllers()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Triggering Initialize on controllers."))
			{
				LogWriter.Debug("Number of controllers: " + Controllers.Count.ToString());
				
				foreach (IController controller in Controllers)
				{
					controller.Initialize();
				}
			}
		}
		
		
		protected override void OnLoad(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Loading the projection."))
			{
				LogWriter.Debug("Url: " + Request.Url.ToString());
				
				base.OnLoad(e);
				
				LoadControllers();
			}
		}
		
		public virtual void LoadControllers()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Triggering Load on controllers."))
			{
				LogWriter.Debug("Number of controllers: " + Controllers.Count.ToString());
				
				foreach (IController controller in Controllers)
				{
					controller.Load();
				}
			}
		}
		
		public override void DataBind()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Data binding the projection."))
			{
				LogWriter.Debug("Url: " + Request.Url.ToString());
				
				base.DataBind();
				
				DataBindControllers();
			}
		}
		
		public virtual void DataBindControllers()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Triggering DataBind on controllers."))
			{
				LogWriter.Debug("Number of controllers: " + Controllers.Count.ToString());
				
				foreach (IController controller in Controllers)
				{
					controller.DataBind();
				}
			}
		}
		
		public void CheckCommand()
		{
			if (Command == null)
				throw new InvalidOperationException("The Command property must be set.");
			
			if (Command.Action == null || Command.Action == String.Empty)
				throw new InvalidOperationException("The Command.Action property must be set.");
			
			if (Command.TypeName == null || Command.TypeName == String.Empty)
				throw new InvalidOperationException("The Command.TypeName property must be set.");
		}
		
		[Obsolete("Use CheckCommand() instead.")]
		public void CheckType()
		{
			CheckCommand();
		}
		
		[Obsolete("Use CheckCommandInfo() instead.")]
		public void CheckActions()
		{
			CheckCommand();
		}
	}
}
