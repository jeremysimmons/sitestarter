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

		private EntityThree[] referencedEntities;
		[Reference]
		public EntityThree[] ReferencedEntities
		{
			get { return referencedEntities; }
			set { referencedEntities = value;
				//	referencedEntityIDs = Collection<BaseEntity>.GetIDs(referencedEntities);
			}
		}
		
	}
}
