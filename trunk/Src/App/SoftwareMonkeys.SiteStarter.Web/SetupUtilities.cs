using System;
using System.Web;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Description of SetupUtilities.
	/// </summary>
	public class SetupUtilities
	{		
		static public bool UseExistingData
		{
			get
			{
				if (HttpContext.Current.Request.QueryString["ExistingData"] != null)
				{
					return Convert.ToBoolean(HttpContext.Current.Request.QueryString["ExistingData"]);
				}
				else
					return LegacyDataExists();
			}
		}
		
		
		static public bool LegacyDataExists()
		{
			return Directory.Exists(XmlEntitySchemaEditor.GetImportsDirectory(HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath)));
		}
	}
}
