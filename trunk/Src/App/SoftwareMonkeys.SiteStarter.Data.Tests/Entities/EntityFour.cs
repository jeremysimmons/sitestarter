using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests.Entities
{
    [DataStore("Testing")]
    public class EntityFour : BaseEntity
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

       private Guid[] referencedEntityIDs = new Guid[] {};
	[EntityIDReferences(MirrorName="ReferencedEntityIDs",
		EntitiesPropertyName="ReferencedEntities",
		IDsPropertyName="ReferencedEntityIDs")]
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
	[EntityReferences(ExcludeFromDataStore=true,
		MirrorName="ReferencedEntities",
		EntitiesPropertyName="ReferencedEntities",
		IDsPropertyName="ReferencedEntityIDs")]
        public EntityThree[] ReferencedEntities
	{
		get { return referencedEntities; }
		set { referencedEntities = value;
		//	referencedEntityIDs = Collection<BaseEntity>.GetIDs(referencedEntities);
		}
	}
    }
}
