using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of EntityIDReferenceCollection.
	/// </summary>
	[Serializable]
	public class EntityIDReferenceCollection : List<EntityIDReference>
	{
		
		private EntityIDReference[] removedReferences;
		/// <summary>
		/// Gets/sets the removed references.
		/// </summary>
		public EntityIDReference[] RemovedReferences
		{
			get { return removedReferences; }
			set { removedReferences = value; }
		}
		
		private string referenceTypeName;
		/// <summary>
		/// Gets/sets the short name of the referenced entity type.
		/// </summary>
		public string ReferenceTypeName
		{
			get { return referenceTypeName; }
		}
		
		public EntityIDReferenceCollection()
		{
		}
		
		public new void Add(EntityIDReference reference)
		{
			if (reference == null)
				throw new ArgumentNullException("reference");
			
			referenceTypeName = reference.TypeName2;
			
			base.Add(reference);
		}
		
		public EntityIDReferenceCollection SwitchFor(IEntity entity)
		{
			for (int i = 0; i < Count; i ++)
			{
				this[i] = this[i].SwitchFor(entity);
			}
			
			return this;
		}
		
	}
}
