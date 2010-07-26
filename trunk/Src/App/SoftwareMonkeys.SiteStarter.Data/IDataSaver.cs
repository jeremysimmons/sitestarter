using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of IDataSaver.
	/// </summary>
	public interface IDataSaver : IDataAdapter
	{		
		void Save(IEntity entity);
		
		void PreSave(IEntity entity);
		
	}
}
