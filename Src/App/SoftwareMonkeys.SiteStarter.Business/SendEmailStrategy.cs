using System;
using System.Net.Mail;
using System.Configuration;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to send emails.
	/// </summary>
	[Strategy("Send", "Email")]
	public class SendEmailStrategy : BaseStrategy
	{
		static public string SmtpServer
		{
			get
			{
				return Emailer.SmtpServer;
			}
		}
		
		public SendEmailStrategy()
		{
		}
		
		
  		public void SendEmail(string subject, string message, string senderName, string senderEmail, string recipientName, string recipientEmail)
        {
            MailMessage mm = new MailMessage(senderEmail, recipientEmail, subject, message);

            ExecuteSend(mm);
        }
  		
  		public void SendEmail(string subject, string message, Entities.User fromUser, string name, string email)
        {
            MailMessage mm = new MailMessage(fromUser.Email, email, subject, message);

            ExecuteSend(mm);
        }
  		
  		public void SendEmail(string subject, string message, Entities.User fromUser, Entities.User toUser)
        {
  			if (fromUser == null)
  				throw new ArgumentNullException("fromUser");
  			
  			if (toUser == null)
  				throw new ArgumentNullException("toUser");
  			
            MailMessage mm = new MailMessage(fromUser.Email, toUser.Email, subject, message);

            ExecuteSend(mm);
        }
  		
  		public void ExecuteSend(MailMessage message)
  		{
  			Emailer.CreateSmtpClient().Send(message);
  		}
  		
  		static public SendEmailStrategy New(bool requiresAuthorisation)
  		{
  			SendEmailStrategy strategy = new SendEmailStrategy();
  			strategy.RequireAuthorisation = true;
  			return strategy;
  		}
  		static public SendEmailStrategy New()
  		{
  			return new SendEmailStrategy();
  		}
	}
}
