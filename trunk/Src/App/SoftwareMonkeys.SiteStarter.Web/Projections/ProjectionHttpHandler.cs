using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	public class ProjectionHttpHandler : IHttpHandler
	{
		public bool IsReusable {
			get { return false; }
		}

		private string virtualPath = String.Empty;

		public ProjectionHttpHandler (string virtualPath)
		{
			this.virtualPath = virtualPath;
			
		}

		public void ProcessRequest (HttpContext context)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Processing request via projection HTTP handler."))
			{
				LogWriter.Debug("Internal URL: " + virtualPath);
				
				// If an internal URL was generated then redirect (if not then skip it)
				if (virtualPath != String.Empty)
				{
					context.Server.Execute(virtualPath, false);

					// Other ways to redirect
					//  TODO: Remove code if not needed
					//context.Server.Transfer(virtualPath, true);
					//context.RewritePath(virtualPath, true);
				}
			}
		}
	}
}

