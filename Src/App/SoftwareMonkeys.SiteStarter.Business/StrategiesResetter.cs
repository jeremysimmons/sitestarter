using System;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to reset the strategies so they can be re-scanned and re-intialized.
	/// </summary>
	public class StrategiesResetter
	{
		public StrategiesResetter()
		{
		}
		
		public void Reset()
		{
			// Dispose the in-memory state
			new StrategiesDisposer().Dispose();
			
			string path = new StrategyFileNamer().StrategiesInfoDirectoryPath;
			
			// Delete strategy info (not the actual strategies)
			foreach (string file in Directory.GetFiles(path))
			{
				File.Delete(file);
			}
			
			// Now the strategies can be re-scanned and re-initialized
		}
	}
}
