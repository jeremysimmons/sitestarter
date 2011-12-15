using System;
using System.Net;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Configuration;
using System.Net.Mail;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to notify users of events via email.
	/// </summary>
	public class Emailer
	{
		/// <summary>
		/// Gets/sets the SMTP server from the application configuration settings.
		/// </summary>
		static public string SmtpServer
		{
			get
			{
				// If it's configured in the settings then use it
				if (Configuration.Config.IsInitialized && Configuration.Config.Application.Settings.ContainsKey("SmtpServer") && Configuration.Config.Application.Settings["SmtpServer"] != null)
					return Configuration.Config.Application.Settings.GetString("SmtpServer");
				
				// Otherwise fall back to the config file
				else
					return ConfigurationSettings.AppSettings["SmtpServer"];
			}
		}
		
		public Emailer()
		{
		}
		
		/// <summary>
		/// Sends the provided email to the provided user.
		/// </summary>
		/// <param name="user">The user being emailed.</param>
		/// <param name="subject">The subject of the email to send.</param>
		/// <param name="message">The message of the email to send.</param>
		public virtual void SendEmail(User user, string subject, string message)
		{
			if (user == null)
				throw new ArgumentNullException("user");
			
			SendEmail(user.Name, user.Email, subject, message);
		}
		
		public virtual void SendEmail(string recipientName, string recipientEmail, string subject, string message)
		{
			string systemName = Config.Application.Title;
			// TODO: Let administrators specify reply address
			string systemEmail = "noreply@softwaremonkeys.net";
			
			SendEmail(systemName, systemEmail, recipientName, recipientEmail, subject, message);
		}
		
		public virtual void SendEmail(string senderName, string senderEmail, string recipientName, string recipientEmail, string subject, string message)
		{
			try
			{
				MailMessage mm = new MailMessage(senderEmail,
				                                 recipientEmail,
				                                 PrepareEmailText(subject, senderName, senderEmail, recipientName, recipientEmail),
				                                 PrepareEmailText(message, senderName, senderEmail, recipientName, recipientEmail)
				                                );
				
				CreateSmtpClient().Send(mm);
			}
			catch(FormatException ex)
			{
				LogWriter.Error(ex.ToString());
			}
			catch(SmtpFailedRecipientException ex)
			{
				LogWriter.Error(ex.ToString());
			}
			catch(SmtpException ex)
			{
				LogWriter.Error(ex.ToString());
			}
		}
		
		/// <summary>
		/// Must be overridden to format the email text and insert data.
		/// </summary>
		/// <param name="original">The original text.</param>
		/// <returns></returns>
		protected virtual string PrepareEmailText(string original, string senderName, string senderEmail, string recipientName, string recipientEmail)
		{
			string text = original;
			
			text = text.Replace("${Application.Title}", Config.Application.Title);
			text = text.Replace("${Sender.Name}", senderName);
			text = text.Replace("${Recipient.Name}", recipientName);
			
			return original;
		}
		
		static public SmtpClient CreateSmtpClient()
		{
			SmtpClient smtp = new SmtpClient(SmtpServer);
			
			// If the SMTP authentication is enabled then use the username and password in the settings
			if (Config.Application.Settings.GetBool("EnableSmtpAuthentication")
			    && Config.Application.Settings.GetString("SmtpUsername") != String.Empty
			    && Config.Application.Settings.GetString("SmtpPassword") != String.Empty)
			{
				smtp.Credentials = new NetworkCredential(Config.Application.Settings.GetString("SmtpUsername"),
				                                         Config.Application.Settings.GetString("SmtpPassword"));
				
			}
			return smtp;
		}
		
		public static Emailer New()
		{
			return new Emailer();
		}
	}
}
