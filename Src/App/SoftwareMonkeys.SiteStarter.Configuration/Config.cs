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
		//static private ConfigCollection all = new ConfigCollection();
		/// <summary>
		/// Gets the config collection for the application.
		/// </summary>
		static public ConfigCollection All
		{
			get {
				if (!State.StateAccess.IsInitialized)
					throw new InvalidOperationException("The state provider has not been initialized.");
				
				if (!StateAccess.State.ContainsApplication("Config.All"))
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
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Gets/sets the application configuration object.
		/// </summary>
		static public MappingConfig Mappings
		{
			get {
				if (All != null && All.Count > 0)
				{
					for (int i = 0; i < All.Count; i++)
					{
						if (All[i] is MappingConfig)
							return (MappingConfig)All[i];
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
						if (All[i] is MappingConfig)
							All[i] = (IConfig)value;
					}
				}
				else
					All.Add((IConfig)value);
			}
		}*/
			
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
			using (LogGroup logGroup = LogGroup.Start("Initializing the application configuration settings.", NLog.LogLevel.Debug))
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
