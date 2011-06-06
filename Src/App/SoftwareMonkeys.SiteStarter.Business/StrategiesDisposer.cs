using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to remove/dispose strategies from the system.
	/// </summary>
	public class StrategiesDisposer
	{
		
		private StrategyFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create strategy file names/paths.
		/// </summary>
		public StrategyFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new StrategyFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		public StrategiesDisposer()
		{
		}
		
		/// <summary>
		/// Disposes the strategies found by the scanner.
		/// </summary>
		public void Dispose()
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the strategies.", NLog.LogLevel.Debug))
			{
				StrategyInfo[] strategies = new StrategyInfo[]{};
				
				Dispose(StrategyState.Strategies.ToArray());
			}
		}
		
		/// <summary>
		/// Disposes the provided strategies.
		/// </summary>
		public void Dispose(StrategyInfo[] strategies)
		{
			using (LogGroup logGroup = LogGroup.Start("Disposing the strategies.", NLog.LogLevel.Debug))
			{
				foreach (StrategyInfo strategy in strategies)
				{
					StrategyState.Strategies.Remove(
						StrategyState.Strategies[strategy.TypeName]
					);
					
					DeleteInfo(strategy);
				}
			}
		}
		
		/// <summary>
		/// Deletes the strategy info file.
		/// </summary>
		/// <param name="info"></param>
		public void DeleteInfo(StrategyInfo info)
		{
			string path = FileNamer.CreateInfoFilePath(info);
			
			File.Delete(path);
		}
	}
}
