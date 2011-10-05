using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// 
	/// </summary>
	public class UrlRewriter
	{
		public static void Initialize()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Initializing the URL rewriter."))
			{
				if (StateAccess.IsInitialized && ProjectionState.IsInitialized)
				{
					string url = new ProjectionMapper().GetInternalPath(HttpContext.Current.Request.Url.ToString());
					
					if (url != null && url != String.Empty)
					{
						//HttpContext.Current.Server.Execute(url, true);
						HttpContext.Current.RewritePath(url, false); // Pass false parameter to ensure the form post back path is correct.
					}
				}
			}
		}
	}
}
