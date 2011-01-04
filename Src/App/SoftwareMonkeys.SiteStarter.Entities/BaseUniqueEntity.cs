using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Derived by most (but not necessarily all) unique entities.
	/// </summary>
	[Serializable]
	public class BaseUniqueEntity : BaseEntity, IUniqueEntity
	{
		/// <summary>
		/// Private field for UniqueKey property.
		/// </summary>
		private string uniqueKey = String.Empty;
		
		/// <summary>
		/// A unique value used to identify this entity apart from all others that are similar.
		/// Note: This is not to be used for the GUID which is stored on the ID property.
		/// </summary>
		public virtual string UniqueKey
		{
			get { return uniqueKey; }
			set { uniqueKey = EntitiesUtilities.FormatUniqueKey(value); }
		}
		
		public BaseUniqueEntity()
		{
		}
		
		public BaseUniqueEntity(Guid id) : base(id)
		{}
	}
}
