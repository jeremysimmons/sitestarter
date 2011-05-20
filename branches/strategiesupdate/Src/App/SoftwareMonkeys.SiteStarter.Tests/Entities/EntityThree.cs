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

		private EntityFour[] referencedEntities;
		[Reference]
		public EntityFour[] ReferencedEntities
		{
			get { return referencedEntities; }
			set { referencedEntities = value;
			}
		}
	}
}
