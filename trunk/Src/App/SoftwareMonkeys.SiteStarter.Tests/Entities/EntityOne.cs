using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
    public class EntityOne : BaseTestEntity
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Guid[] twoIDs;
        public Guid[] TwoIDs
        {
            get { return twoIDs; }
            set { twoIDs = value; }
        }
        
        private EntityTwo singleReferenceProperty;
        [Reference]
        public EntityTwo SingleReferenceProperty
        {
        	get { return singleReferenceProperty; }
        	set { singleReferenceProperty = value; }
        }
        
        
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
				
			MappingItem item2 = new MappingItem("EntityOne");
			item2.Settings.Add("DataStoreName", "Testing_EntityOnes");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(EntityOne).FullName);
			item2.Settings.Add("AssemblyName", typeof(EntityOne).Assembly.FullName);
			
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
