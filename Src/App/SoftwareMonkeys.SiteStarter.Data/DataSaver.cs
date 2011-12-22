using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class DataSaver : DataAdapter, IDataSaver
	{		
		public virtual void Save(IEntity entity)
		{
			Save(entity, true);
		}
		
		public abstract void Save(IEntity entity, bool handleReferences);
		
		public abstract void PreSave(IEntity entity, bool handleReferences);
		
		
	}
}
