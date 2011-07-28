using System;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Web.UI;
using System.Web;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.IO;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Used to check that the application is set up and, if it's not, redirects to the appopriate page.
	/// </summary>
	public class SetupChecker
	{
		private Uri currentUrl;
		/// <summary>
		/// Gets/sets the URL of the current request.
		/// </summary>
		public Uri CurrentUrl
		{
			get {
				if (currentUrl == null && HttpContext.Current != null)
					currentUrl = HttpContext.Current.Request.Url;
				return currentUrl; }
			set { currentUrl = value; }
		}
		
		private string applicationPath;
		/// <summary>
		/// Gets/sets the relative path to the application.
		/// </summary>
		public string ApplicationPath
		{
			get {
				if (applicationPath == null && HttpContext.Current != null)
					applicationPath = HttpContext.Current.Request.ApplicationPath;
				return applicationPath; }
			set { applicationPath = value; }
		}
		
		private ApplicationInstaller installer = null;
		/// <summary>
		/// Gets/sets an instance of the ApplicationInstaller.
		/// </summary>
		public ApplicationInstaller Installer
		{
			get {
				if (installer == null)
				{
					if (HttpContext.Current != null)
					{
						installer = new ApplicationInstaller(HttpContext.Current.Request.ApplicationPath, WebUtilities.GetLocationVariation(HttpContext.Current.Request.Url));
						installer.FileMapper = FileMapper;
					}
				}
				return installer;
				
			}
			set { installer = value; }
			
		}
		
		private ApplicationRestorer restorer;
		/// <summary>
		/// Gets/sets an instance of the ApplicationRestorer.
		/// </summary>
		public ApplicationRestorer Restorer
		{
			get
			{
				if (restorer == null)
					restorer = new ApplicationRestorer(FileMapper);
				return restorer;
			}
			set
			{
				restorer = value;
			}
		}
		
		
		
		private ApplicationRestorer importer;
		/// <summary>
		/// Gets/sets an instance of the ApplicationRestorer configured for importing data (similar to restoration but from a different directory and for a different purpose).
		/// </summary>
		public ApplicationRestorer Importer
		{
			get
			{
				if (importer == null)
				{
					importer = new ApplicationRestorer(FileMapper);
					// Change the legacy directory from .../Legacy/ to .../Import/
					importer.LegacyDirectoryPath = importer.FileMapper.MapApplicationRelativePath(importer.DataDirectory + "/Import");
				}
				return importer;
			}
			set
			{
				importer = value;
			}
		}
		
		
		private IFileMapper fileMapper;
		/// <summary>
		/// Gets/sets the component used to map file paths.
		/// </summary>
		public IFileMapper FileMapper
		{
			get {
				if (fileMapper == null)
					fileMapper = new FileMapper();
				return fileMapper;  }
			set {
				fileMapper = value;
				if (restorer != null)
					restorer.FileMapper = value;
				if (installer != null)
					installer.FileMapper = value;
				if (importer != null)
					importer.FileMapper = value;
			}
		}
		
		
		public SetupChecker()
		{
		}
		
		public SetupChecker(Uri currentUrl, string applicationPath)
		{
			CurrentUrl = currentUrl;
			ApplicationPath = applicationPath;
		}
		
		
		public SetupChecker(Uri currentUrl, string applicationPath, IFileMapper fileMapper)
		{
			CurrentUrl = currentUrl;
			ApplicationPath = applicationPath;
			FileMapper = fileMapper;
		}
		
		/// <summary>
		/// Checks that the application is set up and, if it's not, redirects to the setup page or the down for maintenance page.
		/// </summary>
		public void Check()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the application needs to be set up or restored."))
			{
				string redirectPath = GetRedirectPath();
				
				if (redirectPath != String.Empty)
				{
					LogWriter.Debug("Redirecting to: " + redirectPath);
					
					HttpContext.Current.Response.Redirect(redirectPath);
				}
				else
				{
					LogWriter.Debug("No redirect required.");
				}
			}
			
		}
		
		/// <summary>
		/// Retrieves the path to redirect the user to, if applicable. Otherwise returns String.Empty.
		/// </summary>
		/// <returns>The path to redirect the user to.</returns>
		public string GetRedirectPath()
		{
			string returnPath = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the path to redirect the user to."))
			{
				CheckApplicationPath();
				
				if (!SkipPage())
				{
					LogWriter.Debug("Not skipping page.");
					
					if (RequiresRestore())
					{
						// If the user is an administrator then go to the update ready page
						if (AuthenticationState.IsAuthenticated && AuthenticationState.UserIsInRole("Administrator"))
						{
							LogWriter.Debug("Requires restore. Redirecting administrator to update ready page.");
							
							returnPath = ApplicationPath + "/Admin/UpdateReady.html";
						}
						else
						{
							LogWriter.Debug("Requires restore. Redirecting standard user to maintenance page.");
							
							returnPath = ApplicationPath + "/Maintenance.html";
						}
					}
					else if (RequiresImport())
					{
						// If import is required then the user must be the administrator and should be sent to the import page.
						
						LogWriter.Debug("Requires import. Redirecting to import page.");
						
						returnPath = ApplicationPath + "/Admin/Import.aspx";
					}
					else if (RequiresInstall())
					{
						// If install is required then the user must be the administrator and should be sent to the setup page.
						
						LogWriter.Debug("Requires setup. Redirecting to setup page.");
						
						returnPath = ApplicationPath + "/Admin/Setup.aspx";
					}
				}
				else
				{
					LogWriter.Debug("Skipping page.");
				}
				
				LogWriter.Debug("Return path: " + returnPath);
			}
			
			return returnPath;
			
		}
		
		/// <summary>
		/// Checks whether the application data needs to be restored.
		/// </summary>
		/// <returns>A value indicating whether the application needs to be restored.</returns>
		public bool RequiresRestore()
		{
			CheckRestorer();
			
			return Restorer.RequiresRestore;
		}
		
		/// <summary>
		/// Checks whether the application needs to be installed/configured.
		/// </summary>
		/// <returns>A value indicating whether the application needs to be installed/configured.</returns>
		public bool RequiresInstall()
		{
			CheckInstaller();
			
			return !Installer.IsInstalled;
		}
		
		/// <summary>
		/// Checks whether the application needs to be installed/configured.
		/// </summary>
		/// <returns>A value indicating whether the application needs to be installed/configured.</returns>
		public bool RequiresImport()
		{
			CheckImporter();
			
			return Importer.RequiresRestore;
		}
		
		/// <summary>
		/// Checks whether the current page should be skipped (ie. doesn't need to be setup to load).
		/// </summary>
		/// <returns></returns>
		public bool SkipPage()
		{
			bool skip = true;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Checking whether the current page should be skipped."))
			{
				CheckCurrentUrl();
				
				Uri url = CurrentUrl;
				
				LogWriter.Debug("URL: " + url.ToString());
				
				string[] skippable = new String[] {
					"backup.aspx",
					"setup.aspx",
					"restore.aspx",
					"import.aspx",
					"quicksetup.aspx",
					"error.aspx",
					"initializecache.aspx"
				};
				
				string urlString = url.ToString();
				if (url.Query != String.Empty)
					urlString = urlString.Replace(url.Query, String.Empty);
				
				LogWriter.Debug("Adjusted URL string: " + urlString);
				
				string[] parts = urlString.ToLower().Split('/');
				
				string fileName = parts[parts.Length-1];
				
				LogWriter.Debug("File name: " + fileName.ToLower());
				
				if (IsTestPage(urlString) || Array.IndexOf(skippable, fileName.ToLower()) > -1)
					skip = true;
				else
					skip = false;
				
				LogWriter.Debug("Skip: " + skip);
			}
			
			return skip;
		}
		
		/// <summary>
		/// Checks whether the provided URL points to an administration tests page.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public bool IsTestPage(string url)
		{
			if (url.ToLower().IndexOf("/admin/tests/") > -1)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Checks that the CurrentUrl property has been set.
		/// </summary>
		public void CheckCurrentUrl()
		{
			if (CurrentUrl == null)
				throw new InvalidOperationException("The CurrentUrl property hasn't been set.");
		}
		
		/// <summary>
		/// Checks that the ApplicationPath property has been set.
		/// </summary>
		public void CheckApplicationPath()
		{
			if (ApplicationPath == null)
				throw new InvalidOperationException("The ApplicationPath property hasn't been set.");
		}
		
		/// <summary>
		/// Checks that the Installer property has been set.
		/// </summary>
		public void CheckInstaller()
		{
			if (Installer == null)
				throw new InvalidOperationException("The Installer property hasn't been set.");
		}
		
		/// <summary>
		/// Checks that the Restorer property has been set.
		/// </summary>
		public void CheckRestorer()
		{
			if (Restorer == null)
				throw new InvalidOperationException("The Restorer property hasn't been set.");
		}
		
		/// <summary>
		/// Checks that the Importer property has been set.
		/// </summary>
		public void CheckImporter()
		{
			if (Importer == null)
				throw new InvalidOperationException("The Importer property hasn't been set.");
		}
	}
}
