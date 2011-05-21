using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Configuration;
using System.Net.Mail;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("Test", "SMTP")]
	public class TestSmtpStrategy : BaseStrategy
	{
		static public string SmtpServer
		{
			get
			{
				return SendEmailStrategy.SmtpServer;
			}
		}
		
		public TestSmtpStrategy()
		{
		}
		
		/// <summary>
		/// Sends a test email to the primary administrator to ensure that it works.
		/// </summary>
		/// <param name="smtpServer">The SMTP server to send a test email to.</param>
		/// <returns>A boolean value indicating whether the test succeeded.</returns>
		public bool RunTest()
		{
			return RunTest(SmtpServer);
		}
		
		/// <summary>
		/// Sends a test email to the primary administrator to ensure that it works.
		/// </summary>
		/// <param name="smtpServer">The SMTP server to send a test email to.</param>
		/// <returns>A boolean value indicating whether the test succeeded.</returns>
		public bool RunTest(string smtpServer)
		{
				try
				{
					User administrator = RetrieveStrategy.New<User>().Retrieve<User>("ID", Configuration.Config.Application.PrimaryAdministratorID);
					
					if (administrator == null)
						throw new Exception("The specified primary administrator could not be found.");
					
					if (administrator.Email == null || administrator.Email == String.Empty)
						throw new Exception("The primary administrator doesn't have an email address specified.");
					
					MailMessage message = new MailMessage(administrator.Email,
					    administrator.Email,
                        "Test Email",
                        "Test Worked!");

                    new SmtpClient(smtpServer).Send(message);
	
					return true;
				}
				catch (SmtpException ex)
				{
					//throw ex;
					return false;
				}
		}
		
		static public TestSmtpStrategy New()
		{
			return new TestSmtpStrategy();
		}
	}
}
