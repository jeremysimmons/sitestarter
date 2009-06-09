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
        string PhysicalPath { get;set; }

		/// <summary>
		/// The universal ID of the current project.
		/// </summary>
		Guid UniversalProjectID { get; }
		
		/// <summary>
		/// The primary administrator ID.
		/// </summary>
        Guid PrimaryAdministratorID { get;set; }

        // TODO: Clean up
        int SessionTimeout { get;set; }

        string ApplicationPath { get; set; }
        
        bool EnableVirtualServer {get;set;}
        bool EnableVirtualServerRegistration {get;set;}
        bool AutoApproveVirtualServerRegistration {get;set;}
        string[] DefaultVirtualServerKeywords {get;set;}
        
       // string PathVariation {get;}


		string SmtpServer { get; }

        SerializableDictionary<string, object> Settings { get;set; }

    }
}
