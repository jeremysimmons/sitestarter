using System;
using System.Web;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.State;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Used to load a user control.
	/// </summary>
	public class ControlLoader
	{
		private string applicationPath = String.Empty;
		public virtual string ApplicationPath
		{
			get
			{
				if (applicationPath == String.Empty)
				{
					if (HttpContext.Current == null)
						throw new Exception("HttpContext.Current is not initialized. Set the ApplicationPath property manually when running outside a web environment.");
					
					applicationPath = HttpContext.Current.Request.ApplicationPath;
				}
				return applicationPath;
			}
			set { applicationPath = value; }
		}
		
		private Page page;
		/// <summary>
		/// Gets/sets the page with the LoadControl function to use.
		/// </summary>
		public Page Page
		{
			get { return page; }
			set { page = value; }
		}
		
		public ControlLoader(Page page)
		{
			Page = page;
		}
		
		public virtual Control LoadControl(string path)
		{
			if (path.IndexOf(StateAccess.State.PhysicalApplicationPath) > -1)
				path = PhysicalToVirtual(path);
			
			if (Page == null)
				throw new InvalidOperationException("The Page property hasn't been initialized.");
			
			return Page.LoadControl(path);
		}
		
		public virtual string PhysicalToVirtual(string physicalPath)
		{
			string virtualPath = physicalPath;
			
			if (StateAccess.IsInitialized)
			{
				virtualPath = virtualPath
					.Replace(StateAccess.State.PhysicalApplicationPath, "")
					.Replace("\\", "/");
			}
			
			virtualPath = ApplicationPath.TrimEnd(Path.DirectorySeparatorChar) + "/" + virtualPath;
			
			return virtualPath;
		}
	}
}
