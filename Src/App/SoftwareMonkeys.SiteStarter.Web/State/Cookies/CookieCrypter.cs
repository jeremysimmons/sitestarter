﻿using System;
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
			FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(value);
            
            return ticket.UserData;
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
            
            return encryptedValue;
		}
	}
}

