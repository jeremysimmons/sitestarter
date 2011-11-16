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
			foreach (User user in IndexStrategy.New<User>(false).Index<User>("EnableNotifications", true))
			{
				if (user != null)
				{
					string replyTo = "noreply@noreply.com";
					if (Config.Application.Settings.ContainsKey("SystemEmail"))
					{
						replyTo = Config.Application.Settings.GetString("SystemEmail");
					}
					else
						LogWriter.Error("No primary administrator has been assigned. Notification emails have 'noreply@noreply.com' in the reply field instead of the admistrator's email address.");
					
					try
					{
					
						MailMessage mm = new MailMessage(replyTo,
						                                                                 user.Email,
						                                                                 PrepareNotificationText(subject, entity),
						                                                                 PrepareNotificationText(message, entity));
						
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
				else
					throw new InvalidOperationException("No primary administrator configured on Config.Application.PrimaryAdministratorID.");
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
