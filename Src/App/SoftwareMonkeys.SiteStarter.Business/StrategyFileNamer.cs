using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create file names/paths for strategy components.
	/// </summary>
	public class StrategyFileNamer
	{
		private string strategiesDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized strategy component information.
		/// </summary>
		public string StrategiesDirectoryPath
		{
			get {
				if (strategiesDirectoryPath == null || strategiesDirectoryPath == String.Empty)
				{
					if (DataAccess.IsInitialized)
						strategiesDirectoryPath = DataAccess.Data.DataDirectoryPath + Path.DirectorySeparatorChar + "Strategies";
				}
				return strategiesDirectoryPath;
			}
			set { strategiesDirectoryPath = value; }
		}
		
		public StrategyFileNamer()
		{
		}
		
		/// <summary>
		/// Creates the file name for the provided strategy.
		/// </summary>
		/// <param name="strategy">The strategy to create the file name for.</param>
		/// <returns>The full file name for the provided strategy.</returns>
		public string CreateFileName(StrategyInfo strategy)
		{			
			if (strategy == null)
				throw new ArgumentNullException("strategy");
			
			if (strategy.Action == null)
				throw new ArgumentNullException("strategy.Action", "No action has been set to the Action property.");
			
			string name = strategy.Action + "_" + strategy.TypeName + ".strategy";
			
			return name;
		}
		
		/// <summary>
		/// Creates the file name for the provided strategy.
		/// </summary>
		/// <param name="strategy">The strategy to create the file name for.</param>
		/// <returns>The full file name for the provided entity.</returns>
		public string CreateFileName(IStrategy strategy)
		{
			return CreateFileName(new StrategyInfo(strategy));
		}
		
		
		/// <summary>
		/// Creates the full file path for the provided strategy.
		/// </summary>
		/// <param name="strategy">The strategy to create the file path for.</param>
		/// <returns>The full file path for the provided strategy.</returns>
		public string CreateFilePath(StrategyInfo strategy)
		{
			return StrategiesDirectoryPath + Path.DirectorySeparatorChar + CreateFileName(strategy);
		}
		
		/// <summary>
		/// Creates the full file path for the provided strategy.
		/// </summary>
		/// <param name="strategy">The strategy to create the file path for.</param>
		/// <returns>The full file path for the provided strategy.</returns>
		public string CreateFilePath(IStrategy strategy)
		{
			return CreateFilePath(new StrategyInfo(strategy));
		}
		
	}
}
