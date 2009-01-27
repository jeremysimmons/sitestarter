using System;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Holds the current configuration settings used for backend operations.
	/// </summary>
  //  [XmlType(Namespace = "urn:SoftwareMonkeys.SiteStarter.Config")]
   // [XmlRoot(Namespace = "urn:SoftwareMonkeys.SiteStarter.Config")]
	public class Config
	{
        //static private ConfigCollection all = new ConfigCollection();
        /// <summary>
        /// Gets the config collection for the application.
        /// </summary>
        static public ConfigCollection All
        {
            get {
                if (State.StateAccess.State.GetApplication("Config.All") == null)
                    State.StateAccess.State.SetApplication("Config.All", new ConfigCollection());
                return (ConfigCollection)State.StateAccess.State.GetApplication("Config.All"); }
            set { State.StateAccess.State.SetApplication("Config.All", value); }
        }

        /// <summary>
        /// Gets/sets the application configuration object.
        /// </summary>
        static public IAppConfig Application
        {
            get {
                if (All != null && All.Count > 0)
                {
                    for (int i = 0; i < All.Count; i++)
                    {
                        if (All[i] is IAppConfig)
                            return (IAppConfig)All[i];
                    }
                }
                return null;
            }
            set
            {
                if (All.Contains((IConfig)value))
                {
                    for (int i = 0; i < All.Count; i++)
                    {
                        if (All[i] is IAppConfig)
                            All[i] = (IConfig)value;
                    }
                }
                else
                    All.Add((IConfig)value);
            } 
        }

        /// <summary>
        /// Gets a flag indicating whether the application configuration has been initialized.
        /// </summary>
        static public bool IsInitialized
        {
            get { return (Application != null); }
        }

        /// <summary>
        /// Initializes all configuration objects.
        /// </summary>
        /// <param name="physicalApplicationPath">The physical path to the root of the application.</param>
        static public void Initialize(string physicalApplicationPath)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Initializes the application configuration settings."))
            {
                AppLogger.Info("Looking for configs in: " + physicalApplicationPath);

                All = ConfigFactory.LoadAllConfigs(physicalApplicationPath.TrimEnd('\\') + @"\App_Data\");
            }
        }

        /// <summary>
        /// Clears and disposes all configuration objects.
        /// </summary>
        static public void Dispose()
        {
            All = null;
        }
    }
}
