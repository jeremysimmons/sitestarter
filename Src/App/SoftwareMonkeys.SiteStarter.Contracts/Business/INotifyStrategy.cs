using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all notification strategies.
	/// </summary>
	public interface INotifyStrategy : IStrategy
	{
		/// <summary>
		/// Sends the provided notification message to all notifiable users.
		/// </summary>
		/// <param name="subject">The subject of the email to send to all notifiable users.</param>
		/// <param name="message">The message of the email to send to all notifiable users.</param>
		void SendNotification(string subject, string message);
		
		/// <summary>
		/// Sends the provided notification message to all notifiable users.
		/// </summary>
		/// <param name="entity">The entity involved in the event that users are being notified about.</param>
		/// <param name="subject">The subject of the email to send to all notifiable users.</param>
		/// <param name="message">The message of the email to send to all notifiable users.</param>
		void SendNotification(IEntity entity, string subject, string message);
	}
}
