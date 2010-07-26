using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataActivator.
	/// </summary>
	public abstract class DataActivator : DataAdapter, IDataActivator
	{
		public abstract void ActivateReference(EntityReference reference);
		
		public abstract void Activate(IEntity[] entity);
		
		public abstract void Activate(IEntity[] entity, int depth);
		
		public abstract void Activate(IEntity entity);
		
		public abstract void Activate(IEntity entity, int depth);
		
		public abstract void Activate(IEntity entity, string propertyName);
		
		public abstract void Activate(IEntity entity, string propertyName, int depth);
		
		public abstract void Activate(IEntity entity, string propertyName, Type propertyType);
		
		public abstract void Activate(IEntity entity, string propertyName, Type propertyType, int depth);
		
	}
}
