using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using System.Web;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Web.Security;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class BaseController : IController
	{
		private bool autoNavigate = true;
		public bool AutoNavigate
		{
			get { return autoNavigate; }
			set { autoNavigate = value; }
		}
		
		private string unauthorisedUrl = String.Empty;
		/// <summary>
		/// Gets/sets the URL of the page that unauthorised users are sent to.
		/// </summary>
		public string UnauthorisedUrl
		{
			get { return UrlCreator.Current.CreateUrl("SignIn", "User"); }
			set { unauthorisedUrl = value; }
		}
		
		private string actionOnSuccess = String.Empty;
		/// <summary>
		/// Gets/sets the action that is performed once the controller is successful.
		/// </summary>
		public string ActionOnSuccess
		{
			get { return actionOnSuccess; }
			set { actionOnSuccess = value; }
		}
		
		private string commandOnSuccess = String.Empty;
		/// <summary>
		/// Gets/sets the command (ie. "[action] [entity]") that is performed once the controller is successful.
		/// </summary>
		public string CommandOnSuccess
		{
			get
			{
				if (commandOnSuccess == String.Empty)
					return ActionOnSuccess + "-" + Command.TypeName;
				return commandOnSuccess;
			}
			set { commandOnSuccess = value; }
		}
		
		public ICommandInfo Command
		{
			get {
				CheckContainer();
				return Container.Command; }
		}
		
		/// <summary>
		/// Gets/sets the title displayed in the window.
		/// </summary>
		public string WindowTitle
		{
			get
			{
				if (HttpContext.Current == null)
					return String.Empty;
				if (HttpContext.Current.Items["WindowTitle"] == null)
					HttpContext.Current.Items["WindowTitle"] = "SiteStarter";
				return (string)HttpContext.Current.Items["WindowTitle"];
			}
			set { HttpContext.Current.Items["WindowTitle"] = value; }
		}
		
		private IControllable container = null;
		/// <summary>
		/// The container of the controller.
		/// </summary>
		public IControllable Container
		{
			get { return container; }
			set { container = value; }
		}
		
		/// <summary>
		/// Gets/sets the data source.
		/// </summary>
		public object DataSource
		{
			get { return ((BaseProjection)Container).DataSource; }
			set { ((BaseProjection)Container).DataSource = value; }
		}
		
		private ControllerAuthorisation authorisation;
		/// <summary>
		/// Gets/sets the controller authorisation component.
		/// </summary>
		public ControllerAuthorisation Authorisation
		{
			get {
				if (authorisation == null)
				{
					CheckContainer();
					authorisation = new ControllerAuthorisation(Container.RequireAuthorisation);
				}
				return authorisation; }
			set { authorisation = value; }
		}
		
		public string UniquePropertyName;
		
		public BaseController()
		{
		}
		
		#region Control cycle
		public virtual void Initialize()
		{
			
		}
		
		public virtual void Load()
		{
			
		}
		
		public virtual void DataBind()
		{
			
		}
		#endregion
		
		public object Eval(string propertyName)
		{
			if (DataSource == null)
				throw new InvalidOperationException("No data source found. Make sure it has been loaded.");
			
			PropertyInfo property = DataSource.GetType().GetProperty(propertyName);
			
			if (property == null)
				throw new ArgumentException("Cannot find property '" + propertyName + "' on type '" + DataSource.GetType().ToString() + "'.");
			
			return property.GetValue(DataSource, null);
		}
		
		/// <summary>
		/// Ensures that the user is authorised to perform the desired operation.
		/// </summary>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public virtual bool EnsureAuthorised(IEntity entity)
		{
			CheckContainer();
			
			return Authorisation.EnsureAuthorised(Command.AllActions, entity);
		}
		
		/// <summary>
		/// Ensures that the user is authorised to perform the desired operation.
		/// </summary>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public virtual bool EnsureAuthorised()
		{
			CheckContainer();
			
			return Authorisation.EnsureAuthorised(Command.AllActions, Command.TypeName);
		}
		
		/// <summary>
		/// Displays a message to the user informing them that they're not authorised and redirects them to the unauthorised page.
		/// </summary>
		public virtual void FailAuthorisation()
		{
			Authorisation.FailAuthorisation(Command.Action, Command.TypeName);
		}
		
		public void CheckContainer()
		{
			if (Container == null)
				throw new Exception("Container property is not set.");
		}
	}
}
