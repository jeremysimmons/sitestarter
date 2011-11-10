using System;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	/// <summary>
	/// 
	/// </summary>
	[Entity("MockEntity")]
	public class MockEntity : BaseTestEntity
	{
		private MockRestrictedEntity[] restrictedEntities;
		[Reference]
		public MockRestrictedEntity[] RestrictedEntities
		{
			get { return restrictedEntities; }
			set { restrictedEntities = value; }
		}
		
		private MockPublicEntity[] publicEntities;
		[Reference(CountPropertyName="TotalPublicEntities")]
		public MockPublicEntity[] PublicEntities
		{
			get { return publicEntities; }
			set { publicEntities = value; }
		}
		
		private MockSyncEntity[] syncEntities;
		[Reference(MirrorPropertyName="Entities",
		           CountPropertyName="TotalSyncEntities")]
		public MockSyncEntity[] SyncEntities
		{
			get { return syncEntities; }
			set { syncEntities = value; }
		}
		
		private int totalPublicEntities = 0;
		public int TotalPublicEntities
		{
			get { return totalPublicEntities; }
			set { totalPublicEntities = value; }
		}
		
		private int totalSyncEntities = 0;
		public int TotalSyncEntities
		{
			get { return totalSyncEntities; }
			set { totalSyncEntities = value; }
		}
		
		public MockEntity()
		{
		}
	}
}
