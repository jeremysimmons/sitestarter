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
		
		public EntityNotFoundException(string typeName) : base("An entity could not be found with the name '" + typeName + "'.")
		{
			TypeName = typeName;
		}
	}
}
