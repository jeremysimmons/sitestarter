using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Thrown when attempting an operation which requires entity.IsActivated to be true but is currently false.
	/// </summary>
	public class InactiveEntityException : Exception
	{
		private IEntity entity;
		public IEntity Entity
		{
			get { return entity; }
			set { entity = value; }
		}
		
		public InactiveEntityException(IEntity entity) : base("The '" + entity.ShortTypeName + "' entity has not been activated. Call 'entity.Activate()'.")
		{
			Entity = entity;
		}
	}
}
