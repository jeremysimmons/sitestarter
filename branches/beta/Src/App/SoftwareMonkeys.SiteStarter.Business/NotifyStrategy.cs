using System;
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
	[Strategy("Notify", "IEntity")]
	public class NotifyStrategy : BaseStrategy, INotifyStrategy
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
		
		public NotifyStrategy()
		{
		}
		
		/// <summary>
		/// Sends the provided notification message to all notifiable users.
		/// </summary>
		/// <param name="subject">The subject of the email to send to all notifiable users.</param>
		/// <param name="message">The message of the email to send to all notifiable users.</param>
		public virtual void SendNotification(string subject, string message)
		{
			SendNotification(null, subject, message);
		}
		
		
		/// <summary>
		/// Sends the provided notification message to all notifiable users.
		/// </summary>
		/// <param name="entity">The entity involved in the event that users are being notified about.</param>
		/// <param name="subject">The subject of the email to send to all notifiable users.</param>
		/// <param name="message">The message of the email to send to all notifiable users.</param>
		public virtual void SendNotification(IEntity entity, string subject, string message)
		{
			User[] recipients = IndexStrategy.New<User>(false).Index<User>("EnableNotifications", true);
			
			SendNotification(recipients, entity, subject, message);
		}
		
		/// <summary>
		/// Sends the provided notification message to the provided notifiable users.
		/// </summary>
		/// <param name="entity">The recipients of the notification.</param>
		/// <param name="entity">The entity involved in the event that users are being notified about.</param>
		/// <param name="subject">The subject of the email to send to the provided notifiable users.</param>
		/// <param name="message">The message of the email to send to the provided notifiable users.</param>
		public virtual void SendNotification(User[] recipients, IEntity entity, string subject, string message)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Sending a notification to the provided recipients."))
			{
				LogWriter.Debug("Recipients: " + recipients.Length.ToString());
				
				foreach (User user in recipients)
				{
					if (user != null && user.EnableNotifications)
					{
						LogWriter.Debug("To: " + user.Email);
						
						string replyTo = "noreply@noreply.com";
						if (Config.Application.Settings.ContainsKey("SystemEmail"))
						{
							replyTo = Config.Application.Settings.GetString("SystemEmail");
						}
						else
							LogWriter.Error("No system email has been set. Notification emails have 'noreply@noreply.com' in the reply field instead.");
				
						LogWriter.Debug("Reply to: " + replyTo);
						
						try
						{
							
							MailMessage mm = new MailMessage(replyTo,
							                                 user.Email,
							                                 PrepareNotificationText(subject, entity),
							                                 PrepareNotificationText(message, entity));
							
							Emailer.CreateSmtpClient().Send(mm);
						}
						catch(FormatException ex)
						{
							LogWriter.Error(ex);
						}
						catch(SmtpFailedRecipientException ex)
						{
							LogWriter.Error(ex);
						}
						catch(SmtpException ex)
						{
							LogWriter.Error(ex);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Must be overridded to format the notification email text and insert data.
		/// </summary>
		/// <param name="original">The original text.</param>
		/// <param name="entity">The entity containing data to insert into the text.</param>
		/// <returns></returns>
		protected virtual string PrepareNotificationText(string original, IEntity entity)
		{
			return original;
		}
		
		public static NotifyStrategy New()
		{
			return StrategyState.Strategies.Creator.New<NotifyStrategy>("Notify", "IEntity");
		}
	}
}
