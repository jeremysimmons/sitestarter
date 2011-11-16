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
		private string strategiesInfoDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized strategy component information.
		/// </summary>
		public string StrategiesInfoDirectoryPath
		{
			get {
				if (strategiesInfoDirectoryPath == null || strategiesInfoDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
					{
						strategiesInfoDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Strategies";
					}
				}
				return strategiesInfoDirectoryPath;
			}
			set { strategiesInfoDirectoryPath = value; }
		}
		
		public StrategyFileNamer()
		{
		}
		
		/// <summary>
		/// Creates the file name for the provided strategy.
		/// </summary>
		/// <param name="strategy">The strategy to create the file name for.</param>
		/// <returns>The full file name for the provided strategy.</returns>
		public string CreateInfoFileName(StrategyInfo strategy)
		{			
			if (strategy == null)
				throw new ArgumentNullException("strategy");
			
			if (strategy.Action == null)
				throw new ArgumentNullException("strategy.Key", "No key has been set to the Key property.");
			
			string name = strategy.Key.Replace("*", "'");
			
			if (strategy is AuthoriseReferenceStrategyInfo)
				name = name + ".ar";
					
			name = name + ".strategy";
			
			return name;
		}
		
		/// <summary>
		/// Creates the full file path for the provided strategy.
		/// </summary>
		/// <param name="strategy">The strategy to create the file path for.</param>
		/// <returns>The full file path for the provided strategy.</returns>
		public string CreateInfoFilePath(StrategyInfo strategy)
		{
			return StrategiesInfoDirectoryPath + Path.DirectorySeparatorChar + CreateInfoFileName(strategy);
		}		
	}
}
