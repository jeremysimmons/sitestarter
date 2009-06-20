using System;
using System.Collections;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business;
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

            VirtualServer server = VirtualServerFactory.GetVirtualServerByName(SiteStarter.State.VirtualServerState.VirtualServerName);
            IUser user = null;
            if (HttpContext.Current.Request.IsAuthenticated && My.User != null)
                user = UserFactory.Current.GetUser(My.User.ID);

            string output = String.Empty;

            if (settingValue != null)
            {
                output = settingValue;
                if (server != null)
                    output = Parse(output, server);
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

            SiteStarter.State.VirtualServerState.SuspendVirtualServerState();

            Entities.IUser systemAdministrator = UserFactory.Current.GetUser(Config.Application.PrimaryAdministratorID);

            SiteStarter.State.VirtualServerState.RestoreVirtualServerState();

            string newText = text;
            newText = newText.Replace("${WebSite.Title}", Config.Application.Title);
            newText = newText.Replace("${SystemAdministrator.Name}", systemAdministrator.Name);
            return newText;
        }

       static public string Parse(string text, VirtualServer server)
       {
           if (text == null || text == String.Empty)
               return String.Empty;

            UriBuilder builder = new UriBuilder();
            builder.Host = HttpContext.Current.Request.Url.Host;
            builder.Path = HttpContext.Current.Request.ApplicationPath;
            if (server != null)
	            builder.Query = "VS=" + server.Name;
       
       		string newText = text;
       		newText = newText.Replace("${VirtualServer.Url}", builder.Uri.ToString());// TODO: Fix

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
