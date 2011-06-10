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
        private IEntity entity;
        /// <summary>
        /// Gets the entity involved in the event.
        /// </summary>
        public IEntity Entity
        {
            get { return entity; }
        }

        /// <summary>
        /// Sets the ID of the entity involved in the event.
        /// </summary>
        /// <param name="entity">The entity that was involved in the event.</param>
        public EntityEventArgs(IEntity entity)
        {
            this.entity = entity;
        }
    }
}