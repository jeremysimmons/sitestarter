using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Collections;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using System.Reflection;
using System.Net.Mail;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Provides an interface for interacting with entities.
	/// </summary>
    [DataObject(true)]
	public class EmailFactory
    {
        /// <summary>
		/// Sends a test email to the primary administrator to ensure that it works.
		/// </summary>
		/// <param name="smtpServer">The SMTP server to send a test email to.</param>
		/// <returns>A boolean value indicating whether the test succeeded.</returns>
		static public bool TestSmtpServer(string smtpServer)
		{
			// TODO: The email test can bypass the EnableEmailNotification setting
		//	if (Config.EnableEmailNotification)
		//	{
				try
				{
					MailMessage message = new MailMessage("test@softwaremonkeys.com",
					    "test@softwaremonkeys.com",
                        "Test Email",
                        "Test Worked!");

                    new SmtpClient(smtpServer).Send(message);
	
					return true;
				}
				catch (Exception ex)
				{
					// TODO: Should a failed test email be logged?
					//ErrorEngine.ReportLocalError(new Error(ex));
					return false;
				}
		//	}
		//	else
				return true;
		}
	}
}
