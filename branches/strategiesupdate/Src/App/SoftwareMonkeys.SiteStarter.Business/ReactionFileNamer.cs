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
		private string reactionsDirectoryPath;
		/// <summary>
		/// Gets the path to the directory containing serialized reaction component information.
		/// </summary>
		public string ReactionsDirectoryPath
		{
			get {
				if (reactionsDirectoryPath == null || reactionsDirectoryPath == String.Empty)
				{
					if (StateAccess.IsInitialized)
					{
						reactionsDirectoryPath = StateAccess.State.PhysicalApplicationPath
							+ Path.DirectorySeparatorChar + "App_Data"
							+ Path.DirectorySeparatorChar + "Reactions";
					}
				}
				return reactionsDirectoryPath;
			}
			set { reactionsDirectoryPath = value; }
		}
		
		public ReactionFileNamer()
		{
		}
		
		/// <summary>
		/// Creates the file name for the provided reaction.
		/// </summary>
		/// <param name="reaction">The reaction to create the file name for.</param>
		/// <returns>The full file name for the provided reaction.</returns>
		public string CreateFileName(ReactionInfo reaction)
		{			
			if (reaction == null)
				throw new ArgumentNullException("reaction");
			
			if (reaction.Action == null)
				throw new ArgumentNullException("reaction.Action", "No action has been set to the Action property.");
			
			string name = reaction.TypeName + "-" + reaction.Action + ".reaction";
			
			return name;
		}
		
		/// <summary>
		/// Creates the file name for the provided reaction.
		/// </summary>
		/// <param name="reaction">The reaction to create the file name for.</param>
		/// <returns>The full file name for the provided entity.</returns>
		public string CreateFileName(IReaction reaction)
		{
			return CreateFileName(new ReactionInfo(reaction));
		}
		
		
		/// <summary>
		/// Creates the full file path for the provided reaction.
		/// </summary>
		/// <param name="reaction">The reaction to create the file path for.</param>
		/// <returns>The full file path for the provided reaction.</returns>
		public string CreateFilePath(ReactionInfo reaction)
		{
			return ReactionsDirectoryPath + Path.DirectorySeparatorChar + CreateFileName(reaction);
		}
		
		/// <summary>
		/// Creates the full file path for the provided reaction.
		/// </summary>
		/// <param name="reaction">The reaction to create the file path for.</param>
		/// <returns>The full file path for the provided reaction.</returns>
		public string CreateFilePath(IReaction reaction)
		{
			return CreateFilePath(new ReactionInfo(reaction));
		}
		
	}
}
