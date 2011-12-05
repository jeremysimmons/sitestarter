using System;
using SoftwareMonkeys.SiteStarter.Configuration;
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
				return Emailer.SmtpServer;
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
				string systemEmail = Config.Application.Settings.GetString("SystemEmail");
				
				if (systemEmail == null || systemEmail == String.Empty)
					return false;
				
				MailMessage message = new MailMessage(systemEmail,
				                                      systemEmail,
				                                      "Test Email",
				                                      "Test Worked!");

				Emailer.CreateSmtpClient().Send(message);
				
				return true;
			}
			catch (SmtpException)
			{
				return false;
			}
		}
		
		static public TestSmtpStrategy New()
		{
			return new TestSmtpStrategy();
		}
	}
}
