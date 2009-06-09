using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Contains arguments for an event associated with an entity.
    /// </summary>
    public class EntityEventArgs : EventArgs
    {
        private BaseEntity entity;
        /// <summary>
        /// Gets the entity involved in the event.
        /// </summary>
        public BaseEntity Entity
        {
            get { return entity; }
        }

        /// <summary>
        /// Sets the ID of the entity involved in the event.
        /// </summary>
        /// <param name="entity">The entity that was involved in the event.</param>
        public EntityEventArgs(BaseEntity entity)
        {
            this.entity = entity;
        }
    }
}