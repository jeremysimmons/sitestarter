using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Web;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Description of ErrorHandler.
	/// </summary>
	public class ExceptionHandler
	{
		public ExceptionHandler()
		{
		}
		
		
		public void Handle(Exception exception)
		{
			try
			{
				LogWriter.Error(exception);
				
				SendEmail(exception);
				
				if (Convert.ToBoolean(ConfigurationSettings.AppSettings["EnableErrorHandling"]))
				{
					LogWriter.Debug("Error handling is enabled. Redirecting to error page.");
					
					// Send the user to the error page if they aren't already there
					if (HttpContext.Current.Request.Url.ToString().ToLower().IndexOf("error.aspx") == -1)
						HttpContext.Current.Server.Transfer(HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/Error.aspx");
				}
				else
					LogWriter.Debug("Error handling is disabled");
			}
			catch (Exception ex)
			{
				using (LogGroup logGroup = LogGroup.StartDebug("Error handling failed. There was an error within the error handling itself."))
				{
					LogWriter.Error(ex);
					throw ex;
				}
			}
		}
		
		public void SendEmail(Exception exception)
		{
			if (new ModeDetector().IsRelease)
			{
			UserRole role = RetrieveStrategy.New<UserRole>().Retrieve<UserRole>("Name", "Administrator");
			
			ActivateStrategy.New(role).Activate(role, "Users");
			
			foreach (User user in role.Users)
			{
				string subject = "Exception";
				string message = "An exception occurred...\n"
					+ "URL:\n"
					+ HttpContext.Current.Request.Url.ToString() + "\n"
					+ "Site:\n"
					+ WebUtilities.ConvertRelativeUrlToAbsoluteUrl(HttpContext.Current.Request.ApplicationPath) + "\n"
					+ "Time:\n"
					+ DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString() + "\n"
					+ "Exception:\n"
					+ exception.ToString();
				
				Emailer.New().SendEmail(user, subject, message);
			}
			}
		}
	}
}
