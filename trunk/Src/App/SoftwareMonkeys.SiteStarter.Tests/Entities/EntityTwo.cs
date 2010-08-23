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
        
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
			
			MappingItem item2 = new MappingItem("EntityTwo");
			item2.Settings.Add("DataStoreName", "Testing_EntityTwos");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(EntityTwo).FullName);
			item2.Settings.Add("AssemblyName", typeof(EntityTwo).Assembly.FullName);
			
			Config.Mappings.AddItem(item2);
        }
        
        /// <summary>
        /// Deregisters the entity from the system.
        /// </summary>
        static public void DeregisterType()
        {
        	throw new NotImplementedException();
        }

    }
}
