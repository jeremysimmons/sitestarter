using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Inherited by all entity components in the application.
    /// </summary>
    [Serializable]
    public abstract class BaseEntity : IEntity
    {
        private Guid id;
        /// <summary>
        /// Gets/sets the ID of the entity.
        /// </summary>
        public Guid ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public BaseEntity()
        {
        }

        /// <summary>
        /// Sets the ID of the entity.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        public BaseEntity(Guid id)
        {
            ID = id;
        }
    }
}