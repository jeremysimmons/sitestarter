using System;
using System.Data;
using System.Configuration;
using System.Web;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Inherited by all entity components in the application.
    /// </summary>
    [Serializable]
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
        
        public virtual string UniqueKey
        {
        	get { throw new InvalidOperationException("This property must be overridden on type: " + this.GetType().ToString()); }
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
    }
}