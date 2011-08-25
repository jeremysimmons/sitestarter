using System;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web;
using System.Web.UI;
using System.Security;
using System.Web.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web
{
	public class StandardRouteHandler : IRouteHandler
	{
		public StandardRouteHandler()
		{
			VirtualPath = String.Empty;
		}
		
		public StandardRouteHandler(string virtualPath) : this(virtualPath, true)
		{
		}

		public StandardRouteHandler(string virtualPath, bool checkPhysicalUrlAccess)
		{
			this.VirtualPath = virtualPath;
			this.CheckPhysicalUrlAccess = checkPhysicalUrlAccess;
		}

		public string VirtualPath { get; protected set; }

		public bool CheckPhysicalUrlAccess { get; set; }

		public virtual IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			IHttpHandler handler = null;
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving HTTP handler."))
			{
				LogWriter.Debug("Virtual path: " + VirtualPath);
				
				if (this.CheckPhysicalUrlAccess
				   && !UrlAuthorizationModule.CheckUrlAccessForPrincipal(this.VirtualPath
				                                                        ,  requestContext.HttpContext.User
				                                                      , requestContext.HttpContext.Request.HttpMethod))
					throw new SecurityException();

				if (VirtualPath != null || VirtualPath != String.Empty)
				{
					foreach (KeyValuePair<string, object> token in requestContext.RouteData.Values)  
				    {                  
				        requestContext.HttpContext.Items.Add(token.Key, token.Value);  
				    }
					
			
					var page = BuildManager
						.CreateInstanceFromVirtualPath("~" + VirtualPath
						                               , typeof(Page)) as IHttpHandler;
					handler = (IHttpHandler)page;
					
					
				}
				else
					LogWriter.Debug("VirtualPath not set. Skipping.");
			}
			return handler;
		}
	}
}
