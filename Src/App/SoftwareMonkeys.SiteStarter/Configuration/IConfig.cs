using System;
using Db4objects.Db4o;
using System.Resources;
using SoftwareMonkeys.SiteStarter.Data;

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

        /// <summary>
        /// The full physical path to the root of the application.
        /// </summary>
        string PhysicalPath { get; }

	}
}
