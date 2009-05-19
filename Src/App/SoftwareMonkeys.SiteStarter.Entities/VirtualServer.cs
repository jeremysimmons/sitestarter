using System;
using System.Data;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines a virtual server.
    /// </summary>
    [DataStore("VirtualServers")]
    [Serializable]
    public class VirtualServer : BaseEntity
    {
        private string name;
        /// <summary>
        /// Gets/sets the name of the role.
        /// </summary>
        public string Name
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
        public bool IsApproved
        {
            get { return isApproved; }
            set { isApproved = value; }
        }
        
                private Guid primaryAdministratorID;
        /// <summary>
        /// Gets/sets the ID of the primaryAdministrator that the feature is part of.
        /// </summary>
        [EntityIDReference(IDsPropertyName = "PrimaryAdministratorID",
           EntitiesPropertyName="PrimaryAdministrator")]
        public Guid PrimaryAdministratorID
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

        private User primaryAdministrator;
        /// <summary>
        /// Gets/sets the name of the primaryAdministrator that the feature is part of.
        /// </summary>
        [XmlIgnore]
        [EntityReference(ExcludeFromDataStore = true,
           IDsPropertyName = "PrimaryAdministratorID",
           EntitiesPropertyName="PrimaryAdministrator")]
        public User PrimaryAdministrator
        {
            get { return primaryAdministrator; }
            set
            {
                primaryAdministrator = value;
            }
        }

        private DateTime dateCreated;
        /// <summary>
        /// Gets/sets the date that the virtual server was created.
        /// </summary>
        public DateTime DateCreated
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
    }
}