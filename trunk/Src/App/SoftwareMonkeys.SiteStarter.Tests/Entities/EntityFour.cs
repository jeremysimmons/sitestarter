using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
    public class EntityFour : BaseTestEntity
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

	private EntityThree[] referencedEntities;
	[Reference]
        public EntityThree[] ReferencedEntities
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
			
			MappingItem item = new MappingItem("EntityFour");
			item.Settings.Add("DataStoreName", "Testing_EntityFours");
			item.Settings.Add("IsEntity", true);
			item.Settings.Add("FullName", typeof(EntityFour).FullName);
			item.Settings.Add("AssemblyName", typeof(EntityFour).Assembly.FullName);
			
			Config.Mappings.AddItem(item);
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
