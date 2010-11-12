using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using System.Web;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Web.Properties;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class BaseController : IController
	{		
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
		/// The container of the controller.
		/// </summary>
		public IControllable Container;
		
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
		
		private string unauthorisedUrl;
		/// <summary>
		/// Gets/sets the URL of the page that unauthorised users are sent to.
		/// </summary>
		public string UnauthorisedUrl
		{
			get { return Configuration.Config.Application.ApplicationPath + "/Default.aspx"; }
			set { unauthorisedUrl = value; }
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
		/// Displays a message to the user informing them that they're not authorised and redirects them to the unauthorised page.
		/// </summary>
		public virtual void FailAuthorisation()
		{
			Result.DisplayError(Language.Unauthorised);
			
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
				HttpContext.Current.Response.Redirect(UnauthorisedUrl);

		}
	}
}
