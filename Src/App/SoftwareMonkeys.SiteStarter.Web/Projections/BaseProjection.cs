using System;
using System.Web.UI;
using System.Web;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business.Security;
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
		
		private string action = String.Empty;
		public string Action
		{
			get
			{
				if (action == String.Empty && HttpContext.Current != null)
					action = QueryStrings.Action;
				return action;
			}
			set { action = value; }
		}
		
		private string internalAction = String.Empty;
		/// <summary>
		/// Gets/sets the action name used internally. Example: The action "View" has the corresponding internal action of "Retrieve".
		/// </summary>
		public virtual string InternalAction
		{
			get {
				if (internalAction == String.Empty)
					internalAction = Action;
				return internalAction; }
			set { internalAction = value; }
		}
		
		private Type type;
		/// <summary>
		/// The type of entity involved in the operation being controlled.
		/// </summary>
		public Type Type
		{
			get { return type; }
			set { type = value; }
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
		
		
		public BaseProjection()
		{
		}

		public BaseProjection(string action, Type type)
		{
			using (LogGroup logGroup = LogGroup.Start("Constructing projection object.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + Action);
				LogWriter.Debug("Type: " + Type);
				
				Action = action;
				Type = type;
				
			}
		}
		
		public BaseProjection(string action, Type type, bool requireAuthorisation)
		{
			using (LogGroup logGroup = LogGroup.Start("Constructing projection object.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Action: " + Action);
				LogWriter.Debug("Type: " + Type);
				LogWriter.Debug("RequireAuthorisation: " + RequireAuthorisation.ToString());
				
				Action = action;
				Type = type;
				RequireAuthorisation = requireAuthorisation;
			}
		}
		
		public virtual void InitializeMenu()
		{
			// override to set menu properties such as MenuTitle, MenuCategory, and ShowOnMenu
		}
		
		protected override void OnInit(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing the projection.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Url: " + Request.Url.ToString());
				
				base.OnInit(e);
			}
		}
		
		protected override void OnLoad(EventArgs e)
		{
			using (LogGroup logGroup = LogGroup.Start("Loading the projection.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Url: " + Request.Url.ToString());
				
				base.OnLoad(e);
			}
		}
		
		public override void DataBind()
		{
			using (LogGroup logGroup = LogGroup.Start("Data binding the projection.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Url: " + Request.Url.ToString());
				
				base.DataBind();
			}
		}
		
		public void CheckType()
		{
			if (Type == null)
				throw new InvalidOperationException("The Type property has not been set.");
		}
		
		public void CheckAction()
		{
			if (Action == String.Empty)
				throw new InvalidOperationException("The Action property has not been set.");
		}
	}
}
