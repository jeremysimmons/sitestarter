using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
	public class EntityThree : BaseTestEntity
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
	}
}
