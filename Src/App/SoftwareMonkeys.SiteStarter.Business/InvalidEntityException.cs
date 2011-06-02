using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Thrown when attempting an operation which requires entity.IsValid to be true but is currently false.
	/// </summary>
	public class InvalidEntityException : Exception
	{
		private IEntity entity;
		public IEntity Entity
		{
			get { return entity; }
			set { entity = value; }
		}
		
		public InvalidEntityException(IEntity entity) : base("The '" + entity.ShortTypeName + "' entity has not been validated.")
		{
			Entity = entity;
		}
	}
}
