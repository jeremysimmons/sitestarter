using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	[Serializable]
	public class EntityFive : BaseTestEntity
	{
		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private EntitySix[] referencedEntities = new EntitySix[]{};
		[Reference]
		public EntitySix[] ReferencedEntities
		{
			get { return referencedEntities; }
			set { referencedEntities = value; }
		}
		
		private ISimple[] interfaceReferencedEntities = new ISimple[]{};
		[Reference(TypeName="EntitySix")]
		public ISimple[] InterfaceReferencedEntities
		{
			get { return interfaceReferencedEntities; }
			set { interfaceReferencedEntities = value; }
		}
	}
}
