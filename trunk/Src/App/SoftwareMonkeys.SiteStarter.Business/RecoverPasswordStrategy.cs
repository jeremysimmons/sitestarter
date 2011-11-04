using System;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("Recover", "Password")]
	public class RecoverPasswordStrategy : BaseStrategy
	{
		private RecoverPasswordStrategy()
		{
		}
		
		private RecoverPasswordStrategy(bool requireAuthorisation)
		{
			RequireAuthorisation = requireAuthorisation;
		}
		
		public bool ResetViaEmail(string emailAddress, string subject, string message, string applicationUrl)
		{
			bool foundUser = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Requesting password reset via email."))
			{
				User user = RetrieveStrategy.New<User>(RequireAuthorisation).Retrieve<User>("Email", emailAddress);
				
				user.Password = CreateTemporaryPassword();
				
				if (user == null)
					foundUser = false;
				else
				{
					SendResetEmail(user, subject, message, applicationUrl);
					
					foundUser = true;
				}
			}
			
			return foundUser;
		}
		
		private string CreateTemporaryPassword()
		{
			string tmpPassword = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Creating a temporary password."))
			{
				tmpPassword = Guid.NewGuid().ToString().Substring(0, 7);
				
				tmpPassword = Crypter.EncryptPassword(tmpPassword);
			}
			
			return tmpPassword;
		}
		
		private void SendResetEmail(User user, string subject, string message, string applicationUrl)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Sending reset email."))
			{
				string title = Configuration.Config.Application.Title;
				string resetUrl = applicationUrl.Trim('/') + "/ChangePassword.aspx?p=" + user.Password + "&u=" + user.Email;
				
				message = message.Replace("${ResetUrl}", resetUrl);
				message = message.Replace("${Title}", title);
				
				string fromEmail = "noreply@noreply.com";
				if (Config.Application.Settings.ContainsKey("SystemEmail")
				    && Config.Application.Settings.GetString("SystemEmail") != String.Empty)
					fromEmail = Config.Application.Settings.GetString("SystemEmail");
				
				LogWriter.Debug("To email: " + user.Email);
				LogWriter.Debug("From email: " + fromEmail);
				LogWriter.Debug("Subject: " + subject);
				LogWriter.Debug("Message: " + message);
				
				SendEmailStrategy.New(RequireAuthorisation).SendEmail(subject,
				                                  	message,
				                                  	"System",
				                                  	fromEmail,
				                                  	user.Name,
				                                  	user.Email);
			}
		}
		
		public bool ChangePassword(string emailAddress, string oldPassword, string newPassword)
		{
			bool updated = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Changing user password."))
			{
				User user = RetrieveStrategy.New<User>(RequireAuthorisation).Retrieve<User>("Email", emailAddress);
				
				if (user != null)
				{
					if (user.Password == oldPassword)
					{
						user.Password = Crypter.EncryptPassword(newPassword);
						
						UpdateStrategy.New(user, RequireAuthorisation).Update(user);
						
						updated = true;
					}
				}
				
				LogWriter.Debug("Updated: " + updated.ToString());
			}
			
			return updated;
		}
		
		public static RecoverPasswordStrategy New()
		{
			return new RecoverPasswordStrategy();
		}
		
		public static RecoverPasswordStrategy New(bool requireAuthorisation)
		{
			return new RecoverPasswordStrategy(requireAuthorisation);
		}
	}
}
