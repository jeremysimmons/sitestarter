using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create file names/paths for strategy components.
	/// </summary>
	public class StrategyFileNamer
	{
		private string strategiesInfoFilePath;
		/// <summary>
		/// Gets the path to the file containing the strategy info.
		/// </summary>
		public virtual string StrategiesInfoFilePath
		{
			get {
				if (strategiesInfoFilePath == null || strategiesInfoFilePath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						strategiesInfoFilePath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Strategies.xml";
				}
				return strategiesInfoFilePath;
			}
			set { strategiesInfoFilePath = value; }
		}
		
		public StrategyFileNamer()
		{
		}
	}
}
