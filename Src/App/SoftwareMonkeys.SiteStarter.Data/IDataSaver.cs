using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// 
	/// </summary>
	public interface IDataSaver : IDataAdapter
	{		
		void Save(IEntity entity);
		
		void Save(IEntity entity, bool handleReferences);
		
		void PreSave(IEntity entity, bool handleReferences);
		
	}
}
