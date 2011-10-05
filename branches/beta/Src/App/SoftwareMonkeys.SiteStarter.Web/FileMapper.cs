using System;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.State;

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
		
		private string physicalApplicationPath = String.Empty;
		/// <summary>
		/// Gets/sets the physical application path.
		/// </summary>
		public string PhysicalApplicationPath
		{
			get {
				if (physicalApplicationPath == String.Empty)
				{
					if (Configuration.Config.IsInitialized)
						physicalApplicationPath = Configuration.Config.Application.PhysicalApplicationPath;
					else
						physicalApplicationPath = HttpContext.Current.Request.PhysicalApplicationPath;
				}
				return physicalApplicationPath; }
			set {physicalApplicationPath = value; }
		}
		
		public FileMapper()
		{
			
		}
		
		public FileMapper(string applicationPath)
		{
			ApplicationPath = applicationPath;
		}
		
		/// <summary>
		/// Maps the provided relative URL to the full physical path of the resource.
		/// </summary>
		/// <param name="relativeUrl">The relative URL of the resource to map. (Relative to the root of the application.)</param>
		/// <returns>The full physical path to the specified resource.</returns>
		public virtual string MapApplicationRelativePath(string relativeUrl)
		{
			string output = String.Empty;
		
			using (LogGroup logGroup = LogGroup.StartDebug("Mapping the provided path."))
			{
				if (ApplicationPath == String.Empty)
					throw new InvalidOperationException("The ApplicationPath property has not been set.");
				
				LogWriter.Debug("Application relative path: " + relativeUrl);
				
				string serverRelativePath = ApplicationPath + "/" + relativeUrl.Trim('/');
				
				LogWriter.Debug("Server relative path: " + serverRelativePath);
				
				output = MapPath(serverRelativePath);
				
				LogWriter.Debug("Output: " + output);
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
		
			using (LogGroup logGroup = LogGroup.Start("Mapping the path: " + physicalApplicationPath, NLog.LogLevel.Debug))
			{
				
				LogWriter.Debug("Server relative path: " + relativeUrl);
				
				relativeUrl = relativeUrl.Replace(ApplicationPath, "").TrimStart('\\');
				
				LogWriter.Debug("Updated relative path: " + relativeUrl);
				
				output = PhysicalApplicationPath.TrimEnd('\\') + @"\" + relativeUrl.Replace("/", @"\").TrimStart('\\');
				
				LogWriter.Debug("Output: " + output);
			}
			
			return output;
		}
	}
}
