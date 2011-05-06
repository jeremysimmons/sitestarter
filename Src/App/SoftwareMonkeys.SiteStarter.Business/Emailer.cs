using System;
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
		/// Gets/sets the SMTP server from the Web.config file.
		/// </summary>
		static public string SmtpServer
		{
			get
			{
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
			
			// TODO: Let administrators specify reply address
			string fromEmail = "noreply@softwaremonkeys.net";
			
			try
			{
				MailMessage mm = new MailMessage(fromEmail,
				                                 user.Email,
				                                 PrepareEmailText(subject),
				                                 PrepareEmailText(message));
				
				new SmtpClient(SmtpServer).Send(mm);
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
		protected virtual string PrepareEmailText(string original)
		{
			return original;
		}
		
		public static Emailer New()
		{
			return new Emailer();
		}
	}
}
