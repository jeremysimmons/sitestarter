using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using System.Web;
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
					return ActionOnSuccess + "-" + TypeName;
				return commandOnSuccess;
			}
			set { commandOnSuccess = value; }
		}
		
		private string action = String.Empty;
		/// <summary>
		/// Gets/sets the action being performed.
		/// </summary>
		public virtual string Action
		{
			get
			{
				if (action == String.Empty)
					action = new ControllerInfo(this.GetType()).Action;
				return action;
			}
			set
			{
				action = value;
			}
		}
		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the action that is performed once the controller is successful.
		/// </summary>
		public string TypeName
		{
			get {
				if (typeName == String.Empty)
				{
					Container.CheckType();
					
					return Container.Type.Name;
				}
				else
					return typeName; }
			set { typeName = value; }
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
		
		private IEntity dataSource;
		/// <summary>
		/// Gets/sets the data source.
		/// </summary>
		public IEntity DataSource
		{
			get { return dataSource; }
			set { dataSource = value;
			}
		}
		
		public string UniquePropertyName;
		
		public BaseController()
		{
		}
		
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
			bool output = false;
			
			using (LogGroup logGroup = LogGroup.Start("Ensuring that the current user is authorised to performed the desired action.", NLog.LogLevel.Debug))
			{
				Container.CheckAction();
				Container.CheckType();
				
				LogWriter.Debug("Require authorisation: " + Container.RequireAuthorisation.ToString());
				
				if (Container.RequireAuthorisation)
				{
					bool isAuthorised = AuthoriseStrategies();
					
					LogWriter.Debug("Is authorised: " + isAuthorised);
					
					if (!isAuthorised)
						FailAuthorisation();
				
					output = isAuthorised;
				}
				// If authorisation isn't required then the user is authorised by default
				else
					output = true;
				
				LogWriter.Debug("Output: " + output.ToString());
			}
			return output;
		}
		
		public virtual bool AuthoriseStrategies(IEntity entity)
		{
			return Security.Authorisation.UserCan(Action, entity);
		}
		
		public virtual bool AuthoriseStrategies()
		{
			return Security.Authorisation.UserCan(Action, Container.Type.Name);
		}
		
		/// <summary>
		/// Ensures that the user is authorised to perform the desired operation.
		/// </summary>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public virtual bool EnsureAuthorised()
		{
			bool output = false;
			
			using (LogGroup logGroup = LogGroup.Start("Ensuring that the current user is authorised to performed the desired action.", NLog.LogLevel.Debug))
			{
				Container.CheckAction();
				Container.CheckType();
				
				LogWriter.Debug("Require authorisation: " + Container.RequireAuthorisation.ToString());
				
				bool isAuthorised = false;
				
				if (Container.RequireAuthorisation)
				{
					isAuthorised = AuthoriseStrategies();
					
					LogWriter.Debug("Is authorised: " + isAuthorised);
					
					if (!isAuthorised)
						FailAuthorisation();
					
					output = isAuthorised;
				}
				else // Authorisation is not required, so the user is authorised by default
					output = true;
				
				
				LogWriter.Debug("Return value: " + output.ToString());
			}
			return output;
		}
		
		/// <summary>
		/// Ensures that the user is authorised to perform the desired action.
		/// </summary>
		/// <param name="entity">The entity involved in the authorisation check.</param>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public bool Authorise(IEntity entity)
		{
			bool output = false;
			
			using (LogGroup logGroup = LogGroup.Start("Checking whether the current user is authorised to perform the desired action.", NLog.LogLevel.Debug))
			{
				bool isAuthorised = Security.Authorisation.UserCan(Action, entity);
				
				LogWriter.Debug("Is authorised: " + isAuthorised.ToString());
				LogWriter.Debug("Require authorisation: " + Container.RequireAuthorisation.ToString());
				
				output = (!Container.RequireAuthorisation || isAuthorised);
				
				LogWriter.Debug("Output: " + output.ToString());
			}
			
			return output;
		}
	
		
		/// <summary>
		/// Displays a message to the user informing them that they're not authorised and redirects them to the unauthorised page.
		/// </summary>
		public virtual void FailAuthorisation()
		{
			using (LogGroup logGroup = LogGroup.Start("User failed authorisation.", NLog.LogLevel.Debug))
			{
				if (HttpContext.Current != null && HttpContext.Current.Request != null)
				{
					Authorisation.InvalidPermissionsRedirect();
				}
				else
				{
					throw new UnauthorisedException(QueryStrings.Action, Container.Type.Name);
				}
			}
		}
	}
}
