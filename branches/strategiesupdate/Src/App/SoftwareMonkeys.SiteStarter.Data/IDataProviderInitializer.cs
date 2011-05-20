using System;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the interface of all data provider initializer components.
	/// </summary>
	public interface IDataProviderInitializer
	{
		/// <summary>
		/// Initializes the data provider.
		/// </summary>
		/// <returns>The initialized data provider.</returns>
		DataProvider Initialize();
	}
}
