using System;
using System.Data;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines a virtual server.
    /// </summary>
    [XmlRoot(Namespace="urn:SoftwareMonkeys.SiteStarter.Entities")]
    [XmlType(Namespace="urn:SoftwareMonkeys.SiteStarter.Entities")]
    [Serializable]
    public class VirtualServer : BaseEntity, IVirtualServer, IVirtualServerConfig
    {
        private string name;
        /// <summary>
        /// Gets/sets the name of the server.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private bool isApproved;
        /// <summary>
        /// Gets/sets a flag indicating whether the virtual server has been approved.
        /// </summary>
        public virtual bool IsApproved
        {
            get { return isApproved; }
            set { isApproved = value; }
        }
        
        private Guid primaryAdministratorID;
        /// <summary>
        /// Gets/sets the ID of the primaryAdministrator that the feature is part of.
        /// </summary>
        public virtual Guid PrimaryAdministratorID
        {
            get {
                if (primaryAdministrator != null)
                    return primaryAdministrator.ID;
                return primaryAdministratorID; }
            set
            {
                primaryAdministratorID = value;
                if (primaryAdministrator != null && primaryAdministrator.ID != primaryAdministratorID)
                    primaryAdministrator = null;
            }
        }

        private IUser primaryAdministrator;
        /// <summary>
        /// Gets/sets the name of the primaryAdministrator that the feature is part of.
        /// </summary>
        [XmlIgnore]
        [Reference]
        public virtual IUser PrimaryAdministrator
        {
            get { return primaryAdministrator; }
            set
            {
                primaryAdministrator = value;
            }
        }
        
        private string[] keywords;
        /// <summary>
        /// Gets/sets the keywords applying to the virtual server.
        /// </summary>
        public virtual string[] Keywords
        {
        	get { return keywords; }
        	set { keywords = value; }
        }
        
        private string[] enabledModules = new string[] {};
        /// <summary>
        /// Gets/sets the names of the enabled modules.
        /// </summary>
        public virtual string[] EnabledModules
        {
        	get { return enabledModules; }
        	set { enabledModules = value; }
        }

        private DateTime dateCreated;
        /// <summary>
        /// Gets/sets the date that the virtual server was created.
        /// </summary>
        public virtual DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }/*

        private int maximumProjects;
        /// <summary>
        /// Gets/sets the maximum number of projects allows on the virtual server.
        /// </summary>
        public int MaximumProjects
        {
            get { return maximumProjects; }
            set { maximumProjects = value; }
        }

        private int maximumUsers;
        /// <summary>
        /// Gets/sets the maximum number of users allows on the virtual server.
        /// </summary>
        public int MaximumUsers
        {
            get { return maximumUsers; }
            set { maximumUsers = value; }
        }*/
        
    	private string pathVariation;
        /// <summary>
        /// Gets/sets the variation applied to the config file path (eg. staging, local, etc.).
        /// </summary>
        public string PathVariation
        {
        	get { return pathVariation; }
        	set { pathVariation = value; }
        }
        
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
        	MappingItem item = new MappingItem("IVirtualServer");
			item.Settings.Add("Alias", "UserRole");
			
			MappingItem item2 = new MappingItem("VirtualServer");
			item2.Settings.Add("DataStoreName", "VirtualServers");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(VirtualServer).FullName);
			item2.Settings.Add("AssemblyName", typeof(VirtualServer).Assembly.FullName);
			
			Config.Mappings.AddItem(item);
			Config.Mappings.AddItem(item2);
        }
        
        /// <summary>
        /// Deregisters the entity from the system.
        /// </summary>
        /// <param name="config">The mapping configuration object to remove the settings from.</param>
        static public void DeregisterType()
        {
        	throw new NotImplementedException();
        }

    }
}