using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Derived by most (but not necessarily all) unique entities.
	/// </summary>
	public class BaseUniqueEntity : BaseEntity, IUniqueEntity
	{
		/// <summary>
		/// A unique value used to identify this entity apart from all others that are similar.
		/// Note: This is not to be used for the GUID which is stored on the ID property.
		/// </summary>
		public virtual string UniqueKey
		{
			get { throw new InvalidOperationException("This property must be overridden on type: " + this.GetType().ToString()); }
		}
		
		public BaseUniqueEntity()
		{
		}
		
		public BaseUniqueEntity(Guid id) : base(id)
		{
		}
	}
}
