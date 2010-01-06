using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
	[Serializable]
    [DataStore("Testing3")]
    public class EntityThree : BaseEntity
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Guid[] referencedEntityIDs = new Guid[] {};
	[Reference]
        public Guid[] ReferencedEntityIDs
        {
            get {
		if (referencedEntities != null)
			return Collection<BaseEntity>.GetIDs(referencedEntities);
		return referencedEntityIDs; }
            set {
		referencedEntityIDs = value;
		if (referencedEntityIDs == null || (referencedEntities != null && !referencedEntityIDs.Equals(Collection<BaseEntity>.GetIDs(referencedEntities))))
			referencedEntities = null;
		}
        }

	private EntityFour[] referencedEntities;
	[Reference]
        public EntityFour[] ReferencedEntities
	{
		get { return referencedEntities; }
		set { referencedEntities = value;
		//	referencedEntityIDs = Collection<BaseEntity>.GetIDs(referencedEntities);
		}
	}
        
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
			MappingItem item2 = new MappingItem("EntityThree");
			item2.Settings.Add("DataStoreName", "Testing_EntityThrees");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(EntityThree).FullName);
			item2.Settings.Add("AssemblyName", typeof(EntityThree).Assembly.FullName);
			
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
