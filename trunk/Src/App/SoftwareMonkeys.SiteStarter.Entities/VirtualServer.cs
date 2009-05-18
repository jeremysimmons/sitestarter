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