using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataSaver.
	/// </summary>
	public abstract class DataSaver : DataAdapter, IDataSaver
	{		
		public abstract void Save(IEntity entity);
		
		public abstract void PreSave(IEntity entity);
		
		
	}
}
