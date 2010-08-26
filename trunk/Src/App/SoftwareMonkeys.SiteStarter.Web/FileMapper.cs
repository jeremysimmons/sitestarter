using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.IO;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Maps relative file resource paths to full physical paths.
	/// </summary>
	public class FileMapper : IFileMapper
	{		
		private string applicationPath = String.Empty;
		/// <summary>
		/// Gets/sets the server relative application path.
		/// </summary>
		public string ApplicationPath
		{
			get {
				if (applicationPath == String.Empty)
				{
					if (Configuration.Config.IsInitialized)
						applicationPath = Configuration.Config.Application.ApplicationPath;
					else
						applicationPath = HttpContext.Current.Request.ApplicationPath;
				}
				return applicationPath; }
			set {applicationPath = value; }
		}
		
		/// <summary>
		/// Maps the provided relative URL to the full physical path of the resource.
		/// </summary>
		/// <param name="relativeUrl">The relative URL of the resource to map. (Relative to the root of the application.)</param>
		/// <returns>The full physical path to the specified resource.</returns>
		public virtual string MapApplicationRelativePath(string relativeUrl)
		{
			string output = String.Empty;
		
			using (LogGroup logGroup = AppLogger.StartGroup("Mapping the provided path.", NLog.LogLevel.Debug))
			{
				if (ApplicationPath == String.Empty)
					throw new InvalidOperationException("The ApplicationPath property has not been set.");
				
				AppLogger.Debug("Application relative path: " + relativeUrl);
				
				string serverRelativePath = ApplicationPath + "/" + relativeUrl.Trim('/');
				
				AppLogger.Debug("Server relative path: " + serverRelativePath);
				
				output = MapPath(serverRelativePath);
				
				AppLogger.Debug("Output: " + output);
			}
			
			return output;
		}
		
		/// <summary>
		/// Maps the provided relative URL to the full physical path of the resource.
		/// </summary>
		/// <param name="relativeUrl">The relative URL of the resource to map. (Relative to the root of the server.)</param>
		/// <returns>The full physical path to the specified resource.</returns>
		public virtual string MapPath(string relativeUrl)
		{
			string output = String.Empty;
		
			using (LogGroup logGroup = AppLogger.StartGroup("Mapping the provided server root relative path.", NLog.LogLevel.Debug))
			{
				
				AppLogger.Debug("Server relative path: " + relativeUrl);
				
				output = HttpContext.Current.Server.MapPath(relativeUrl);
				
				AppLogger.Debug("Output: " + output);
			}
			
			return output;
		}
	}
}
