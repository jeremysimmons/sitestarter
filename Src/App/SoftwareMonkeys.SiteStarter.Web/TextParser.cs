using System;
using System.Collections;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Helps to parse text and insert dynamic values.
	/// </summary>
	[DataObject(true)]
	public class TextParser
	{
		static public string ParseSetting(string key)
		{
			if (!Config.Application.Settings.ContainsKey(key))
				return String.Empty;

			string settingValue = Config.Application.Settings[key].ToString();

			IUser user = null;
			if (HttpContext.Current.Request.IsAuthenticated && AuthenticationState.User != null)
				user = RetrieveStrategy.New<User>().Retrieve<User>(AuthenticationState.User.ID);

			string output = String.Empty;

			if (settingValue != null)
			{
				output = settingValue;
				if (user != null)
					output = Parse(output, user);

				output = Parse(output);
			}

			return output;
		}

		static public string Parse(string text)
		{
			if (text == null || text == String.Empty)
				return String.Empty;

			Entities.IUser systemAdministrator = RetrieveStrategy.New<User>().Retrieve<User>(Config.Application.PrimaryAdministratorID);

			string newText = text;
			newText = newText.Replace("${WebSite.Title}", Config.Application.Title);
			newText = newText.Replace("${SystemAdministrator.Name}", systemAdministrator.Name);
			return newText;
		}


		static public string Parse(string text, IUser user)
		{
			if (text == null || text == String.Empty)
				return String.Empty;

			string newText = text;
			if (user != null)
			{
				newText = newText.Replace("${User.Name}", user.Name);
				newText = newText.Replace("${User.FirstName}", user.FirstName);
				newText = newText.Replace("${User.LastName}", user.LastName);
			}

			return newText;

		}
	}
}
