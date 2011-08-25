using System;
using System.Web.Routing;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	public class ProjectionRouteHandler : StandardRouteHandler
	{
		public ProjectionRouteHandler ()
		{
			Initialize();
		}
		
		public void Initialize()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Initializing projection route handler."))
			{
				VirtualPath = GetInternalUrl(HttpContext.Current.Request.Url.ToString());
			}
		}

		public override IHttpHandler GetHttpHandler (RequestContext requestContext)
		{
			IHttpHandler handler = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Creating HTTP handler"))
			{
				if (VirtualPath == null || VirtualPath == String.Empty)
					VirtualPath = GetInternalUrl(HttpContext.Current.Request.Url.ToString());
				
				if (VirtualPath != null && VirtualPath != String.Empty)
				{
					handler = new ProjectionHttpHandler(VirtualPath);
				}
			}
			
			return handler;
		}
		
		
		public string GetInternalUrl(string friendlyUrl)
		{
			ProjectionMapper mapper = new ProjectionMapper();
			return mapper.GetInternalPath(friendlyUrl);
		}
		
	}
}

