using System;
using System.Resources;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Defines the configuration settings required for backend operations.
	/// </summary>
	public interface IConfig// : SoftwareMonkeys.WorkHub.ClientBackend.IConfig
	{
        /// <summary>
        /// The name of the config/module file (excluding the extension).
        /// </summary>
        string Name { get; set; }



	}
}
