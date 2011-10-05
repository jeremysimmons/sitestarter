using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataUpdater.
	/// </summary>
	public abstract class DataUpdater : DataAdapter, IDataUpdater
	{		
		public abstract void Update(IEntity entity);
		
		public abstract void PreUpdate(IEntity entity);
		
	}
}
