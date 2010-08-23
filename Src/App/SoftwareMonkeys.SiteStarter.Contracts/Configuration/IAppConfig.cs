using System;
using System.Collections;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
    public interface IAppConfig : IConfig
    {
        /// <summary>
        /// The title of the application.
        /// </summary>
        string Title { get;set;}

        /// <summary>
        /// The full physical path to the root of the application.
        /// </summary>
        string PhysicalApplicationPath { get;set; }
		
		/// <summary>
		/// The primary administrator ID.
		/// </summary>
        Guid PrimaryAdministratorID { get;set; }

        // TODO: Clean up
        int SessionTimeout { get;set; }

        string ApplicationPath { get; set; }
        
        // TODO: Remove if not needed
        /*
        bool EnableVirtualServer {get;set;}
        bool EnableVirtualServerRegistration {get;set;}
        bool AutoApproveVirtualServerRegistration {get;set;}
        string[] DefaultVirtualServerKeywords {get;set;}
        */
		string SmtpServer { get; }

        IConfigurationDictionary Settings { get;set; }

    }
}
