using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Business;
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
		
		public virtual IEntity Clone()
		{
			//return EntityCloner.Clone(this);
			IEntity newEntity = (IEntity)System.Activator.CreateInstance(GetType());
			
			CopyTo(newEntity);
			
			return newEntity;
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
		
		#region Activation
		
		/// <summary>
		/// Strips all the referenced entities.
		/// </summary>
		// TODO: Remove strip function if not in use
		[Obsolete("Use Deactivate function instead.")]
		public virtual void Strip()
		{
			Deactivate();
		}
		
		/// <summary>
		/// Activates the entity by loading all referenced entities to the relevant properties.
		/// </summary>
		public virtual void Activate()
		{
			if (Activator == null)
				throw new InvalidOperationException("Cannot activate.  No activator has been assigned to the Activator property.");
			
			Activator.Activate(this);
		}
		
		/// <summary>
		/// Deactivates the entity by removing all referenced entities from all reference properties.
		/// </summary>
		public virtual void Deactivate()
		{
			foreach (PropertyInfo property in GetType().GetProperties())
			{
				if (EntitiesUtilities.IsReference(GetType(), property))
				{
					property.SetValue(this, null, null);
				}
			}
		}
		
		private IActivateStrategy activator;
		/// <summary>
		/// Gets/sets the strategy used to activate the entity references.
		/// </summary>
		[XmlIgnore]
		public IActivateStrategy Activator
		{
			get { return activator; }
			set { activator = value; }
		}
		
		private bool isActivated = false;
		/// <summary>
		/// Gets/sets a flag indicating whether the current instance has been activated (ie. the references have been loaded to the properties of the current instance).
		/// </summary>
		[XmlIgnore]
		public bool IsActivated
		{
			get { return isActivated; }
			set { isActivated = value; }
		}
		
		private bool autoActivate = true;
		/// <summary>
		/// Gets/sets a value indicating whether the entity should be automatically activated if necessary. Note: Automatic activation may override changes to references (eg. references being added or removed) unless the entity is manually activated before the changes are made.
		/// </summary>
		[XmlIgnore]
		public bool AutoActivate
		{
			get { return autoActivate; }
			set { autoActivate = value; }
		}
		#endregion
		
		#region Validation
		/// <summary>
		/// Gets a value indicating whether the entity is valid according to the corrensponding validation strategies.
		/// </summary>
		[XmlIgnore]
		public bool IsValid
		{
			get
			{
				// If validation isn't required then it's always valid.
				if (!RequiresValidation)
					return true;
				else
				{
					if (Validator == null)
						throw new InvalidOperationException("Cannot validate entity. No validation strategy has been set to the Validator property.");
					return Validator.Validate(this);
				}
			}
		}
		
		private IValidateStrategy validator;
		/// <summary>
		/// Gets/sets the validation strategy used to validate this entity.
		/// </summary>
		[XmlIgnore]
		public IValidateStrategy Validator
		{
			get { return validator; }
			set { validator = value; }
		}
		
		private bool requiresValidation = true;
		/// <summary>
		/// Gets/sets a value indicating whether the entity requires validation before being saved or updated. Note: Default is true.
		/// </summary>
		[XmlIgnore]
		public bool RequiresValidation
		{
			get { return requiresValidation; }
			set { requiresValidation = value; }
		}
		#endregion
		
		public virtual void PreStore()
		{
		}
	}
}