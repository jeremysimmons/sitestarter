using System;
using System.IO;

using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create file names/paths for reaction components.
	/// </summary>
	public class ReactionFileNamer
	{
		private string reactionsInfoFilePath;
		/// <summary>
		/// Gets the path to the file containing the reaction info.
		/// </summary>
		public virtual string ReactionsInfoFilePath
		{
			get {
				if (reactionsInfoFilePath == null || reactionsInfoFilePath == String.Empty)
				{
					if (StateAccess.IsInitialized)
						reactionsInfoFilePath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Reactions.xml";
				}
				return reactionsInfoFilePath;
			}
			set { reactionsInfoFilePath = value; }
		}
		
		public ReactionFileNamer()
		{
		}
	}
}
