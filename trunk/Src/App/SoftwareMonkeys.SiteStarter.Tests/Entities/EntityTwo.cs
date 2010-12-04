using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
    public class EntityTwo : BaseTestEntity
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Guid[] oneIDs;
        public Guid[] OneIDs
        {
            get { return oneIDs; }
            set { oneIDs = value; }
        }
        
        private EntityOne singleReferenceProperty;
        [Reference]
        public EntityOne SingleReferenceProperty
        {
        	get { return singleReferenceProperty; }
        	set { singleReferenceProperty = value; }
        }
        
    }
}
