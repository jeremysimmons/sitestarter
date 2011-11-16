using System;
using System.Web.Security;

namespace SoftwareMonkeys.SiteStarter.Web.State.Cookies
{
	public class CookieCrypter
	{
		public CookieCrypter ()
		{
		}
		
		static public string Decrypt(string value)
		{
			FormsAuthenticationTicket ticket = null;
            
			try
			{
				ticket = FormsAuthentication.Decrypt(value);
			}
			catch
			{
				// Ignore and return String.Empty
			}
            
			if (ticket != null)
            	return ticket.UserData;
			else
				return String.Empty;
		}
		
		static public string Encrypt(string name, string value, DateTime expires)
		{
			
			
			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, expires, false, value, FormsAuthentication.FormsCookiePath);
			
			string encryptedValue = String.Empty;
			
			try
			{
				encryptedValue = FormsAuthentication.Encrypt(ticket);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new ArgumentOutOfRangeException("Invalid expiration date: " + expires.ToString(), ex);
			}
			catch
			{
				// Ignore and return String.Empty
			}
            
            		return encryptedValue;
		}
	}
}

