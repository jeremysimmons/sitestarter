using System;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to initialize the reaction state and make reactions available for use.
	/// </summary>
	public class ReactionInitializer
	{
		private ReactionFileNamer fileNamer;
		/// <summary>
		/// Gets/sets the file namer used to create reaction file names/paths.
		/// </summary>
		public ReactionFileNamer FileNamer
		{
			get {
				if (fileNamer == null)
					fileNamer = new ReactionFileNamer();
				return fileNamer; }
			set { fileNamer = value; }
		}
		
		private ReactionSaver saver;
		/// <summary>
		/// Gets/sets the reaction saver used to save reactions to file.
		/// </summary>
		public ReactionSaver Saver
		{
			get {
				if (saver == null)
				{
					saver = new ReactionSaver();
					if (ReactionsDirectoryPath != null && ReactionsDirectoryPath != String.Empty)
						saver.ReactionsDirectoryPath = ReactionsDirectoryPath;
				}
				return saver; }
			set { saver = value; }
		}
		
		private ReactionScanner scanner;
		/// <summary>
		/// Gets/sets the reaction scanner used to find available reactions in the existing assemblies.
		/// </summary>
		public ReactionScanner Scanner
		{
			get {
				if (scanner == null)
				{
					scanner = new ReactionScanner();
				}
				return scanner; }
			set { scanner = value; }
		}
		
		private ReactionLoader loader;
		/// <summary>
		/// Gets/sets the reaction loader used to find available reactions in the existing assemblies.
		/// </summary>
		public ReactionLoader Loader
		{
			get {
				if (loader == null)
				{
					loader = new ReactionLoader();
					if (ReactionsDirectoryPath != null && ReactionsDirectoryPath != String.Empty)
						loader.ReactionsDirectoryPath = ReactionsDirectoryPath;
				}
				return loader; }
			set { loader = value; }
		}
		
		/// <summary>
		/// Gets a value indicating whether the reactions have been mapped yet.
		/// </summary>
		public bool IsMapped
		{
			get {
				bool isMapped = ReactionMappingsExist();
				return isMapped; }
		}
		
		/// <summary>
		/// Gets the full path to the directory containing reaction mappings.
		/// </summary>
		public string ReactionsDirectoryPath
		{
			get { return FileNamer.ReactionsInfoDirectoryPath; }
		}
		
		public ReactionInitializer()
		{
		}
		
		
		/// <summary>
		/// Initializes the reactions and loads all reactions to state.
		/// </summary>
		/// <param name="reactions">The reactions to initialize.</param>
		public void Initialize(ReactionInfo[] reactions)
		{
			ReactionState.Reactions = new ReactionStateNameValueCollection(reactions);
		}
		
		/// <summary>
		/// Initializes the reactions and loads all reactions to state.
		/// </summary>
		public void Initialize()
		{
			Initialize(false);
		}
		
		/// <summary>
		/// Initializes the reactions and loads all reactions to state.
		/// </summary>
		/// <param name="includeTestReactions"></param>
		public void Initialize(bool includeTestReactions)
		{
			using (LogGroup logGroup = LogGroup.Start("Initializing the business reactions.", NLog.LogLevel.Debug))
			{
				ReactionInfo[] reactions = new ReactionInfo[]{};
				if (IsMapped)
				{
					LogWriter.Debug("Is mapped. Loading from XML.");
					
					reactions = LoadReactions();
				}
				else
				{
					LogWriter.Debug("Is not mapped. Scanning from type attributes.");
					
					reactions = FindReactions(includeTestReactions);
					SaveToFile(reactions);
				}
				
				Initialize(reactions);
			}
		}
		
		/// <summary>
		/// Saves the mappings for the provided reactions to reactions mappings directory.
		/// </summary>
		/// <param name="reactions">The reactions to save to file.</param>
		public void SaveToFile(ReactionInfo[] reactions)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving the provided reactions to XML.", NLog.LogLevel.Debug))
			{
				foreach (ReactionInfo reaction in reactions)
				{
					Saver.SaveToFile(reaction);
				}
			}
		}
		
		
		/// <summary>
		/// Loads the available reactions from file.
		/// </summary>
		/// <returns>The loaded from the reactions mappings directory.</returns>
		public ReactionInfo[] LoadReactions()
		{
			return Loader.LoadFromDirectory();
		}
		
		/// <summary>
		/// Finds all the reactions available to the application.
		/// </summary>
		/// <param name="includeTestReactions"></param>
		/// <returns>An array of the available reactions.</returns>
		public ReactionInfo[] FindReactions(bool includeTestReactions)
		{
			return Scanner.FindReactions(includeTestReactions);
		}
		
		/// <summary>
		/// Checks whether the reaction mappings have been created and saved to file.
		/// </summary>
		/// <returns>A value indicating whether the reaction mappings directory was found.</returns>
		public bool ReactionMappingsExist()
		{
			string directory = ReactionsDirectoryPath;
			
			return (Directory.Exists(directory) && Directory.GetFiles(directory).Length > 0);
		}
	}
	
}
