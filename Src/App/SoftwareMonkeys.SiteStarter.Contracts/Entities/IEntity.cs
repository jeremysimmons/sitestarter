using System;
using System.Data;
using System.Configuration;
using System.Web;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Entities
{
    /// <summary>
    /// Defines the interface for all entities.
    /// </summary>
    public interface IEntity
    {
        Guid ID { get;set; }
        string ShortTypeName { get; }
        
		#region Activation   
		/// <summary>
        /// Activates the entity by loading all referenced entities to the relevant properties.
        /// </summary>		
        void Activate();
        
        /// <summary>
        /// Deactivates the entity by removing all referenced entities from all reference properties.
        /// </summary>
        void Deactivate();
        
        /// <summary>
        /// Gets/sets the activate strategy used to activate the entity instance.
        /// </summary>
        IActivateStrategy Activator { get;set; }
        
        /// <summary>
        /// Gets/sets a value indicating whether the entire entity instance has been activated. Note: This will not be true if only some of the properties have been activated.
        /// </summary>
        bool IsActivated { get;set; }
        
        /// <summary>
        /// Gets/sets a value indicating whether the entity should be automatically activated if necessary. Note: Automatic activation may override changes to references (eg. references being added or removed) unless the entity is manually activated before the changes are made.
        /// </summary>
        bool AutoActivate { get;set; }
        
        // TODO: Remove strip function if not in use
        [Obsolete("Use Deactivate function instead.")]
        void Strip();
        #endregion
        
        #region Validation
        /// <summary>
		/// Gets a value indicating whether the entity is valid according to the corrensponding validation strategies.
		/// </summary>
        bool IsValid { get; }
        
		/// <summary>
		/// Gets/sets the validation strategy used to validate this entity.
		/// </summary>
        IValidateStrategy Validator { get;set; }
        #endregion
        
        IEntity Clone();
        void CopyTo(IEntity entity);
        void PreStore();
    }
}