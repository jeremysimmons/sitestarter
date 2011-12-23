using System;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.State;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Holds the current configuration settings used for backend operations.
	/// </summary>
	//  [XmlType(Namespace = "urn:SoftwareMonkeys.SiteStarter.Config")]
	// [XmlRoot(Namespace = "urn:SoftwareMonkeys.SiteStarter.Config")]
	public class Config
	{
		static private ConfigCollection all;
		/// <summary>
		/// Gets the config collection for the application.
		/// </summary>
		static public ConfigCollection All
		{
			get
			{
				if (!State.StateAccess.IsInitialized)
					throw new InvalidOperationException("The state provider has not been initialized.");
				
				if (all == null)
				{
				if (!StateAccess.State.ContainsApplication("Config.All"))
					State.StateAccess.State.SetApplication("Config.All", new ConfigCollection());
					all = (ConfigCollection)State.StateAccess.State.GetApplication("Config.All");
				}
				return all;
			}
			set { State.StateAccess.State.SetApplication("Config.All", value); }
		}

		static private IAppConfig application;
		/// <summary>
		/// Gets/sets the application configuration object.
		/// </summary>
		static public IAppConfig Application
		{
			get {
				if (application == null)
				{
					ConfigCollection allConfigs = All;
					if (allConfigs != null & allConfigs.Count > 0)
					{
						for (int i = 0; i < allConfigs.Count; i++)
						{
							if (allConfigs[i] is IAppConfig)
								application = (IAppConfig)allConfigs[i];
					}
				}
			}
				return application;
			}
			set
			{
				application = value;
				
				ConfigCollection allConfigs = All;
				if (allConfigs.Contains((IConfig)value))
				{
					for (int i = 0; i < All.Count; i++)
					{
						if (allConfigs[i] is IAppConfig)
							allConfigs[i] = (IConfig)value;
					}
				}
				else
				{
					allConfigs.Add((IConfig)value);
					All = allConfigs;
			}
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
		/// <param name="variation">The path variation applied to configuration files.</param>
		static public void Initialize(string physicalApplicationPath, string variation)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Initializing the application configuration settings."))
			{
				if (!IsInitialized)
				{
					LogWriter.Debug("Looking for configs in: " + physicalApplicationPath);
					
					string fullPath = physicalApplicationPath.TrimEnd('\\') + Path.DirectorySeparatorChar + "App_Data";
					
					All.Add(ConfigFactory<AppConfig>.LoadConfig(fullPath, "Application", variation));
				}
				else
					LogWriter.Debug("Already initialized. Skipping.");
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
