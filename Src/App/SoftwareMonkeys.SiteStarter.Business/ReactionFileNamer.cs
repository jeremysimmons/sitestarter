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
		private string reactionsInfoDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized reaction component information.
		/// </summary>
		public string ReactionsInfoDirectoryPath
		{
			get {
				if (reactionsInfoDirectoryPath == null || reactionsInfoDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
					{
						reactionsInfoDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Reactions";
					}
				}
				return reactionsInfoDirectoryPath;
			}
			set { reactionsInfoDirectoryPath = value; }
		}
		
		public ReactionFileNamer()
		{
		}
		
		/// <summary>
		/// Creates the file name for the provided reaction.
		/// </summary>
		/// <param name="reaction">The reaction to create the file name for.</param>
		/// <returns>The full file name for the provided reaction.</returns>
		public string CreateInfoFileName(ReactionInfo reaction)
		{			
			if (reaction == null)
				throw new ArgumentNullException("reaction");
			
			if (reaction.Action == null)
				throw new ArgumentNullException("reaction.Action", "No action has been set to the Action property.");
			
			string name = reaction.TypeName + "-" + reaction.Action + ".reaction";
			
			return name;
		}
		
		
		/// <summary>
		/// Creates the full file path for the provided reaction.
		/// </summary>
		/// <param name="reaction">The reaction to create the file path for.</param>
		/// <returns>The full file path for the provided reaction.</returns>
		public string CreateInfoFilePath(ReactionInfo reaction)
		{
			return ReactionsInfoDirectoryPath + Path.DirectorySeparatorChar + CreateInfoFileName(reaction);
		}
		
	}
}
