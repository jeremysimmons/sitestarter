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
        
        
        /// <summary>
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
        }
        
        /*
        static protected string VirtualServerKey
        {
        	get { return "VirtualServer_" + VirtualServerState.VirtualServerID; }
        }*/

//        /// <summary>
  //      /// Gets/sets the virtual server configuration object.
  //      /// </summary>
  //      static public IVirtualServerConfig VirtualServer
  //      {
  //              get {
   //         	IVirtualServerConfig server = null;
            	
                /*if (All != null && All.Count > 0)
                {
                    for (int i = 0; i < All.Count; i++)
                    {
                        if (All[i] is IVirtualServerConfig && ((IVirtualServerConfig)All[i]).ID.ToString() == (string)StateAccess.State.GetSession("VirtualServerID"))
                        {
                            server = (IVirtualServerConfig)All[i];
                        }
                    }
                }*/
                
    //            server = (IVirtualServerConfig)StateAccess.State.GetApplication(VirtualServerKey);
                
    //            if (server == null)
     //           {
    //            	server = LoadVirtualServerConfig();
     //           	
    //            	StateAccess.State.SetApplication(VirtualServerKey, server);
    //            }
    //            return server;
    //        }
            /*set
            {
                if (All.Contains((IVirtualServerConfig)value))
                {
                    for (int i = 0; i < All.Count; i++)
                    {
                        if (All[i] is IVirtualServerConfig && ((IVirtualServerConfig)All[i]).ID.ToString() == (string)StateAccess.State.GetSession("VirtualServerID"))
                            All[i] = (IVirtualServerConfig)value;
                    }
                }
                else
                    All.Add((IVirtualServerConfig)value);
            } */
      //  }

        /// <summary>
        /// Gets a flag indicating whether the application configuration has been initialized.
        /// </summary>
        static public bool IsInitialized
        {
            get { return (Application != null && Mappings != null); }
        }
        
        /// <summary>
        /// Initializes all configuration objects.
        /// </summary>
        /// <param name="physicalApplicationPath">The physical path to the root of the application.</param>
        /// <param name="variation">The path variation applied to configuration files.</param>
        static public void Initialize(string physicalApplicationPath, string variation)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Initializes the application configuration settings.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Looking for configs in: " + physicalApplicationPath);
                
                string fullPath = physicalApplicationPath.TrimEnd('\\') + Path.DirectorySeparatorChar + "App_Data";
                
                // TODO: Remove if virtual server feature is removed
                /*
            	string virtualServerID = String.Empty;
                
                if (StateAccess.IsInitialized && StateAccess.State != null)
                	virtualServerID = (string)StateAccess.State.GetSession("VirtualServerID");
                
                if (virtualServerID != null && virtualServerID != String.Empty && virtualServerID != Guid.Empty.ToString())
                	fullPath += virtualServerID + @"\";*/
                
                All.Add(ConfigFactory<AppConfig>.LoadConfig(fullPath, "Application", variation));
                All.Add(ConfigFactory<MappingConfig>.LoadConfig(fullPath, "Mappings", variation));
            }
        }
        
       /* static protected IVirtualServerConfig LoadVirtualServerConfig()
        {
        	using (LogGroup logGroup = AppLogger.StartGroup("Loading the current virtual server config.", NLog.LogLevel.Debug))
        	{
        		string path = Config.Application.PhysicalApplicationPath + @"\App_Data\VS\" + StateAccess.State.GetSession("VirtualServerID") + @"\VirtualServer.config";
        		
        		AppLogger.Debug("Path: " + path);
        		AppLogger.Debug("Virtual server name: " + VirtualServerState.VirtualServerName);
        		AppLogger.Debug("Virtual server ID: " + VirtualServerState.VirtualServerID);
        		
	        	if (!VirtualServerState.VirtualServerSelected
	        		|| StateAccess.State.GetSession("VirtualServerID") == null
	        		|| StateAccess.State.GetSession("VirtualServerID") == String.Empty
	        		|| StateAccess.State.GetSession("VirtualServerID") == Guid.Empty.ToString())
	        	{
	        		return null;
	        	}
	        		
	        	return (IVirtualServerConfig)ConfigFactory.LoadConfig(path, typeof(VirtualServer));
        	}
        }*/

        /// <summary>
        /// Clears and disposes all configuration objects.
        /// </summary>
        static public void Dispose()
        {
            All = null;
        }
    }
}
