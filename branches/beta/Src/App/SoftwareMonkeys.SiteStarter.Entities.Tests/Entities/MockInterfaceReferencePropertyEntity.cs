using System;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	/// <summary>
	/// 
	/// </summary>
	public class MockInterfaceReferencePropertyEntity : BaseEntity
	{
		private IEntity referencedEntity;
		[Reference(TypePropertyName="ReferenceTypeName")]
		public IEntity ReferencedEntity
		{
			get { return referencedEntity; }
			set { referencedEntity = value;
				if (referencedEntity != null)
					ReferenceTypeName = referencedEntity.ShortTypeName;
			}
		}
		
		private string referenceTypeName;
		public string ReferenceTypeName
		{
			get { return referenceTypeName; }
			set { referenceTypeName = value; }
		}
		
		public MockInterfaceReferencePropertyEntity()
		{
		}
	}
}
