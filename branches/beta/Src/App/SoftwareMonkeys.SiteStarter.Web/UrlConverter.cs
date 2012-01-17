using System;
using System.Collections.Generic;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// 
	/// </summary>
	public class UrlConverter
	{
		private string applicationPath = String.Empty;
		public string ApplicationPath
		{
			get {
				if (applicationPath == String.Empty)
					applicationPath = StateAccess.State.ApplicationPath;
				return applicationPath; }
			set { applicationPath = value; }
		}
		
		private string host = String.Empty;
		public string Host
		{
			get {
				if (host == String.Empty)
				{
					if (HttpContext.Current != null)
						host = HttpContext.Current.Request.Url.Host;
				}
				return host; }
			set { host = value; }
		}
		
		private string protocol = String.Empty;
		public string Protocol
		{
			get {
				if (protocol == String.Empty)
				{
					if (HttpContext.Current != null)
						protocol = HttpContext.Current.Request.Url.Scheme;
				}
				return protocol; }
			set { protocol = value; }
		}
		
		public UrlConverter()
		{
		}
		
		public UrlConverter(string applicationPath)
		{
			ApplicationPath = applicationPath;
		}
		
		public UrlConverter(Uri applicationUrl)
		{
			ApplicationPath = ToRelative(applicationUrl.AbsolutePath);
			Host = applicationUrl.Host;
			Protocol = applicationUrl.Scheme;
		}
		
		/// <summary>
		/// Converts the provided relative URL to an absolute URL.
		/// </summary>
		/// <param name="relativeUrl">The relative URL to convert.</param>
		/// <returns></returns>
		public string ToAbsolute(string relativeUrl)
		{
			if (Protocol == String.Empty)
				throw new Exception("Protocol property has not been set.");
			
			if (Host == String.Empty)
				throw new Exception("Host property has not been set.");
			
			return string.Format("{0}://{1}/{2}", Protocol, Host, ResolveUrl(relativeUrl).Trim('/'));
		}
		
		public string ToRelative(string absoluteUrl)
		{
			return ToRelative(absoluteUrl, ApplicationPath);
		}
		
		public string ToRelative(string absoluteUrl, string relatedPath)
		{
			string newPath = String.Empty;
			using (LogGroup logGroup = LogGroup.StartDebug("Converting the provided absolute URL to be relative to the one provided."))
			{
				if (absoluteUrl == null)
					throw new ArgumentNullException("absoluteUrl");
				
				if (relatedPath == null)
					throw new ArgumentNullException("relatedPath");
				
				LogWriter.Debug("Original URL: " + absoluteUrl);
				LogWriter.Debug("Related path: " + relatedPath);
				
				// If necessary, first fix the related path by converting to absolute
				if (relatedPath.ToLower().IndexOf("http://") == -1
				    && relatedPath.ToLower().IndexOf("https://") == -1)
				{
					relatedPath = ToAbsolute(relatedPath);
					
					LogWriter.Debug("Related path changed to absolute: " + relatedPath);
				}
				
				// If necessary, first fix the URL by converting to absolute
				if (absoluteUrl.ToLower().IndexOf("http://") == -1
				    && absoluteUrl.ToLower().IndexOf("https://") == -1)
				{
					absoluteUrl = ToAbsolute(absoluteUrl);
					
					LogWriter.Debug("Original url changed to absolute: " + absoluteUrl);
				}
				
				// Now convert to relative
				string[] originalParts = absoluteUrl.Trim('/').Split('/');
				string[] relatedParts = relatedPath.Trim('/').Split('/');
				
				List<string> keptItems = new List<string>();
				
				// Strip off the related path from the original path
				for (int i = 0; i < originalParts.Length; i++)
				{
					if (i >= relatedParts.Length)
						keptItems.Add(originalParts[i]);
				}
				
				newPath = "/" + String.Join("/", keptItems.ToArray());

				LogWriter.Debug("New path: " + newPath);
				
			}
			return newPath;
		}
		
		public string ResolveUrl(string originalUrl)
		{
			if (originalUrl == null)
				originalUrl = String.Empty;

			if (!string.IsNullOrEmpty(originalUrl) && '~' == originalUrl[0])
			{
				int index = originalUrl.IndexOf('?');
				string queryString = (-1 == index) ? null : originalUrl.Substring(index);
				if (-1 != index) originalUrl = originalUrl.Substring(0, index);
				originalUrl = VirtualPathUtility.ToAbsolute(originalUrl) + queryString;
			}

			return originalUrl;

		}
	}
}
