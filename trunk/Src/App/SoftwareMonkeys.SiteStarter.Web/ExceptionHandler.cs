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
			if (exception != null)
			{
				try
				{
					string message = GetMessage(exception);

					LogWriter.Error(message);
					
					SendEmail(exception);
					
					if (Convert.ToBoolean(ConfigurationSettings.AppSettings["EnableErrorHandling"]))
					{
						LogWriter.Debug("Error handling is enabled. Redirecting to error page.");
						
						string errorPage = DecideErrorPage(exception);
						
						// Send the user to the error page if they aren't already there
						if (!IsErrorPage(HttpContext.Current.Request.Url.ToString()))
						{
							HttpContext.Current.Response.Clear();
							HttpContext.Current.Server.Transfer(HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/" + errorPage);
						}
					}
					else
						LogWriter.Debug("Error handling is disabled");
				}
				catch (Exception ex)
				{
					using (LogGroup logGroup = LogGroup.StartDebug("Error handling failed. There was an error within the error handling itself."))
					{
						string message = GetMessage(ex);
						
						LogWriter.Error(message);
						
						throw;
					}
				}
			}
		}
		
		public string GetMessage(Exception exception)
		{
			string message = exception.ToString();
			
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
			{
				if (HttpContext.Current.Request.UrlReferrer != null)
				{
					message = message + Environment.NewLine + Environment.NewLine
						+ "Referrer: "
						+ HttpContext.Current.Request.UrlReferrer.ToString();
				}

				message = message + Environment.NewLine + Environment.NewLine
					+ "User Agent: "
					+ HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
			}
			
			return message;
		}
		
		public bool IsErrorPage(string url)
		{
			return url.ToLower().IndexOf("/error.aspx") > -1
				|| url.ToLower().IndexOf("/error404.aspx") > -1;
		}
		
		public string DecideErrorPage(Exception exception)
		{
			string errorPage = "Error.aspx";
			
			if (exception is HttpException)
			{
				if (exception.Message.IndexOf("The file") > -1
				    && exception.Message.IndexOf("does not exist.") > -1)
					errorPage = "Error404.aspx";
			}
			
			return errorPage;
		}
		
		public void SendEmail(Exception exception)
		{
			try
			{
				if (new ModeDetector().IsRelease)
				{
					UserRole role = RetrieveStrategy.New<UserRole>(false).Retrieve<UserRole>("Name", "Administrator");
					
					ActivateStrategy.New(role, false).Activate(role, "Users");
					
					foreach (User user in role.Users)
					{
						string subject = "Exception";
						string message = "An exception occurred...\n";
						
						if (HttpContext.Current.Request != null && HttpContext.Current.Request.Url != null)
						{
							message = "URL:\n"
								+ HttpContext.Current.Request.Url.ToString() + "\n\n";
						}
						
						if (HttpContext.Current.Request.UrlReferrer != null)
						{
							message = message + "Referrer:\n"
								+ HttpContext.Current.Request.UrlReferrer.ToString() + "\n\n";
						}
						
						if (HttpContext.Current.Request != null)
						{
							message = message + Environment.NewLine
								+ "User Agent:"
								+ HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"] + Environment.NewLine;
						}
						
						message = message + "Site:\n"
							+ new UrlConverter().ToAbsolute(HttpContext.Current.Request.ApplicationPath) + "\n\n"
							+ "Time:\n"
							+ DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString() + "\n\n"
							+ "Exception:\n"
							+ exception.ToString() + "\n\n"
							+ "(please do not reply to this email)\n";
						
						Emailer.New().SendEmail(user, subject, message);
					}
				}
			}
			catch(Exception ex)
			{
				LogWriter.Error("Exception occurred when trying to send error report email.");
				
				LogWriter.Error(ex);
			}
		}
	}
}
