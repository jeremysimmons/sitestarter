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

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class BaseElement : WebControl, IElement
	{		
		private string elementName = String.Empty;
		public string ElementName
		{
			get { return elementName; }
			set { elementName = value; }
		}
		
		private string action = String.Empty;
		/// <summary>
		/// Gets/sets the action being performed.
		/// </summary>
		public virtual string Action
		{
			get
			{
				return action;
			}
			set
			{
				action = value;
			}
		}
		
		private string typeName = String.Empty;
		/// <summary>
		/// Gets/sets the action that is performed once the element is successful.
		/// </summary>
		public string TypeName
		{
			get {
					return typeName; }
			set { typeName = value; }
		}
		
		
		public BaseElement()
		{
			
		}
	}
}
