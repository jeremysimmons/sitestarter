using System;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	/// <summary>
	/// Represents an entity with a synchronous (two way) reference between it and the MockEntity.
	/// </summary>
	[Entity]
	public class MockSyncEntity : BaseEntity
	{
		private MockEntity[] entities;
		[Reference(
			MirrorPropertyName="SyncEntities",
		    CountPropertyName="TotalEntities")
		]
		public MockEntity[] Entities
		{
			get { return entities; }
			set { entities = value; }
		}
		
		private int totalEntities = 0;
		public int TotalEntities
		{
			get { return totalEntities; }
			set { totalEntities = value; }
		}
		
		public MockSyncEntity()
		{
		}
	}
}
