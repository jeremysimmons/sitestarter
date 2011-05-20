using System;
using System.Data;
using System.Configuration;
using System.Web;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Inherited by all entity components in the application.
	/// </summary>
	[Serializable]
	[Entity]
	public abstract class BaseEntity : IEntity
	{
		private Guid id = Guid.NewGuid();
		/// <summary>
		/// Gets/sets the ID of the entity.
		/// </summary>
		public Guid ID
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string ShortTypeName
		{
			get { return GetType().Name; }
		}

		/// <summary>
		/// Empty constructor.
		/// </summary>
		protected BaseEntity()
		{
		}

		/// <summary>
		/// Sets the ID of the entity.
		/// </summary>
		/// <param name="id">The ID of the entity.</param>
		protected BaseEntity(Guid id)
		{
			ID = id;
		}
		
		public BaseEntity Clone()
		{
			return ObjectCloner.Clone(this);
		}
		
		IEntity IEntity.Clone()
		{
			return (IEntity)this.Clone();
		}
		
		public virtual void CopyTo(IEntity entity)
		{
			foreach (PropertyInfo property in entity.GetType().GetProperties())
			{
				if (property.CanWrite)
				{					
					object value = EntitiesUtilities.GetPropertyValue(this, property.Name);
					
					property.SetValue(entity, value, null);
				}
			}
		}
		
		
		/// <summary>
		/// Strips all the referenced entities.
		/// </summary>
		public virtual void Strip()
		{
			foreach (PropertyInfo property in GetType().GetProperties())
			{
				if (EntitiesUtilities.IsReference(GetType(), property))
				{
					property.SetValue(this, null, null);
				}
			}
		}
		
		public virtual void PreStore()
		{
		}
	}
}