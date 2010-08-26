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

        /// <summary>
        /// The session timeout period.
        /// </summary>
        int SessionTimeout { get;set; }

        /// <summary>
        /// The virtul path to the application.
        /// </summary>
        string ApplicationPath { get; set; }
        
        /// <summary>
        /// The various application settings.
        /// </summary>
        IConfigurationDictionary Settings { get;set; }

    }
}
