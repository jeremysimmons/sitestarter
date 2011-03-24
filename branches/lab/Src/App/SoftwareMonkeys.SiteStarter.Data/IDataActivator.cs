using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Activates entites by loading referenced entities and populating the appropriate properties.
	/// </summary>
	public interface IDataActivator
	{
		void ActivateReference(EntityReference reference);
		
		void Activate(IEntity[] entity);
		
		void Activate(IEntity[] entity, int depth);
		
		void Activate(IEntity entity);
		
		void Activate(IEntity entity, int depth);
		
		void Activate(IEntity entity, string propertyName);
		
		void Activate(IEntity entity, string propertyName, int depth);
		
		void Activate(IEntity entity, string propertyName, Type propertyType);
		
		void Activate(IEntity entity, string propertyName, Type propertyType, int depth);
		
	}
}
