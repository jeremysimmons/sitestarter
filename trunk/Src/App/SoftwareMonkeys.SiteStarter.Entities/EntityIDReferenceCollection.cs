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
		
		public EntityIDReferenceCollection(EntityIDReference[] references)
		{
			if (references != null)
			{
				foreach (EntityIDReference reference in references)
				{
					if (reference != null)
						Add(reference);
				}
			}
		}
		
		public new void Add(EntityIDReference reference)
		{
			if (reference == null)
				throw new ArgumentNullException("reference");
			
			if (!Contains(reference))
			{
				referenceTypeName = reference.Type2Name;
				
				base.Add(reference);
			}
		}
		
		public void AddRange(EntityIDReference[] references)
		{
			foreach (EntityIDReference reference in references)
			{
				Add(reference);
			}
		}
		
		public EntityIDReferenceCollection SwitchFor(IEntity entity)
		{
			for (int i = 0; i < Count; i ++)
			{
				this[i] = this[i].SwitchFor(entity);
			}
			
			return this;
		}
		
		public new bool Contains(EntityIDReference reference)
		{
			bool match = false;
			foreach (EntityIDReference r in this)
			{
				if (r.Includes(reference.Entity1ID, reference.Property1Name)
				    && r.Includes(reference.Entity2ID, reference.Property2Name))
				{
					match = true;
				}
			}
			return match;
		}
		
		/// <summary>
		/// Retrieves all the IDs of the entities from the collection that are associated with the specified entity.
		/// </summary>
		/// <param name="sourceEntityID">The entity that the referenced IDs are being retrieved for. If this is Guid.Empty then all IDs from both sides of the reference are returned.</param>
		/// <returns>The IDs of the referenced entities.</returns>
		public Guid[] GetEntityIDs(Guid sourceEntityID)
		{
			List<Guid> list = new List<Guid>();
			foreach (EntityReference reference in this)
			{
				if (sourceEntityID == Guid.Empty || reference.Entity1ID == sourceEntityID)
				{
					if (!list.Contains(reference.Entity2ID))
						list.Add(reference.Entity2ID);
				}
				
				if (sourceEntityID == Guid.Empty || reference.Entity2ID == sourceEntityID)
				{
					if (!list.Contains(reference.Entity1ID))
						list.Add(reference.Entity1ID);
				}
			}
			
			return list.ToArray();
		}
	}
}
