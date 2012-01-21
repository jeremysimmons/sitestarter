using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Indicates that a entity for the provided type and action could not be found.
	/// </summary>
	public class EntityNotFoundException : ApplicationException
	{		
		private string typeName;
		/// <summary>
		/// Gets/sets the name of the type involved in the entity.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private Guid entityID;
		public Guid EntityID
		{
			get { return entityID; }
			set { entityID = value; }
		}
		
		public EntityNotFoundException(string typeName) : base("An entity could not be found with the name '" + typeName + "'.")
		{
			TypeName = typeName;
		}
		
		public EntityNotFoundException(string typeName, Guid entityID) : base("An entity could not be found with the name '" + typeName + "' and ID '" + entityID.ToString() + "'.")
		{
			TypeName = typeName;
			EntityID = entityID;
		}
	}
}
